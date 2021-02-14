using System.Threading.Tasks;
using Manager.Classes;
using Manager.Classes.Chat;
using Storage.Classes.Models.Chat;
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
            await model.Device.WriteStringAsync(BTUtils.CHARACTERISTIC_JID_SENDER, model.Client.dbAccount.bareJid);
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

        private async Task AddIoTDevice(string deviceBareJid, Client client)
        {
            // Run it in a new Task since we want to access the DB and this is a blocking action.
            await Task.Run(async () =>
            {
                // Add to DB:
                ChatModel chat = new ChatModel(deviceBareJid, client.dbAccount)
                {
                    chatType = ChatType.IOT_DEVICE,
                    isChatActive = true
                };
                SemaLock semaLock = DataCache.INSTANCE.NewChatSemaLock();
                DataCache.INSTANCE.AddChatUnsafe(chat, client);
                semaLock.Dispose();

                // Add to the roster:
                await client.xmppClient.GENERAL_COMMAND_HELPER.addToRosterAsync(deviceBareJid);

                // Add as a PEP node:
                // TODO
            });
        }

        public async Task<bool> OnNewChatMessage(MessageMessage msg, string deviceFullJid, Client client)
        {
            string fromFullJid = msg.getFrom();
            string fromBareJid = Utils.getBareJidFromFullJid(fromFullJid);
            // Send the response:
            if (string.Equals(fromBareJid, deviceFullJid))
            {
                MessageMessage response = new MessageMessage(client.dbAccount.fullJid.FullJid(), fromBareJid, msg.MESSAGE, MessageMessage.TYPE_CHAT, true);
                await client.xmppClient.SendAsync(response);
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
