using Data_Manager2.Classes;
using Data_Manager2.Classes.DBTables;
using UWP_XMPP_Client.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;

namespace UWP_XMPP_Client.DataTemplates
{
    class ChatMessageDataTemplateSelector : DataTemplateSelector
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DataTemplate sendMessageTemplate { get; set; }
        public DataTemplate receivedMessageTemplate { get; set; }
        public DataTemplate errorMessageTemplate { get; set; }
        public DataTemplate mucDirectInvitationTemplate { get; set; }
        public DataTemplate mucOccupantKickedTemplate { get; set; }

        private MUCChatInfoTable mucInfo;
        bool requestedMUCInfo;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/01/2018 Created [Fabian Sauter]
        /// </history>
        public ChatMessageDataTemplateSelector()
        {
            this.mucInfo = null;
            this.requestedMUCInfo = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private UIElement getParentChatDetailsControl(UIElement element)
        {
            while (element != null && !(element is ChatDetailsControl))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
            return element;
        }

        private MUCChatInfoTable getMUCInfo(DependencyObject container)
        {
            UIElement element = getParentChatDetailsControl(container as UIElement);
            if (element != null)
            {
                ChatDetailsControl detailsControl = element as ChatDetailsControl;
                return detailsControl.MUCInfo;
            }
            return null;
        }

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
                        case MessageMessage.TYPE_ERROR:
                            return errorMessageTemplate;

                        case MessageMessage.TYPE_GROUPCHAT:
                            if (!requestedMUCInfo)
                            {
                                mucInfo = getMUCInfo(container);
                                requestedMUCInfo = true;
                            }

                            if (mucInfo != null && Equals(mucInfo.nickname, template.message.fromUser))
                            {
                                return sendMessageTemplate;
                            }
                            else
                            {
                                return receivedMessageTemplate;
                            }

                        case DirectMUCInvitationMessage.TYPE_MUC_DIRECT_INVITATION:
                            return mucDirectInvitationTemplate;

                        case MUCHandler.TYPE_MUC_OCCUPANT_KICKED:
                            return mucOccupantKickedTemplate;

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
