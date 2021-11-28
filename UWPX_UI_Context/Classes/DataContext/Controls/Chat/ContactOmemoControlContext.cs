﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Chat;
using Omemo.Classes;
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

                if (chat is not null)
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
            fingerprint.Update();
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
                        OmemoDeviceModel device = MODEL.Chat.Chat.omemoInfo.devices.Where(d => d.deviceId == fingerprintUriAction.FINGERPRINT.ADDRESS.DEVICE_ID).FirstOrDefault();
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
                                MODEL.Chat.Chat.omemoInfo.devices.Add(device);
                                ctx.Update(MODEL.Chat.Chat.omemoInfo);
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

        public void LoadFingerprints()
        {
            if (MODEL.Loading)
            {
                return;
            }
            Task.Run(() =>
            {
                MODEL.Loading = true;
                List<OmemoFingerprintDataTemplate> fingerprints = MODEL.Chat.Chat.omemoInfo.devices.Where(d => d.session is not null && d.fingerprint is not null).Select(d => new OmemoFingerprintDataTemplate { State = d.session.state, Fingerprint = d.fingerprint }).ToList();
                // Sort based on the last seen date. If it's the same prefer trusted ones:
                fingerprints.Sort((x, y) =>
                {
                    int dateComp = y.Fingerprint.lastSeen.CompareTo(x.Fingerprint.lastSeen);
                    if (dateComp == 0)
                    {
                        if (x.Fingerprint.trusted == y.Fingerprint.trusted)
                        {
                            return 0;
                        }
                        else if (y.Fingerprint.trusted)
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

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
