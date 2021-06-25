using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml.Media;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public class ComplianceTesterBadgeControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ComplianceTesterBadgeControlDataTemplate MODEL = new ComplianceTesterBadgeControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(string rating)
        {
            // Badge color:
            if (int.TryParse(rating, out int r))
            {
                if (r > 90)
                {
                    MODEL.BadgeBrush = new SolidColorBrush(UiUtils.HexStringToColor("#5cb85c"));
                }
                else if (r > 50)
                {
                    MODEL.BadgeBrush = new SolidColorBrush(UiUtils.HexStringToColor("#f0ad4e"));
                }
                else
                {
                    MODEL.BadgeBrush = new SolidColorBrush(UiUtils.HexStringToColor("#d9534f"));
                }
                MODEL.RatingText = rating + '%';
            }
            else
            {
                MODEL.BadgeBrush = new SolidColorBrush(UiUtils.HexStringToColor("#818181"));
                MODEL.RatingText = "?";
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
