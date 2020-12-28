using Microsoft.EntityFrameworkCore;
using Storage.Classes.Models.Omemo;

namespace Storage.Classes.Contexts
{
    public class OmemoDbContext: AbstractDbContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DbSet<OmemoAccountInformation> AccountInfos { get; set; }
        public DbSet<OmemoChatInformation> ChatInfos { get; set; }
        public DbSet<OmemoDevice> Devices { get; set; }
        public DbSet<OmemoDeviceListSubscription> DeviceListSubscriptions { get; set; }
        public DbSet<OmemoFingerprint> Fingerprints { get; set; }
        public DbSet<OmemoIdentityKey> IdentityKeys { get; set; }
        public DbSet<OmemoPreKey> PreKeys { get; set; }
        public DbSet<OmemoSession> Sessions { get; set; }
        public DbSet<OmemoSignedPreKey> SignedPreKeys { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
