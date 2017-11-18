namespace Data_Manager2.Classes
{
    public class ToastHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        // https://blogs.msdn.microsoft.com/tiles_and_toasts/2015/07/08/quickstart-sending-a-local-toast-notification-and-handling-activations-from-it-windows-10/
        // https://github.com/WindowsNotifications/quickstart-sending-local-toast-win10/tree/master/Quickstart-Sending-Local-Toast

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static void showChatTextToast(string text, string msgId, ChatEntry chat)
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
                                Text = chat.name ?? chat.chatJabberId,
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = text
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = "Assets/Images/default_user_image.png",
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
                            ImageUri = "Assets/Images/send.png",
                            TextBoxId = "textBox",
                        }
                    }
                },
                Launch = chat.id
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());
            toastNotif.Group = chat.id;
            toastNotif.Tag = msgId;

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static void showChatTextImageToast(string text, string imgPath, ChatEntry chat)
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
                                Text = chat.name ?? chat.chatJabberId,
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = text
                            }
                        },
                        HeroImage = new ToastGenericHeroImage()
                        {
                            Source = imgPath
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = "Assets/Images/default_user_image.png",
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
                        new ToastButton("Send", "action=reply&threadId=92187")
                        {
                            ActivationType = ToastActivationType.Background,
                            ImageUri = "Assets/Images/send.png",
                            TextBoxId = "textBox",
                        }
                    }
                },
                Launch = "action=openThread&threadId=92187"
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
