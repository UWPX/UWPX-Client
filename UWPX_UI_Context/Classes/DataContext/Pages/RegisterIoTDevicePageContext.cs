using System.Threading.Tasks;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API_IoT.Classes.Bluetooth;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public class RegisterIoTDevicePageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly RegisterIoTDevicePageDataTemplate MODEL = new RegisterIoTDevicePageDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task SendAsync(BluetoothDeviceInfoControlDataTemplate model)
        {
            await model.Device.WriteStringAsync(BTUtils.CHARACTERISTIC_JID, model.Jid);
            await model.Device.WriteStringAsync(BTUtils.CHARACTERISTIC_JID_PASSWORD, model.JidPassword);
            await model.Device.WriteStringAsync(BTUtils.CHARACTERISTIC_JID_SENDER, model.Client.getXMPPAccount().getBareJid());
            await model.Device.WriteStringAsync(BTUtils.CHARACTERISTIC_WIFI_SSID, model.WifiSsid);
            await model.Device.WriteStringAsync(BTUtils.CHARACTERISTIC_WIFI_PASSWORD, model.WifiPassword ?? "");

            // Inform the device, that all settings have been written to it:
            // This might fail if the device reboots instantly:
            try
            {
                await model.Device.WriteBytesAsync(BTUtils.CHARACTERISTIC_SETTINGS_DONE, new byte[] { 1 });
            }
            catch (System.Exception) { }
        }

        private async Task AddIoTDevice(string deviceBareJid, XMPPClient client)
        {
            // Run it in a new Task since we want to access the DB and this is a blocking action.
            await Task.Run(async () =>
            {
                // Add to DB:
                ChatTable chat = new ChatTable(deviceBareJid, client.getXMPPAccount().getBareJid())
                {
                    chatType = ChatType.IOT_DEVICE,
                    isChatActive = true
                };
                ChatDBManager.INSTANCE.setChat(chat, false, true);

                // Add to the roster:
                await client.GENERAL_COMMAND_HELPER.addToRosterAsync(deviceBareJid);

                // Add as a PEP node:
                // TODO
            });
        }

        public async Task<bool> OnNewChatMessage(MessageMessage msg, string deviceFullJid, XMPPClient client)
        {
            string fromFullJid = msg.getFrom();
            string fromBareJid = Utils.getBareJidFromFullJid(fromFullJid);
            // Send the response:
            if (string.Equals(fromBareJid, deviceFullJid))
            {
                MessageMessage response = new MessageMessage(client.getXMPPAccount().getFullJid(), fromBareJid, msg.MESSAGE, MessageMessage.TYPE_CHAT, true);
                await client.sendAsync(response);
                await AddIoTDevice(fromBareJid, client);
                return true;
            }
            return false;
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
