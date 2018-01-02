using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.DataTemplates
{
    class ChatMessageDataTemplateSelector : DataTemplateSelector
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DataTemplate sendMessageTemplate { get; set; }
        public DataTemplate receivedMessageTemplate { get; set; }
        public DataTemplate errorMessageTemplate { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/01/2018 Created [Fabian Sauter]
        /// </history>


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
            return base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ChatMessageDataTemplate)
            {
                ChatMessageDataTemplate template = item as ChatMessageDataTemplate;
                if (template.message != null && template.chat != null)
                {
                    switch (template.message.type)
                    {
                        case "error":
                            return errorMessageTemplate;
                        default:
                            if (template.chat.userAccountId.Equals(template.message.fromUser))
                            {
                                return sendMessageTemplate;
                            }
                            else
                            {
                                return receivedMessageTemplate;
                            }
                    }
                }
            }
            return base.SelectTemplateCore(item, container);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
