using Data_Manager2.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;

namespace UWPX_UI_Context.Classes.DataTemplates.Selectors
{
    public sealed class SpeechBubbleDataTemplateSelector : DataTemplateSelector
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DataTemplate SpeechBubbleTopTemplate { get; set; }
        public DataTemplate SpeechBubbleDownTemplate { get; set; }
        public DataTemplate SpeechBubbleErrorTemplate { get; set; }
        public DataTemplate SpeechBubbleInfoTemplate { get; set; }
        public DataTemplate SpeechBubbleHeadlineTemplate { get; set; }
        public DataTemplate SpeechBubbleMUCDirectInviteTemplate { get; set; }

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
            return base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ChatMessageDataTemplate model && !(model.Message is null))
            {
                switch (model.Message.type)
                {
                    case MessageMessage.TYPE_ERROR:
                        return SpeechBubbleErrorTemplate;

                    case MessageMessage.TYPE_GROUPCHAT:
                        if (string.Equals(model.MUC?.nickname, model.Message.fromUser))
                        {
                            return SpeechBubbleDownTemplate;
                        }
                        else
                        {
                            return SpeechBubbleTopTemplate;
                        }

                    case DirectMUCInvitationMessage.TYPE_MUC_DIRECT_INVITATION:
                        return SpeechBubbleMUCDirectInviteTemplate;

                    case MUCHandler.TYPE_CHAT_INFO:
                        return SpeechBubbleInfoTemplate;

                    default:
                        if (string.Equals(model.Chat.userAccountId, model.Message.fromUser))
                        {
                            return SpeechBubbleDownTemplate;
                        }
                        else
                        {
                            return SpeechBubbleTopTemplate;
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
