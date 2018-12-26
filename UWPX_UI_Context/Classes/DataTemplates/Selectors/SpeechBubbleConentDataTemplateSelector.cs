using UWPX_UI_Context.Classes.DataContext;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI_Context.Classes.DataTemplates.Selectors
{
    public sealed class SpeechBubbleConentDataTemplateSelector : DataTemplateSelector
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate ImageTemplate { get; set; }

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
            if (item is SpeechBubbleContentContext viewModel)
            {
                if (viewModel.MODEL.IsImage)
                {
                    return ImageTemplate;
                }
                return TextTemplate;
            }
            return base.SelectTemplateCore(item, container);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
