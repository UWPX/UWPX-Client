using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext
{
    public sealed class ChatMasterControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatMasterControlDataTemplate MODEL;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMasterControlContext(ResourceDictionary resources)
        {
            MODEL = new ChatMasterControlDataTemplate(resources);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs args)
        {
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
