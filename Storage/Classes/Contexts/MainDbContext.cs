using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
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
        public DbSet<PushAccountModel> PushAccounts { get; set; }
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
            return ChatMessages.Where(m => m.state == MessageState.UNREAD && m.chatId == chatId).Count();
        }

        public int GetUnreadMessageCount()
        {
            return ChatMessages.Where(m => m.state == MessageState.UNREAD).Join(Chats, m => m.chatId, c => c.id, (m, c) => new { m, c.muted }).Where(t => !t.muted).Count();
        }

        public List<ConferenceItem> GetXEP0048ConferenceItemsForAccount(string accountBareJid)
        {
            return MucInfos.Where(muc => muc.chat.inRoster && string.Equals(muc.chat.accountBareJid, accountBareJid)).Include(GetIncludePaths(typeof(MucInfoModel))).Select(muc => muc.ToConferenceItem()).ToList();
        }

        public IEnumerable<ChatMessageModel> GetNextNChatMessages(ChatModel chat, int n)
        {
            List<ChatMessageModel> msgs = ChatMessages.Where(msg => msg.chatId == chat.id).OrderByDescending(msg => msg.date).Take(n).Include(GetIncludePaths(typeof(ChatMessageModel))).ToList();
            msgs.Reverse();
            return msgs;

        }

        public IEnumerable<ChatMessageModel> GetNextNChatMessages(ChatModel chat, ChatMessageModel lastMessage, int n)
        {
            if (lastMessage is null)
            {
                return GetNextNChatMessages(chat, n);
            }
            IEnumerable<ChatMessageModel> msgs = ChatMessages.Where(msg => msg.chatId == chat.id).OrderByDescending(msg => msg.date).Include(GetIncludePaths(typeof(ChatMessageModel))).ToList();
            return msgs.SkipWhile(msg => msg.id != lastMessage.id).Skip(1).Take(n).Reverse();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void Dispose()
        {
            try
            {
                SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Logger.Error("DB inconsistency found: ", ex);
                foreach (EntityEntry entry in ex.Entries)
                {
                    try
                    {
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"DB inconsistency fix failed for '{entry.Entity.GetType()}'. Trying the other way around.", e);
                        try
                        {
                            entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                        }
                        catch (Exception exc)
                        {
                            Logger.Error($"Second DB inconsistency fix failed for '{entry.Entity.GetType()}'.", exc);
                            break;
                        }
                        if (Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }
                    }
                }
            }
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
        #endregion

        #region --Misc Methods (Protected)--
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Store the list of ChainValidationResults as a string separated by ',':
            modelBuilder.Entity<ServerModel>().Property(p => p.ignoredCertificateErrors).HasConversion(v => string.Join(',', v.Select(i => (int)i)), v => new CustomObservableCollection<ChainValidationResult>(v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => (ChainValidationResult)int.Parse(i)), true));
            IEnumerable<IMutableForeignKey> x = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());

            // Ensure we perform cascade deletion when ever possible:
            foreach (IMutableForeignKey foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Cascade;
            }
            // Except for this one since else we would get a recursive loop:
            modelBuilder.Entity<MucInfoModel>().HasOne(m => m.chat).WithOne(c => c.muc).OnDelete(DeleteBehavior.Restrict);
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
