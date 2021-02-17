using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Shared.Classes.Collections;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;
using Storage.Classes.Models.Omemo;
using Windows.Security.Cryptography.Certificates;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace Storage.Classes.Contexts
{
    public class MainDbContext: AbstractDbContext, IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<JidModel> Jids { get; set; }
        public DbSet<ServerModel> Servers { get; set; }
        public DbSet<MamRequestModel> MamRequests { get; set; }

        public DbSet<ChatModel> Chats { get; set; }
        public DbSet<MucInfoModel> MucInfos { get; set; }
        public DbSet<MucOccupantModel> MucOccupants { get; set; }
        public DbSet<ChatMessageModel> ChatMessages { get; set; }
        public DbSet<ChatMessageImageModel> ChatMessageImages { get; set; }
        public DbSet<SpamMessageModel> SpamMessages { get; set; }
        public DbSet<MucDirectInvitationModel> MucDirectInvitations { get; set; }

        public DbSet<OmemoAccountInformationModel> AccountInfos { get; set; }
        public DbSet<OmemoChatInformationModel> ChatInfos { get; set; }
        public DbSet<OmemoDeviceModel> Devices { get; set; }
        public DbSet<OmemoDeviceListSubscriptionModel> DeviceListSubscriptions { get; set; }
        public DbSet<OmemoFingerprintModel> Fingerprints { get; set; }
        public DbSet<IdentityKeyPairModel> IdentityKeyPairs { get; set; }
        public DbSet<PreKeyModel> PreKeys { get; set; }
        public DbSet<ECKeyModel> ECKeys { get; set; }
        public DbSet<ECPubKeyModel> ECPubKeys { get; set; }
        public DbSet<ECPrivKeyModel> ECPrivKeys { get; set; }
        public DbSet<OmemoSessionModel> Sessions { get; set; }
        public DbSet<SignedPreKeyModel> SignedPreKeys { get; set; }
        public DbSet<SkippedMessageKeyGroupModel> SkippedMessageKeyGroup { get; set; }
        public DbSet<SkippedMessageKeyGroupsModel> SkippedMessageKeyGroups { get; set; }
        public DbSet<SkippedMessageKeyModel> SkippedMessageKeys { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public ChatModel GetChat(string accountBareJid, string chatBareJid)
        {
            return Chats.Where(c => string.Equals(c.accountBareJid, accountBareJid) && string.Equals(c.bareJid, chatBareJid)).Include(GetIncludePaths(typeof(ChatModel))).FirstOrDefault();
        }

        public int GetUnreadMessageCount(int chatId)
        {
            return ChatMessages.Where(m => m.chatId == chatId).Count();
        }

        public int GetUnreadMessageCount()
        {
            return ChatMessages.Where(m => m.state == MessageState.UNREAD).Join(Chats, m => m.chatId, c => c.id, (m, c) => new { m, c.muted }).Where(t => !t.muted).Count();
        }

        public List<ConferenceItem> GetXEP0048ConferenceItemsForAccount(string accountBareJid)
        {
            return MucInfos.Where(muc => muc.chat.inRoster && string.Equals(muc.chat.accountBareJid, accountBareJid)).Select(muc => muc.ToConferenceItem()).ToList();
        }

        public IEnumerable<ChatMessageModel> GetNextNChatMessages(ChatModel chat, int n)
        {
            return ChatMessages.Where(msg => msg.chatId == chat.id).OrderBy(msg => msg.date).Take(n).Include(GetIncludePaths(typeof(ChatMessageModel))).ToList();
        }

        public IEnumerable<ChatMessageModel> GetNextNChatMessages(ChatModel chat, ChatMessageModel lastMessage, int n)
        {
            if (lastMessage is null)
            {
                return GetNextNChatMessages(chat, n);
            }
            return ChatMessages.Where(msg => msg.chatId == chat.id).OrderBy(msg => msg.date).SkipWhile(msg => msg.id != lastMessage.id).Skip(1).Take(n).Include(GetIncludePaths(typeof(ChatMessageModel))).ToList();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void Dispose()
        {
            SaveChanges();
            base.Dispose();
        }

        public IEnumerable<string> GetIncludePaths(Type clrEntityType, int maxDepth = int.MaxValue)
        {
            IEntityType entityType = Model.FindEntityType(clrEntityType);
            Stack<INavigation> navHirar = new Stack<INavigation>();
            HashSet<INavigation> navHirarProps = new HashSet<INavigation>();

            HashSet<string> paths = new HashSet<string>();
            GetIncludePathsRec(navHirar, navHirarProps, entityType, paths, 0, maxDepth);
            return paths;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void GetIncludePathsRec(Stack<INavigation> navHirar, HashSet<INavigation> navHirarProps, IEntityType entityType, HashSet<string> paths, int curDepth, int maxDepth)
        {
            if (curDepth < maxDepth)
            {
                foreach (INavigation navigation in entityType.GetNavigations())
                {
                    if (navHirarProps.Add(navigation))
                    {
                        navHirar.Push(navigation);
                        GetIncludePathsRec(navHirar, navHirarProps, navigation.GetTargetType(), paths, curDepth + 1, maxDepth);
                        navHirar.Pop();
                        navHirarProps.Remove(navigation);
                    }
                }
            }
            if (navHirar.Count > 0)
            {
                paths.Add(string.Join(".", navHirar.Reverse().Select(e => e.Name)));
            }
        }

        private ValueConverter<CustomObservableCollection<T>, List<T>> NewCustomObservableCollectionToListValueConverter<T>()
        {
            return new ValueConverter<CustomObservableCollection<T>, List<T>>(v => v.ToList(), v => new CustomObservableCollection<T>(v, true));
        }
        #endregion

        #region --Misc Methods (Protected)--
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Store the list of ChainValidationResults as a string separated by ',':
            modelBuilder.Entity<ServerModel>().Property(p => p.ignoredCertificateErrors).HasConversion(v => string.Join(',', v.Select(i => (int)i)), v => new CustomObservableCollection<ChainValidationResult>(v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => (ChainValidationResult)int.Parse(i)), true));

            modelBuilder.Entity<MucInfoModel>().Property(p => p.occupants).HasConversion(NewCustomObservableCollectionToListValueConverter<MucOccupantModel>());
            modelBuilder.Entity<OmemoAccountInformationModel>().Property(p => p.preKeys).HasConversion(NewCustomObservableCollectionToListValueConverter<PreKeyModel>());
            modelBuilder.Entity<OmemoAccountInformationModel>().Property(p => p.devices).HasConversion(NewCustomObservableCollectionToListValueConverter<OmemoDeviceModel>());
            modelBuilder.Entity<OmemoChatInformationModel>().Property(p => p.devices).HasConversion(NewCustomObservableCollectionToListValueConverter<OmemoDeviceModel>());
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }

    /// <summary>
    /// Based on: https://stackoverflow.com/questions/49593482/entity-framework-core-2-0-1-eager-loading-on-all-nested-related-entities/49597502#49597502
    /// </summary>
    public static partial class CustomQueryExtensions
    {
        public static IQueryable<T> Include<T>(this IQueryable<T> source, IEnumerable<string> navigationPropertyPaths) where T : class
        {
            return navigationPropertyPaths.Aggregate(source, (query, path) => query.Include(path));
        }
    }
}
