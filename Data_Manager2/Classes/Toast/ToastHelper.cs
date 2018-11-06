using Data_Manager2.Classes.DBTables;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

namespace Data_Manager2.Classes.Toast
{
    public static class ToastHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string DEFAULT_MUC_IMAGE_PATH = "Assets/Images/default_muc_image.png";
        private const string DEFAULT_USER_IMAGE_PATH = "Assets/Images/default_user_image.png";
        private const string SEND_BUTTON_IMAGE_PATH = "Assets/Images/send.png";
        public const string TEXT_BOX_ID = "msg_tbx";

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

        public static void showChatTextToast(ChatMessageTable msg, ChatTable chat)
        {
            var toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
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
                                Text = msg.message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo
                        {
                            Source = chat.chatType == ChatType.CHAT ? DEFAULT_USER_IMAGE_PATH : DEFAULT_MUC_IMAGE_PATH,
                            HintCrop = ToastGenericAppLogoCrop.Default
                        }
                    }
                },
                Actions = getActions(msg, chat),
                DisplayTimestamp = msg.date,
                Launch = new ChatToastActivation(chat.id, false).generate()
            };

            popToast(toastContent, chat);
        }

        public static void showChatTextEncryptedToast(ChatMessageTable msg, ChatTable chat)
        {
            var toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
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
                        AppLogoOverride = new ToastGenericAppLogo
                        {
                            Source = chat.chatType == ChatType.CHAT ? DEFAULT_USER_IMAGE_PATH : DEFAULT_MUC_IMAGE_PATH,
                            HintCrop = ToastGenericAppLogoCrop.Default
                        }
                    }
                },
                Actions = getActions(msg, chat),
                DisplayTimestamp = msg.date,
                Launch = new ChatToastActivation(chat.id, false).generate()
            };

            popToast(toastContent, chat);
        }

        public static void showChatTextImageToast(ChatMessageTable msg, ChatTable chat)
        {
            var toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = chat.chatJabberId,
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
                            HintCrop = ToastGenericAppLogoCrop.Default
                        }
                    }
                },
                Actions = getActions(msg, chat),
                DisplayTimestamp = msg.date,
                Launch = new ChatToastActivation(chat.id, false).generate()
            };

            popToast(toastContent, chat);
        }

        #endregion

        #region --Misc Methods (Private)--
        private static void popToast(ToastContent content, ChatTable chat)
        {
            var toastNotif = new ToastNotification(content.GetXml())
            {
                Group = chat.id
            };

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        private static ToastActionsCustom getActions(ChatMessageTable msg, ChatTable chat)
        {
            return new ToastActionsCustom
            {
                /*Inputs =
                {
                    new ToastTextBox(TEXT_BOX_ID)
                    {
                        PlaceholderContent = "Reply"
                    }
                },*/
                Buttons =
                {
                    /*new ToastButton("Send", new SendReplyToastActivation(chat.id, false).generate())
                    {
                        ActivationType = ToastActivationType.Background,
                        ImageUri = SEND_BUTTON_IMAGE_PATH,
                        TextBoxId = TEXT_BOX_ID,
                    },*/
                    new ToastButton("Mark chat as read", new MarkChatAsReadToastActivation(chat.id, false).generate())
                    {
                        ActivationType = ToastActivationType.Background
                    },
                    new ToastButton("Mark as read", new MarkMessageAsReadToastActivation(msg.id, false).generate())
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
