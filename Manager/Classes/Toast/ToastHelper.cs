using System;
using System.Linq;
using Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using Shared.Classes;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Manager.Classes.Toast
{
    public class ToastHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string DEFAULT_MUC_IMAGE_PATH = "Assets/Images/default_muc_image.png";
        private const string DEFAULT_USER_IMAGE_PATH = "Assets/Images/default_user_image.png";
        private const string SEND_BUTTON_IMAGE_PATH = "Assets/Images/send.png";
        private const string SEND_BUTTON_ENCRYPTED_IMAGE_PATH = "Assets/Images/send_encrypted.png";
        public const string TEXT_BOX_ID = "msg_tbx";
        public const string WILL_BE_SEND_LATER_TOAST_GROUP = "will_be_send_later";

        private static readonly TimeSpan VIBRATE_TS = TimeSpan.FromMilliseconds(150);
        private static readonly TimeSpan VIBRATE_TIMEOUT_TS = TimeSpan.FromSeconds(3);
        private static DateTime lastVibration = DateTime.MinValue;

        private static readonly TimeSpan POP_TOAST_TIMEOUT_TS = TimeSpan.FromSeconds(5);
        private static DateTime lastPopToast = DateTime.MinValue;

        public delegate void OnChatMessageToastHandler(OnChatMessageToastEventArgs args);
        /// <summary>
        /// Called before the toast gets toasted.
        /// </summary>
        public static event OnChatMessageToastHandler OnChatMessageToast;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static void SetBadgeNumber(int i)
        {
            // Get the blank badge XML payload for a badge number
            XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

            // Set the value of the badge in the XML to our number
            XmlElement badgeElement = badgeXml.SelectSingleNode("/badge") as XmlElement;
            badgeElement.SetAttribute("value", i.ToString());

            // Create the badge notification
            BadgeNotification badge = new BadgeNotification(badgeXml);

            // Create the badge updater for the application
            BadgeUpdater badgeUpdater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();

            // And update the badge
            badgeUpdater.Update(badge);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static void RemoveToastGroup(string group)
        {
            ToastNotificationManager.History.RemoveGroup(group);
        }

        public static void ShowWillBeSendLaterToast(ChatModel chat)
        {
            ToastContent toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Your message will be send when the app is started again.",
                                HintMaxLines = 1
                            }
                        }
                    }
                },
                DisplayTimestamp = DateTime.Now,
            };

            RemoveToastGroup(WILL_BE_SEND_LATER_TOAST_GROUP);
            PopToast(toastContent, chat, WILL_BE_SEND_LATER_TOAST_GROUP);
        }

        public static void ShowChatTextToast(ChatMessageModel msg, ChatModel chat)
        {
            ToastContent toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = chat.bareJid,
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = msg.message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo
                        {
                            Source = chat.chatType == ChatType.CHAT ? DEFAULT_USER_IMAGE_PATH : DEFAULT_MUC_IMAGE_PATH,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                Actions = GetActions(msg, chat),
                DisplayTimestamp = msg.date,
                Launch = new ChatToastActivation(chat.id, msg.id).Generate()
            };

            PopToast(toastContent, chat);
        }

        public static void ShowChatTextImageToast(ChatMessageModel msg, ChatModel chat)
        {
            ToastContent toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = chat.bareJid,
                                HintMaxLines = 1
                            },
                            new AdaptiveText
                            {
                                Text = "You received an image!"
                            }
                        },
                        HeroImage = new ToastGenericHeroImage
                        {
                            Source = msg.message
                        },
                        AppLogoOverride = new ToastGenericAppLogo
                        {
                            Source = chat.chatType == ChatType.CHAT ? DEFAULT_USER_IMAGE_PATH : DEFAULT_MUC_IMAGE_PATH,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                Actions = GetActions(msg, chat),
                DisplayTimestamp = msg.date,
                Launch = new ChatToastActivation(chat.id, msg.id).Generate()
            };

            PopToast(toastContent, chat);
        }

        public static void UpdateBadgeNumber()
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                int count = ctx.ChatMessages.Where(m => m.state == MessageState.UNREAD).Join(ctx.Chats, m => m.chatId, c => c.id, (m, c) => new { m, c.muted }).Where(t => !t.muted).Count();
                SetBadgeNumber(count);
            }
        }

        public static void ShowSimpleToast(string text)
        {
            ToastContent toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = text
                            }
                        }
                    }
                },
            };

            ToastNotification toast = new ToastNotification(toastContent.GetXml());
            OnChatMessageToastEventArgs args = new OnChatMessageToastEventArgs(toast, null);
            OnChatMessageToast?.Invoke(args);

            PopToast(toast, args);
        }

        #endregion

        #region --Misc Methods (Private)--
        private static void PopToast(ToastContent content, ChatModel chat)
        {
            PopToast(content, chat, chat.id.ToString());
        }

        private static void PopToast(ToastContent content, ChatModel chat, string group)
        {
            ToastNotification toast = new ToastNotification(content.GetXml())
            {
                Group = group
            };

            OnChatMessageToastEventArgs args = new OnChatMessageToastEventArgs(toast, chat);
            OnChatMessageToast?.Invoke(args);

            PopToast(toast, args);
        }

        private static void PopToast(ToastNotification toast, OnChatMessageToastEventArgs args)
        {
            switch (args.toasterTypeOverride)
            {
                case ChatMessageToasterType.FULL:
                    // Make sure we only send actual on screen popups every 5 seconds:
                    toast.SuppressPopup = DateTime.Now.Subtract(lastPopToast).CompareTo(POP_TOAST_TIMEOUT_TS) < 0;
                    lastPopToast = DateTime.Now;

                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                    Logger.Debug("Toast for group: " + toast.Group + " toasted with toaster type: " + args.toasterTypeOverride.ToString());
                    break;

                case ChatMessageToasterType.REDUCED:
                    PopToastReduced();
                    Logger.Debug("Toast for group: " + toast.Group + " toasted with toaster type: " + args.toasterTypeOverride.ToString());
                    break;

                default:
                    Logger.Debug("Toast for group: " + toast.Group + " canceled.");
                    break;
            }
        }

        private static void PopToastReduced()
        {
            // Only vibrate or play sound once every VIBRATE_TIMEOUT_TS time span:
            if (DateTime.Now.Subtract(lastVibration).CompareTo(VIBRATE_TIMEOUT_TS) < 0)
            {
                return;
            }
            lastVibration = DateTime.Now;

            // Vibrate:
            if (DeviceFamilyHelper.SupportsVibration() && !Settings.GetSettingBoolean(SettingsConsts.DISABLE_VIBRATION_FOR_NEW_CHAT_MESSAGES))
            {
                SharedUtils.VibratePress(VIBRATE_TS);
            }

            // Play sound:
            if (!Settings.GetSettingBoolean(SettingsConsts.DISABLE_PLAY_SOUND_FOR_NEW_CHAT_MESSAGES))
            {
                SharedUtils.PlaySoundFromUri("ms-winsoundevent:Notification.Default");
            }
        }

        private static ToastActionsCustom GetActions(ChatMessageModel msg, ChatModel chat)
        {
            return new ToastActionsCustom
            {
                Inputs =
                {
                    new ToastTextBox(TEXT_BOX_ID)
                    {
                        PlaceholderContent = "Reply"
                    }
                },
                Buttons =
                {
                    new ToastButton("Send", new SendReplyToastActivation(chat.id, msg.id).Generate())
                    {
                        ActivationType = ToastActivationType.Background,
                        ImageUri = chat.omemo.enabled ? SEND_BUTTON_ENCRYPTED_IMAGE_PATH : SEND_BUTTON_IMAGE_PATH,
                        TextBoxId = TEXT_BOX_ID,
                    },
                    new ToastButton("Mark chat as read", new MarkChatAsReadToastActivation(chat.id, false).Generate())
                    {
                        ActivationType = ToastActivationType.Background
                    },
                    new ToastButton("Mark as read", new MarkMessageAsReadToastActivation(chat.id, msg.id).Generate())
                    {
                        ActivationType = ToastActivationType.Background
                    }
                }
            };
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
