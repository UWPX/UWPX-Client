using System.Linq;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
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
        public async Task ResetOmemoDevicesAsync(Client client)
        {
            MODEL.ResettingDevices = true;
            OmemoXmlDevices devices = new OmemoXmlDevices();
            devices.DEVICES.Add(new OmemoXmlDevice(client.dbAccount.omemoInfo.deviceId, client.dbAccount.omemoInfo.deviceLabel));
            await client.xmppClient.OMEMO_COMMAND_HELPER.setDeviceListAsync(devices);
            MODEL.ResettingDevices = false;
        }

        private void OnResetDeviceListResult(bool success)
        {
            MODEL.ResettingDevices = false;
        }

        public async Task RefreshOmemoDevicesAsync(Client client)
        {
            MODEL.RefreshingDevices = true;
            MessageResponseHelperResult<IQMessage> result = await client.xmppClient.OMEMO_COMMAND_HELPER.requestDeviceListAsync(client.dbAccount.bareJid);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is OmemoDeviceListResultMessage deviceListResultMessage)
                {
                    MODEL.DEVICES.Clear();
                    MODEL.DEVICES.AddRange(deviceListResultMessage.DEVICES.DEVICES.Select(x => new UintDataTemplate { Value = x.ID }));
                }
                else
                {
                    Logger.Warn("Failed to request device list (" + result.RESULT.ToString() + ").");
                }
            }
            else
            {
                Logger.Warn("Failed to request device list (" + result.STATE.ToString() + ").");
            }
            MODEL.RefreshingDevices = false;
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
