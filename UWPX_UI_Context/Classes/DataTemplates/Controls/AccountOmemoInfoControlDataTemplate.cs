using Shared.Classes;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class AccountOmemoInfoControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private OmemoHelperState _OmemoState;
        public OmemoHelperState OmemoState
        {
            get => _OmemoState;
            set => SetProperty(ref _OmemoState, value);
        }
        private string _DeviceLabel;
        public string DeviceLabel
        {
            get => _DeviceLabel;
            set => SetProperty(ref _DeviceLabel, value);
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
