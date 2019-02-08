using Shared.Classes;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class AccountOmemoInfoControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private OmemoHelperState _OmemoState;
        public OmemoHelperState OmemoState
        {
            get { return _OmemoState; }
            set { SetProperty(ref _OmemoState, value); }
        }
        private uint _DeviceId;
        public uint DeviceId
        {
            get { return _DeviceId; }
            set { SetProperty(ref _DeviceId, value); }
        }
        private string _ErrorText;
        public string ErrorText
        {
            get { return _ErrorText; }
            set { SetProperty(ref _ErrorText, value); }
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
        public void UpdateView(AccountDataTemplate account)
        {
            if (account is null)
            {
                OmemoState = OmemoHelperState.DISABLED;
                DeviceId = 0;
                ErrorText = "";
            }
            else
            {
                OmemoState = account.Client.getOmemoHelper().STATE;
                DeviceId = account.Account.omemoDeviceId;

                if (!account.Account.checkOmemoKeys())
                {
                    ErrorText = "OMEMO keys are corrupted. Please remove and add your account again!";
                }
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
}
