using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml.Media;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public class ImObservatoryBadgeControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ImObservatoryBadgeControlDataTemplate MODEL = new ImObservatoryBadgeControlDataTemplate();

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
            switch (rating.ToLowerInvariant())
            {
                case "a":
                    MODEL.BadgeBrush = new SolidColorBrush(UiUtils.HexStringToColor("#5cb85c"));
                    break;

                case "b":
                case "c":
                case "d":
                    MODEL.BadgeBrush = new SolidColorBrush(UiUtils.HexStringToColor("#f0ad4e"));
                    break;

                case "e":
                case "f":
                    MODEL.BadgeBrush = new SolidColorBrush(UiUtils.HexStringToColor("#d9534f"));
                    break;

                default:
                    MODEL.BadgeBrush = new SolidColorBrush(UiUtils.HexStringToColor("#818181"));
                    break;
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
