using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class ClearCacheDialogDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _CleaningCache;
        public bool IsCleaningCache
        {
            get => _CleaningCache;
            set => SetProperty(ref _CleaningCache, value);
        }

        // Chat:
        private bool _ChatMessages;
        public bool ChatMessages
        {
            get => _ChatMessages;
            set => SetProperty(ref _ChatMessages, value);
        }
        private bool _Chats;
        public bool Chats
        {
            get => _Chats;
            set => SetProperty(ref _Chats, value);
        }
        private bool _Images;
        public bool Images
        {
            get => _Images;
            set => SetProperty(ref _Images, value);
        }

        // Disco:
        private bool _DiscoFeatures;
        public bool DiscoFeatures
        {
            get => _DiscoFeatures;
            set => SetProperty(ref _DiscoFeatures, value);
        }
        private bool _DiscoIdentities;
        public bool DiscoIdentities
        {
            get => _DiscoIdentities;
            set => SetProperty(ref _DiscoIdentities, value);
        }
        private bool _DiscoItems;
        public bool DiscoItems
        {
            get => _DiscoItems;
            set => SetProperty(ref _DiscoItems, value);
        }

        // MUC:
        private bool _MucChatInfo;
        public bool MucChatInfo
        {
            get => _MucChatInfo;
            set => SetProperty(ref _MucChatInfo, value);
        }
        private bool _MucOccupants;
        public bool MucOccupants
        {
            get => _MucOccupants;
            set => SetProperty(ref _MucOccupants, value);
        }
        private bool _MucDirectInvites;
        public bool MucDirectInvites
        {
            get => _MucDirectInvites;
            set => SetProperty(ref _MucDirectInvites, value);
        }

        // Accounts:
        private bool _Accounts;
        public bool Accounts
        {
            get => _Accounts;
            set => SetProperty(ref _Accounts, value);
        }
        private bool _PasswordVault;
        public bool PasswordVault
        {
            get => _PasswordVault;
            set => SetProperty(ref _PasswordVault, value);
        }
        private bool _IgnoredCertErrors;
        public bool IgnoredCertErrors
        {
            get => _IgnoredCertErrors;
            set => SetProperty(ref _IgnoredCertErrors, value);
        }
        private bool _ConnectionOptions;
        public bool ConnectionOptions
        {
            get => _ConnectionOptions;
            set => SetProperty(ref _ConnectionOptions, value);
        }

        // OMEMO:
        private bool _OmemoDeviceListSubscriptions;
        public bool OmemoDeviceListSubscriptions
        {
            get => _OmemoDeviceListSubscriptions;
            set => SetProperty(ref _OmemoDeviceListSubscriptions, value);
        }
        private bool _OmemoDevices;
        public bool OmemoDevices
        {
            get => _OmemoDevices;
            set => SetProperty(ref _OmemoDevices, value);
        }
        private bool _OmemoIdentityKeys;
        public bool OmemoIdentityKeys
        {
            get => _OmemoIdentityKeys;
            set => SetProperty(ref _OmemoIdentityKeys, value);
        }
        private bool _OmemoPreKeys;
        public bool OmemoPreKeys
        {
            get => _OmemoPreKeys;
            set => SetProperty(ref _OmemoPreKeys, value);
        }
        private bool _OmemoSignedPreKeys;
        public bool OmemoSignedPreKeys
        {
            get => _OmemoSignedPreKeys;
            set => SetProperty(ref _OmemoSignedPreKeys, value);
        }
        private bool _OmemoSessions;
        public bool OmemoSessions
        {
            get => _OmemoSessions;
            set => SetProperty(ref _OmemoSessions, value);
        }

        // Clients:
        private bool _ReloadClients;
        public bool ReloadClients
        {
            get => _ReloadClients;
            set => SetProperty(ref _ReloadClients, value);
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
