using Shared.Classes;
using Shared.Classes.Collections;
using Windows.Security.Cryptography.Certificates;
using XMPP_API.Classes.Network.TCP;

namespace XMPP_API.Classes.Network
{
    public class ConnectionConfiguration: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private TLSConnectionMode _tlsMode;
        public TLSConnectionMode tlsMode
        {
            get => _tlsMode;
            set => SetProperty(ref _tlsMode, value);
        }
        private bool _disableStreamManagement;
        public bool disableStreamManagement
        {
            get => _disableStreamManagement;
            set => SetProperty(ref _disableStreamManagement, value);
        }
        private bool _disableMessageCarbons;
        public bool disableMessageCarbons
        {
            get => _disableMessageCarbons;
            set => SetProperty(ref _disableMessageCarbons, value);
        }
        public readonly CustomObservableCollection<ChainValidationResult> IGNORED_CERTIFICATE_ERRORS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 13/04/2018 Created [Fabian Sauter]
        /// </history>
        public ConnectionConfiguration()
        {
            invokeInUiThread = false;
            tlsMode = TLSConnectionMode.FORCE;
            IGNORED_CERTIFICATE_ERRORS = new CustomObservableCollection<ChainValidationResult>(false);
            IGNORED_CERTIFICATE_ERRORS.CollectionChanged += IGNORED_CERTIFICATE_ERRORS_CollectionChanged;
            disableStreamManagement = false;
            disableMessageCarbons = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            if (obj is ConnectionConfiguration c && c != null)
            {
                return c.tlsMode == tlsMode && c.IGNORED_CERTIFICATE_ERRORS.Equals(IGNORED_CERTIFICATE_ERRORS) && disableStreamManagement == c.disableStreamManagement && disableMessageCarbons == c.disableMessageCarbons;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void IGNORED_CERTIFICATE_ERRORS_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IGNORED_CERTIFICATE_ERRORS));
        }

        #endregion
    }
}
