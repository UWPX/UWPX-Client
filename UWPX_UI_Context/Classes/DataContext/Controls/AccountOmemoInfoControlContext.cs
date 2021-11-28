using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using Storage.Classes.Models.Omemo;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

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

        public void SaveDeviceLabel(string deviceLabel, Client client)
        {
            OmemoAccountInformationModel omemoInfo = client.dbAccount.omemoInfo;
            deviceLabel = deviceLabel.Trim();

            string newLabel;
            if (string.IsNullOrEmpty(deviceLabel) || string.Equals(deviceLabel, omemoInfo.deviceId.ToString()))
            {
                newLabel = null;
            }
            else
            {
                newLabel = deviceLabel;
            }

            // Prevent unnecessary updates in case for example the control loaded:
            if (string.Equals(newLabel, omemoInfo.deviceLabel))
            {
                return;
            }

            MODEL.Saving = true;
            Task.Run(async () =>
            {
                try
                {
                    if (client.xmppClient.isConnected())
                    {
                        MessageResponseHelperResult<IQMessage> result = await client.xmppClient.OMEMO_COMMAND_HELPER.requestDeviceListAsync(client.dbAccount.bareJid);
                        if (result.STATE == MessageResponseHelperResultState.SUCCESS)
                        {
                            if (result.RESULT is OmemoDeviceListResultMessage deviceListResultMessage)
                            {
                                OmemoXmlDevice device = deviceListResultMessage.DEVICES.DEVICES.Where(d => d.ID == omemoInfo.deviceId).FirstOrDefault();
                                if (device is null)
                                {
                                    device = new OmemoXmlDevice(omemoInfo.deviceId, deviceLabel);
                                    deviceListResultMessage.DEVICES.DEVICES.Add(device);
                                }
                                else if (string.Equals(device.label, deviceLabel))
                                {
                                    MODEL.ErrorSaving = false;
                                    MODEL.Saving = false;
                                    Logger.Info("No need to update devices. Label already the same.");
                                    return;
                                }
                                else
                                {
                                    device.label = newLabel;
                                    result = await client.xmppClient.OMEMO_COMMAND_HELPER.setDeviceListAsync(deviceListResultMessage.DEVICES);
                                    if (result.STATE != MessageResponseHelperResultState.SUCCESS)
                                    {
                                        Logger.Error("Failed to set device list (" + result.STATE + ").");
                                    }
                                    else if (result.RESULT is IQErrorMessage errorMessage)
                                    {
                                        Logger.Error("Failed to set device list (" + errorMessage.ERROR_OBJ.ToString() + ").");
                                    }
                                    else
                                    {
                                        MODEL.ErrorSaving = false;
                                        omemoInfo.deviceLabel = newLabel;
                                        omemoInfo.Update();
                                        MODEL.Saving = false;
                                        Logger.Info($"Device label for device {omemoInfo.deviceId} successfully updated to: '{newLabel}'.");
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                Logger.Error("Failed to request device list (" + result.RESULT.ToString() + ").");
                            }
                        }
                        else
                        {
                            Logger.Error("Failed to request device list (" + result.STATE.ToString() + ").");
                        }
                    }
                    else
                    {
                        Logger.Error("Failed to update device label. Client not connected");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to update device label.", e);
                }
                MODEL.Saving = false;
                MODEL.ErrorSaving = true;
            });
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(OmemoAccountInformationModel omemoInfo)
        {
            if (omemoInfo is not null)
            {
                MODEL.DeviceLabel = string.IsNullOrEmpty(omemoInfo.deviceLabel) ? (omemoInfo.deviceId == 0 ? "-" : omemoInfo.deviceId.ToString()) : omemoInfo.deviceLabel;
            }
        }

        public void UpdateView(AccountDataTemplate account)
        {
            if (account is not null)
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
