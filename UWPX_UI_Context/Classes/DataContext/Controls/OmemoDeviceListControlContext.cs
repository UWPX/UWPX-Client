using System.Linq;
using System.Threading.Tasks;
using Manager.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
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
        public async Task ResetDevicesAsync(Client client)
        {
            MODEL.Loading = true;
            MODEL.ResettingDevices = true;
            OmemoXmlDevices devices = new OmemoXmlDevices();
            devices.DEVICES.Add(new OmemoXmlDevice(client.dbAccount.omemoInfo.deviceId, client.dbAccount.omemoInfo.deviceLabel));
            await client.xmppClient.OMEMO_COMMAND_HELPER.setDeviceListAsync(devices);
            await client.xmppClient.getOmemoHelper().refreshDevicesAsync();
            LoadDevices(client);
            MODEL.ResettingDevices = false;
            MODEL.Loading = false;
        }

        public async Task RefreshDevicesAsync(Client client)
        {
            MODEL.Loading = true;
            MODEL.RefreshingDevices = true;
            await client.xmppClient.getOmemoHelper().refreshDevicesAsync();
            LoadDevices(client);
            MODEL.RefreshingDevices = false;
            MODEL.Loading = false;
        }

        public void LoadDevices(Client client)
        {
            MODEL.DEVICES.Clear();
            if (client is not null)
            {
                MODEL.DEVICES.AddRange(client.dbAccount.omemoInfo.devices.Where(d => d.deviceId != client.dbAccount.omemoInfo.deviceId));
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
