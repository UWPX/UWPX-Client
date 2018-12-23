using Data_Manager2.Classes;
using Logging;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace UWPX_UI_Context.Classes.DataContext
{
    public class ChatDetailsControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatDetailsControlDataTemplate MODEL = new ChatDetailsControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
            {
                return;
            }

            ChatDataTemplate newChat = null;
            if (args.OldValue is ChatDataTemplate oldChat)
            {
                oldChat.PropertyChanged -= OldChat_PropertyChanged;
            }

            if (args.NewValue is ChatDataTemplate)
            {
                newChat = args.NewValue as ChatDataTemplate;
                newChat.PropertyChanged += OldChat_PropertyChanged;
            }

            UpdateView(newChat);
        }

        public void SendChatMessage(ChatDataTemplate chat)
        {
            // Remove tailing and leading whitespaces, tabs and newlines:
            if (!string.IsNullOrWhiteSpace(MODEL.MessageText))
            {
                string trimed = MODEL.MessageText.Trim(UiUtils.TRIM_CHARS);
                if (MODEL.OmemoEnabled)
                {
                    // ToDo: Send encrypted
                    Logger.Debug("Send encrypted: " + trimed);
                }
                else
                {
                    // ToDo: Send unencrypted
                    Logger.Debug("Send unencrypted: " + trimed);
                }
                MODEL.MessageText = string.Empty;
            }
        }

        public void OnChatMessageKeyDown(KeyRoutedEventArgs args, ChatDataTemplate chat)
        {
            if (args.Key == VirtualKey.Enter)
            {
                if (!string.IsNullOrWhiteSpace(MODEL.MessageText))
                {
                    if (!UiUtils.IsVirtualKeyDown(VirtualKey.Shift) && Settings.getSettingBoolean(SettingsConsts.ENTER_TO_SEND_MESSAGES))
                    {
                        SendChatMessage(chat);
                        args.Handled = true;
                    }
                }
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(ChatDataTemplate chatTemplate)
        {
            if (chatTemplate is null)
            {

            }
            else
            {
                MODEL.UpdateViewClient(chatTemplate.Client);
                MODEL.UpdateViewChat(chatTemplate.Chat);
                MODEL.UpdateViewMuc(chatTemplate.Chat, chatTemplate.MucInfo);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OldChat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ChatDataTemplate chat)
            {
                UpdateView(chat);
            }
        }

        #endregion
    }
}
