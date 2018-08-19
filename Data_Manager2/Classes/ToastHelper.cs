using Data_Manager2.Classes.DBTables;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel;
using Windows.UI.Notifications;

namespace Data_Manager2.Classes
{
    public class ToastHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string DEFAULT_MUC_IMAGE_PATH = "Assets/Images/default_muc_image.png";
        private const string DEFAULT_USER_IMAGE_PATH = "Assets/Images/default_user_image.png";
        private const string SEND_BUTTON_IMAGE_PATH = "Assets/Images/send.png";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static void removeToastGroup(string group)
        {
            ToastNotificationManager.History.RemoveGroup(group);
        }

        public static void showChatTextToast(string text, string msgId, ChatTable chat)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = chat.chatJabberId,
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = text
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = chat.chatType == ChatType.CHAT ? DEFAULT_USER_IMAGE_PATH : DEFAULT_MUC_IMAGE_PATH,
                            HintCrop = ToastGenericAppLogoCrop.Default
                        }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastTextBox("textBox")
                        {
                            PlaceholderContent = "Reply"
                        }
                    },
                    Buttons =
                    {
                        new ToastButton("Send", chat.id)
                        {
                            ActivationType = ToastActivationType.Background,
                            ImageUri = SEND_BUTTON_IMAGE_PATH,
                            TextBoxId = "textBox",
                        }
                    }
                },
                Launch = "CHAT=" + chat.id
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml())
            {
                Group = chat.id,
                Tag = msgId
            };

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static void showChatTextEncryptedToast(string text, string msgId, ChatTable chat)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = chat.chatJabberId,
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = "You received an encrypted message!"
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = chat.chatType == ChatType.CHAT ? DEFAULT_USER_IMAGE_PATH : DEFAULT_MUC_IMAGE_PATH,
                            HintCrop = ToastGenericAppLogoCrop.Default
                        }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastTextBox("textBox")
                        {
                            PlaceholderContent = "Reply"
                        }
                    },
                    Buttons =
                    {
                        new ToastButton("Send", chat.id)
                        {
                            ActivationType = ToastActivationType.Background,
                            ImageUri = SEND_BUTTON_IMAGE_PATH,
                            TextBoxId = "textBox",
                        }
                    }
                },
                Launch = "CHAT=" + chat.id
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml())
            {
                Group = chat.id,
                Tag = msgId
            };

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static void showChatTextImageToast(string text, string imgPath, ChatTable chat)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = chat.chatJabberId,
                                HintMaxLines = 1
                            }
                        },
                        HeroImage = new ToastGenericHeroImage()
                        {
                            Source = text
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = chat.chatType == ChatType.CHAT ? DEFAULT_USER_IMAGE_PATH : DEFAULT_MUC_IMAGE_PATH,
                            HintCrop = ToastGenericAppLogoCrop.Default
                        }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastTextBox("textBox")
                        {
                            PlaceholderContent = "Reply"
                        }
                    },
                    Buttons =
                    {
                        new ToastButton("Send", chat.id)
                        {
                            ActivationType = ToastActivationType.Background,
                            ImageUri = SEND_BUTTON_IMAGE_PATH,
                            TextBoxId = "textBox",
                        }
                    }
                },
                Launch = "CHAT=" + chat.id
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
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
