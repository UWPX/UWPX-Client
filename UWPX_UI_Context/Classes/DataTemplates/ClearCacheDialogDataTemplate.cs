namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class ClearCacheDialogDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _CleaningCache;
        public bool IsCleaningCache
        {
            get { return _CleaningCache; }
            set { SetProperty(ref _CleaningCache, value); }
        }

        // Chat:
        private bool _ChatMessages;
        public bool ChatMessages
        {
            get { return _ChatMessages; }
            set { SetProperty(ref _ChatMessages, value); }
        }
        private bool _Chats;
        public bool Chats
        {
            get { return _Chats; }
            set { SetProperty(ref _Chats, value); }
        }
        private bool _Images;
        public bool Images
        {
            get { return _Images; }
            set { SetProperty(ref _Images, value); }
        }

        // Disco:
        private bool _DiscoFeatures;
        public bool DiscoFeatures
        {
            get { return _DiscoFeatures; }
            set { SetProperty(ref _DiscoFeatures, value); }
        }
        private bool _DiscoIdentities;
        public bool DiscoIdentities
        {
            get { return _DiscoIdentities; }
            set { SetProperty(ref _DiscoIdentities, value); }
        }
        private bool _DiscoItems;
        public bool DiscoItems
        {
            get { return _DiscoItems; }
            set { SetProperty(ref _DiscoItems, value); }
        }

        // MUC:
        private bool _MucChatInfo;
        public bool MucChatInfo
        {
            get { return _MucChatInfo; }
            set { SetProperty(ref _MucChatInfo, value); }
        }
        private bool _MucOccupants;
        public bool MucOccupants
        {
            get { return _MucOccupants; }
            set { SetProperty(ref _MucOccupants, value); }
        }
        private bool _MucDirectInvites;
        public bool MucDirectInvites
        {
            get { return _MucDirectInvites; }
            set { SetProperty(ref _MucDirectInvites, value); }
        }

        // Accounts:
        private bool _Accounts;
        public bool Accounts
        {
            get { return _Accounts; }
            set { SetProperty(ref _Accounts, value); }
        }
        private bool _PasswordVault;
        public bool PasswordVault
        {
            get { return _PasswordVault; }
            set { SetProperty(ref _PasswordVault, value); }
        }
        private bool _IgnoredCertErrors;
        public bool IgnoredCertErrors
        {
            get { return _IgnoredCertErrors; }
            set { SetProperty(ref _IgnoredCertErrors, value); }
        }
        private bool _ConnectionOptions;
        public bool ConnectionOptions
        {
            get { return _ConnectionOptions; }
            set { SetProperty(ref _ConnectionOptions, value); }
        }

        // OMEMO:
        private bool _OmemoDeviceListSubscriptions;
        public bool OmemoDeviceListSubscriptions
        {
            get { return _OmemoDeviceListSubscriptions; }
            set { SetProperty(ref _OmemoDeviceListSubscriptions, value); }
        }
        private bool _OmemoDevices;
        public bool OmemoDevices
        {
            get { return _OmemoDevices; }
            set { SetProperty(ref _OmemoDevices, value); }
        }
        private bool _OmemoIdentityKeys;
        public bool OmemoIdentityKeys
        {
            get { return _OmemoIdentityKeys; }
            set { SetProperty(ref _OmemoIdentityKeys, value); }
        }
        private bool _OmemoPreKeys;
        public bool OmemoPreKeys
        {
            get { return _OmemoPreKeys; }
            set { SetProperty(ref _OmemoPreKeys, value); }
        }
        private bool _OmemoSignedPreKeys;
        public bool OmemoSignedPreKeys
        {
            get { return _OmemoSignedPreKeys; }
            set { SetProperty(ref _OmemoSignedPreKeys, value); }
        }
        private bool _OmemoSessions;
        public bool OmemoSessions
        {
            get { return _OmemoSessions; }
            set { SetProperty(ref _OmemoSessions, value); }
        }

        // Clients:
        private bool _ReloadClients;
        public bool ReloadClients
        {
            get { return _ReloadClients; }
            set { SetProperty(ref _ReloadClients, value); }
        }

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
