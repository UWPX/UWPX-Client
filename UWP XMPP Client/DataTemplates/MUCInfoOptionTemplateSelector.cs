using Data_Manager2.Classes.DBTables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration;

namespace UWP_XMPP_Client.DataTemplates
{
    class MUCInfoOptionTemplateSelector : DataTemplateSelector
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DataTemplate optionTemplate { get; set; }
        public DataTemplate fieldTemplate { get; set; }
        public DataTemplate instructionsTemplate { get; set; }
        public DataTemplate titleTemplate { get; set; }

        private MUCChatInfoTable mucInfo;
        bool requestedMUCInfo;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCInfoOptionTemplateSelector()
        {
            this.mucInfo = null;
            this.requestedMUCInfo = false;
        }

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
            if (item is MUCInfoOptionTemplate)
            {
                MUCInfoOptionTemplate template = item as MUCInfoOptionTemplate;
                if (template.option is Option)
                {
                    return optionTemplate;
                }
                else if (template.option is Field)
                {
                    return fieldTemplate;
                }
                else if (template.option is Instructions)
                {
                    return instructionsTemplate;
                }
                else if (template.option is Title)
                {
                    return titleTemplate;
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
