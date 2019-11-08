using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
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
            await model.Device.WriteStringAsync(BTUtils.CHARACTERISTIC_JID_SENDER, model.Jid);
            await model.Device.WriteStringAsync(BTUtils.CHARACTERISTIC_WIFI_SSID, model.WifiSsid);
            await model.Device.WriteStringAsync(BTUtils.CHARACTERISTIC_WIFI_PASSWORD, model.WifiPassword ?? "");

            // Inform the device, that all settings have been written to it:
            await model.Device.WriteBytesAsync(BTUtils.CHARACTERISTIC_SETTINGS_DONE, new byte[] { 1 });
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
