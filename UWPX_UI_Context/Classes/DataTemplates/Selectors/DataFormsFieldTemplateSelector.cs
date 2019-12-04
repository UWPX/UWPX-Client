using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace UWPX_UI_Context.Classes.DataTemplates.Selectors
{
    public sealed class DataFormsFieldTemplateSelector: DataTemplateSelector
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DataTemplate TextSingleFieldTemplate { get; set; }
        public DataTemplate TextMultiFieldTemplate { get; set; }
        public DataTemplate TextPrivateFieldTemplate { get; set; }
        public DataTemplate FixedFieldTemplate { get; set; }
        public DataTemplate BooleanFieldTemplate { get; set; }
        public DataTemplate ListSingleFieldTemplate { get; set; }
        public DataTemplate ListMultiFieldTemplate { get; set; }
        public DataTemplate HiddenFieldTemplate { get; set; }

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
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is Field field)
            {
                switch (field.type)
                {
                    case FieldType.HIDDEN:
                        return HiddenFieldTemplate;

                    case FieldType.TEXT_SINGLE:
                        return TextSingleFieldTemplate;

                    case FieldType.TEXT_MULTI:
                        return TextMultiFieldTemplate;

                    case FieldType.TEXT_PRIVATE:
                        return TextPrivateFieldTemplate;

                    case FieldType.FIXED:
                        return FixedFieldTemplate;

                    case FieldType.BOOLEAN:
                        return BooleanFieldTemplate;

                    case FieldType.LIST_SINGLE:
                        return ListSingleFieldTemplate;

                    case FieldType.LIST_MULTI:
                        return ListMultiFieldTemplate;

                    case FieldType.NONE:
                    default:
                        return base.SelectTemplateCore(item, container);
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
