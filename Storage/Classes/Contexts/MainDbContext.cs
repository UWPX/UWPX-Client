using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Omemo.Classes;
using Omemo.Classes.Keys;
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Store the list of ChainValidationResults as a string separated by ',':
            modelBuilder.Entity<ServerModel>().Property(l => l.ignoredCertificateErrors).HasConversion(v => string.Join(',', v.Select(i => (int)i)), v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => (ChainValidationResult)int.Parse(i)).ToList());
        }

        public override void Dispose()
        {
            SaveChanges();
            base.Dispose();
        }

        /// <summary>
        /// Based on: https://stackoverflow.com/questions/49593482/entity-framework-core-2-0-1-eager-loading-on-all-nested-related-entities/49597502#49597502
        /// </summary>
        public IEnumerable<string> GetIncludePaths(Type clrEntityType, int maxDepth = int.MaxValue)
        {
            if (maxDepth < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxDepth));
            }

            IEntityType entityType = Model.FindEntityType(clrEntityType);
            HashSet<INavigation> includedNavigations = new HashSet<INavigation>();
            Stack<IEnumerator<INavigation>> stack = new Stack<IEnumerator<INavigation>>();
            while (true)
            {
                List<INavigation> entityNavigations = new List<INavigation>();
                if (stack.Count <= maxDepth)
                {
                    foreach (INavigation navigation in entityType.GetNavigations())
                    {
                        if (includedNavigations.Add(navigation))
                        {
                            entityNavigations.Add(navigation);
                        }
                    }
                }
                if (entityNavigations.Count == 0)
                {
                    if (stack.Count > 0)
                    {
                        yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
                    }
                }
                else
                {
                    foreach (INavigation navigation in entityNavigations)
                    {
                        INavigation inverseNavigation = navigation.FindInverse();
                        if (inverseNavigation != null)
                        {
                            includedNavigations.Add(inverseNavigation);
                        }
                    }
                    stack.Push(entityNavigations.GetEnumerator());
                }
                while (stack.Count > 0 && !stack.Peek().MoveNext())
                {
                    stack.Pop();
                }

                if (stack.Count == 0)
                {
                    break;
                }

                entityType = stack.Peek().Current.GetTargetType();
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


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
