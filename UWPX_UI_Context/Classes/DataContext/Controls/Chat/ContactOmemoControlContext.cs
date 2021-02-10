using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Chat;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Omemo;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using Windows.UI.Xaml;
using XMPP_API.Classes.XmppUri;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat
{
    public class ContactOmemoControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ContactOmemoControlDataTemplate MODEL = new ContactOmemoControlDataTemplate();

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
            if (e.NewValue is ChatDataTemplate chat)
            {
                MODEL.Chat = chat;

                if (!(chat is null))
                {
                    LoadFingerprints();
                }
            }
        }

        public void OnFingerprintTrustedChanged(OmemoFingerprintModel fingerprint)
        {
            if (fingerprint.trusted)
            {
                MODEL.TrustedOnly = true;
            }
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.Update(fingerprint);
            }
        }

        public void OnQrCodeScannerShown(QrCodeScannerDialogDataTemplate model)
        {
            if (model.Success)
            {
                Uri uri = null;
                try
                {
                    uri = new Uri(model.QrCode);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to parse OMEMO fingerprint XMPP URI. Malformed URI: " + model.QrCode, e);
                    return;
                }

                if (string.Equals(uri.LocalPath.ToLowerInvariant(), MODEL.Chat.Chat.bareJid.ToLowerInvariant()))
                {
                    IUriAction action = UriUtils.parse(uri);

                    if (action is OmemoFingerprintUriAction fingerprintUriAction)
                    {
                        OmemoDeviceModel device = MODEL.Chat.Chat.omemo.devices.Where(d => d.deviceId == fingerprintUriAction.FINGERPRINT.ADDRESS.DEVICE_ID).FirstOrDefault();
                        using (MainDbContext ctx = new MainDbContext())
                        {
                            if (device is null)
                            {
                                device = new OmemoDeviceModel(fingerprintUriAction.FINGERPRINT.ADDRESS)
                                {
                                    fingerprint = new OmemoFingerprintModel(fingerprintUriAction.FINGERPRINT)
                                    {
                                        trusted = true
                                    }
                                };
                                ctx.Add(device);
                                MODEL.Chat.Chat.omemo.devices.Add(device);
                                ctx.Update(MODEL.Chat.Chat.omemo);
                            }
                            else
                            {
                                device.fingerprint.FromOmemoFingerprint(fingerprintUriAction.FINGERPRINT);
                                device.fingerprint.trusted = true;
                                ctx.Update(device.fingerprint.identityKey);
                                ctx.Update(device.fingerprint);
                            }
                        }

                        Logger.Info("Scanned OMEMO fingerprint successful.");
                        Logger.Debug("Fingerprint: " + fingerprintUriAction.FINGERPRINT.ADDRESS.ToString());
                        LoadFingerprints();
                    }
                    else
                    {
                        Logger.Warn("Failed to parse OMEMO fingerprint XMPP URI. Not an " + nameof(OmemoFingerprintUriAction) + ".");
                    }
                }
                else
                {
                    Logger.Warn("Failed to parse OMEMO fingerprint XMPP URI. Wrong chat: " + uri.LocalPath);
                }
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadFingerprints()
        {
            if (MODEL.Loading)
            {
                return;
            }
            Task.Run(() =>
            {
                MODEL.Loading = true;
                List<OmemoFingerprintModel> fingerprints = MODEL.Chat.Chat.omemo.devices.Select(d => d.fingerprint).ToList();
                // Sort based on the last seen date. If it's the same prefer trusted ones:
                fingerprints.Sort((x, y) =>
                {
                    int dateComp = y.lastSeen.CompareTo(x.lastSeen);
                    if (dateComp == 0)
                    {
                        if (x.trusted == y.trusted)
                        {
                            return 0;
                        }
                        else if (y.trusted)
                        {
                            return 1;
                        }
                        return -1;
                    }
                    return dateComp;
                });
                MODEL.FINGERPRINTS.Clear();
                MODEL.FINGERPRINTS.AddRange(fingerprints);
                MODEL.NoFingerprintsFound = fingerprints.Count <= 0;
                MODEL.Loading = false;
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
