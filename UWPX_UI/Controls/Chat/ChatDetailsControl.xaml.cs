﻿using System;
using Logging;
using Manager.Classes;
using Manager.Classes.Chat;
using Shared.Classes.Image;
using Storage.Classes;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;
using Storage.Classes.Models.Omemo;
using UWPX_UI.Classes.Events;
using UWPX_UI.Controls.OMEMO;
using UWPX_UI.Pages;
using UWPX_UI.Pages.Settings;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.Events;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ChatDetailsControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(ChatDataTemplate), typeof(ChatDataTemplate), typeof(ChatDetailsControl), new PropertyMetadata(null, ChatPropertyChanged));

        public bool IsDummy
        {
            get => (bool)GetValue(IsDummyProperty);
            set => SetValue(IsDummyProperty, value);
        }
        public static readonly DependencyProperty IsDummyProperty = DependencyProperty.Register(nameof(IsDummy), typeof(bool), typeof(ChatDetailsControl), new PropertyMetadata(false, OnIsDummyChanged));

        public readonly ChatDetailsControlContext VIEW_MODEL = new ChatDetailsControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatDetailsControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            VIEW_MODEL.UpdateView(args);
        }

        public void OnPageNavigatedTo()
        {
            VIEW_MODEL.MODEL.LoadSettings();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadDummyContent()
        {
            JidModel jid = new JidModel
            {
                domainPart = "xmpp.uwpx.org",
                localPart = "alice",
                resourcePart = "phone"
            };

            ChatModel tmp = new ChatModel
            {
                accountBareJid = jid.BareJid(),
                bareJid = "dave@xmpp.uwpx.org",
                presence = XMPP_API.Classes.Presence.Away,
                status = "ʕノ•ᴥ•ʔノ ︵ ┻━┻",
                omemoInfo = new OmemoChatInformationModel("dave@xmpp.uwpx.org")
                {
                    enabled = true
                }
            };
            Client client = new Client(new AccountModel
            {
                bareJid = jid.BareJid(),
                fullJid = jid,
                enabled = true,
                server = new ServerModel(),
                omemoInfo = new OmemoAccountInformationModel()

            });
            Chat = new ChatDataTemplate(tmp, client);
            VIEW_MODEL.LoadDummyContent(Chat.Chat);
        }

        private void UpdateIsDummy()
        {
            VIEW_MODEL.OnIsDummyChanged(IsDummy);
            if (IsDummy)
            {
                LoadDummyContent();
            }
        }

        private void ShowEnterToSendTip()
        {
            if (VIEW_MODEL.MODEL.EnterToSend && !Storage.Classes.Settings.GetSettingBoolean(SettingsConsts.CHAT_ENTER_TO_SEND_TIP_SHOWN))
            {
                enterToSend_tt.IsOpen = true;
                Storage.Classes.Settings.SetSetting(SettingsConsts.CHAT_ENTER_TO_SEND_TIP_SHOWN, true);
            }
        }

        private void NavigateToChatInfoPage()
        {
            if (!IsDummy)
            {
                if (Chat.Chat.chatType == ChatType.MUC)
                {
                    UiUtils.NavigateToPage(typeof(MucInfoPage), new NavigatedToMucInfoPageEventArgs(Chat));
                }
                else
                {
                    UiUtils.NavigateToPage(typeof(ContactInfoPage), new NavigatedToContactInfoPageEventArgs(Chat));
                }
            }
        }

        private static T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent is null)
            {
                return null;
            }
            if (parent is T parentT)
            {
                return parentT;
            }
            return FindParent<T>(parent);
        }

        private EditImageControl GetEditImageControl()
        {
            ChatPage page = FindParent<ChatPage>(this);
            return page.GetEditImageControl();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void ChatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatDetailsControl detailsControl)
            {
                detailsControl.UpdateView(e);
            }
        }

        private void HeaderInfo_grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                headerInfo_grid.ContextFlyout.ShowAt(element);
            }
        }

        private void CopyNameText_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(VIEW_MODEL.MODEL.NameText);
        }

        private void CopyChatStatus_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(VIEW_MODEL.MODEL.StatusText);
        }

        private void CopyAccountText_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(VIEW_MODEL.MODEL.AccountText);
        }

        private void Info_mfo_Click(object sender, RoutedEventArgs e)
        {
            NavigateToChatInfoPage();
        }

        private async void Enter_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.EnterMucAsync(Chat);
        }

        private async void Leave_mfo_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.LeaveMucAsync(Chat);
        }

        private async void Test_mfo_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDummy)
            {
                await Chat.Client.CheckForAvatarUpdatesAsync(Chat.Chat.contactInfo, Chat.Chat.bareJid, Chat.Chat.bareJid);
            }
        }

        private void Send_btn_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.SendChatMessage(Chat);
            ShowEnterToSendTip();
        }

        private static void OnIsDummyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatDetailsControl chatDetailsControl)
            {
                chatDetailsControl.UpdateIsDummy();
            }
        }

        private void EmojiPickerControl_EmojiSelected(EmojiPickerControl sender, Classes.Events.EmojiSelectedEventArgs args)
        {
            string emoji = args.EMOJI.ToString();
            int i = message_tbx.SelectionStart;
            if (message_tbx.SelectionLength > 0)
            {
                message_tbx.SelectedText = emoji;
                message_tbx.SelectionLength = 0;
            }
            else
            {
                message_tbx.Text = message_tbx.Text.Insert(message_tbx.SelectionStart, emoji);
            }
            i += emoji.Length;
            message_tbx.SelectionStart = i;
        }

        private void MarkAsRead_tmfo_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDummy)
            {
                Chat.MarkAllAsRead();
            }

        }

        private void Message_tbx_EnterKeyDown(object sender, KeyRoutedEventArgs e)
        {
            VIEW_MODEL.OnEnterKeyDown(e, Chat);
        }

        private void Header_grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            chatMessages_cmg.ScrollHeaderMinSize = e.NewSize.Height;
        }

        private void ChatSettings_link_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.NavigateToPage(typeof(ChatSettingsPage));
        }

        private void MarkasIotDevice_mfo_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDummy)
            {
                VIEW_MODEL.MarkAsIotDevice(Chat.Chat);
            }
        }

        private void OnOmemoEnabledChanged(OmemoButtonControl sender, EventArgs e)
        {
            if (!IsDummy && sender.OmemoEnabled != Chat.Chat.omemoInfo.enabled)
            {
                Chat.Chat.omemoInfo.enabled = sender.OmemoEnabled;
                Chat.Chat.omemoInfo.Update();
                Logger.Info($"OMEMO for chat '{Chat.Chat.bareJid}' set to: '{sender.OmemoEnabled}'.");
            }
        }

        private void OnMessageBoxLostFocus(object sender, RoutedEventArgs e)
        {
            Chat.CHAT_STATE_HELPER.SetActive();
        }

        private void OnMessageBoxGotFocus(object sender, RoutedEventArgs e)
        {
            Chat.CHAT_STATE_HELPER.SetComposing();
        }

        private void OnMessageBoxKeyDown(object sender, KeyRoutedEventArgs e)
        {
            Chat.CHAT_STATE_HELPER.OnMessageBoxKeyDown();
        }

        private void OnHeaderTapped(object sender, TappedRoutedEventArgs e)
        {
            NavigateToChatInfoPage();
        }

        private async void OnSendImageFromLibraryClicked(object sender, RoutedEventArgs e)
        {
            if (IsDummy)
            {
                return;
            }

            StorageFile file = await ImageUtils.PickImageAsync();
            if (file is null)
            {
                Logger.Info("Sending image from library canceled.");
                return;
            }
            SoftwareBitmap img = await ImageUtils.LoadImageAsync(file);

            EditImageControl editImagecontrol = GetEditImageControl();
            editImagecontrol.SetImage(img);
            await editImagecontrol.ShowAsync();
        }

        private void OnImageEditDone(EditImageControl sender, ImageEditDoneEventArgs args)
        {
            if (args.SUCCESS)
            {
                sendFile_tbtn.IsChecked = false;
                VIEW_MODEL.SendImageChatMessage(Chat, args.IMAGE);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            GetEditImageControl().ImageEditDone -= OnImageEditDone;
            GetEditImageControl().ImageEditDone += OnImageEditDone;
        }
        #endregion
    }
}
