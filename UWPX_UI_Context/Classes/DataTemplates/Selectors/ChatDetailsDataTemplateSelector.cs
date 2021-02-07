using Manager.Classes.Chat;
using Storage.Classes.Models.Chat;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI_Context.Classes.DataTemplates.Selectors
{
    public sealed class ChatDetailsDataTemplateSelector: DataTemplateSelector
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DataTemplate ChatsTemplate { get; set; }
        public DataTemplate IotTemplate { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override DataTemplate SelectTemplateCore(object item)
        {
            return SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ChatDataTemplate chat)
            {
                switch (chat.Chat.chatType)
                {
                    case ChatType.IOT_DEVICE:
                    case ChatType.IOT_HUB:
                        return IotTemplate;

                    default:
                        return ChatsTemplate;
                }
            }
            return ChatsTemplate;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
