using System.ComponentModel;
using Storage.Classes.Models.Omemo;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class AccountOmemoInfoControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AccountOmemoInfoControlDataTemplate MODEL = new AccountOmemoInfoControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is AccountDataTemplate newAccount)
            {
                newAccount.Client.dbAccount.omemoInfo.PropertyChanged += OnOmemoInfoPropertyChanged;
                UpdateView(newAccount);
                UpdateView(newAccount.Client.dbAccount.omemoInfo);
            }
        }

        public void SaveDeviceLabel(string deviceLabel, OmemoAccountInformationModel omemoInfo)
        {
            if (string.IsNullOrEmpty(deviceLabel) || string.Equals(deviceLabel, omemoInfo.deviceLabel))
            {
                omemoInfo.deviceLabel = null;
            }
            else
            {
                omemoInfo.deviceLabel = deviceLabel;
            }
            omemoInfo.Update();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(OmemoAccountInformationModel omemoInfo)
        {
            if (!(omemoInfo is null))
            {
                MODEL.DeviceLabel = string.IsNullOrEmpty(omemoInfo.deviceLabel) ? (omemoInfo.deviceId == 0 ? "-" : omemoInfo.deviceId.ToString()) : omemoInfo.deviceLabel;
            }
        }

        public void UpdateView(AccountDataTemplate account)
        {
            if (!(account is null))
            {
                MODEL.OmemoState = account.Client.xmppClient.getOmemoHelper()?.STATE ?? OmemoHelperState.DISABLED;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnOmemoInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is OmemoAccountInformationModel omemoInfo)
            {
                UpdateView(omemoInfo);
            }
        }

        #endregion
    }
}
