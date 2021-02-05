using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;
using Storage.Classes.Models.Omemo;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace Storage.Classes.Contexts
{
    public class MainDbContext: AbstractDbContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<JidModel> Jids { get; set; }
        public DbSet<ServerModel> Servers { get; set; }
        public DbSet<IgnoredCertificateErrorModel> IgnoredCertificateErrors { get; set; }
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
        public DbSet<OmemoSessionModel> Sessions { get; set; }
        public DbSet<SignedPreKeyModel> SignedPreKeys { get; set; }
        public DbSet<ECKeyPairModel> ECKeyPairs { get; set; }
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
            return Chats.Where(c => string.Equals(c.accountBareJid, accountBareJid) && string.Equals(c.bareJid, chatBareJid)).FirstOrDefault();
        }

        public MucInfoModel GetMucInfo(string accountBareJid, string chatBareJid)
        {
            return MucInfos.Where(c => string.Equals(c.chat.accountBareJid, accountBareJid) && string.Equals(c.chat.bareJid, chatBareJid)).FirstOrDefault();
        }

        public int GetUnreadMessageCount(int chatId)
        {
            return ChatMessages.Where(m => m.chat.id == chatId).Count();
        }

        public List<ConferenceItem> GetXEP0048ConferenceItemsForAccount(string accountBareJid)
        {
            return MucInfos.Where(muc => muc.chat.inRoster && string.Equals(muc.chat.accountBareJid, accountBareJid)).Select(muc => muc.ToConferenceItem()).ToList();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatMessageModel>().Has // TODO: Continue here
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
}
