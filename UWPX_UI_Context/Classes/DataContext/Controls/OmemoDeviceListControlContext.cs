using System.Linq;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class OmemoDeviceListControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoDeviceListControlDataTemplate MODEL = new OmemoDeviceListControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void ResetOmemoDevices(XMPPClient client)
        {
            OmemoHelper helper = client.getOmemoHelper();
            if (!MODEL.ResettingDevices && !(helper is null))
            {
                MODEL.ResettingDevices = true;
                helper.resetDeviceListStateless(OnResetDeviceListResult);
            }
        }

        private void OnResetDeviceListResult(bool success)
        {
            MODEL.ResettingDevices = false;
        }

        public void RefreshOmemoDevices(XMPPClient client)
        {
            OmemoHelper helper = client.getOmemoHelper();
            if (!MODEL.RefreshingDevices && !(helper is null))
            {
                helper.requestDeviceListStateless(OnRequestDeviceListResult);
            }
        }

        private void OnRequestDeviceListResult(bool success, OmemoDevices devices)
        {
            if (success)
            {
                MODEL.DEVICES.Clear();
                MODEL.DEVICES.AddRange(devices.IDS.Select(x => new UintDataTemplate { Value = x }));
            }
        }

        public void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is AccountDataTemplate account)
            {
                MODEL.UpdateView(account);
            }
            else
            {
                MODEL.UpdateView(null);
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
