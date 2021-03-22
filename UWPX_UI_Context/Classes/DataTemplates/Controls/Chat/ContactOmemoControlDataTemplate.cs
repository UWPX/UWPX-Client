using Manager.Classes.Chat;
using Shared.Classes;
using Shared.Classes.Collections;
using Storage.Classes.Models.Omemo;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat
{
    public class ContactOmemoControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _Loading;
        public bool Loading
        {
            get => _Loading;
            set => SetProperty(ref _Loading, value);
        }

        private ChatDataTemplate _Chat;
        public ChatDataTemplate Chat
        {
            get => _Chat;
            set => SetChatProperty(value);
        }

        private bool _TrustedOnly;
        public bool TrustedOnly
        {
            get => _TrustedOnly;
            set => SetTrustedOnlyPropery(value);
        }

        private bool _NoFingerprintsFound;
        public bool NoFingerprintsFound
        {
            get => _NoFingerprintsFound;
            set => SetProperty(ref _NoFingerprintsFound, value);
        }

        public readonly CustomObservableCollection<OmemoFingerprintModel> FINGERPRINTS = new CustomObservableCollection<OmemoFingerprintModel>(true);

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
        private void SetTrustedOnlyPropery(bool value)
        {
            if (SetProperty(ref _TrustedOnly, value, nameof(TrustedOnly)) && !(Chat is null) && Chat.Chat.omemoInfo.trustedKeysOnly != value)
            {
                Chat.Chat.omemoInfo.trustedKeysOnly = value;
                Chat.Chat.omemoInfo.Update();
            }
        }

        private void SetChatProperty(ChatDataTemplate value)
        {
            if (SetProperty(ref _Chat, value, nameof(Chat)))
            {
                TrustedOnly = !(Chat is null) && Chat.Chat.omemoInfo.trustedKeysOnly;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
