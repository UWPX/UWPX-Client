using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPX_UI.Extensions
{
    public static class ScrollViewerExtensions
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static void ScrollIntoViewVertically(this ScrollViewer scrollViewer, UIElement element, bool disableAnimation)
        {
            GeneralTransform visual = element.TransformToVisual((UIElement)scrollViewer.Content);
            Point pos = visual.TransformPoint(new Point(0, 0));
            scrollViewer.ChangeView(null, pos.Y, null, disableAnimation);
        }

        public static void ScrollIntoViewHorizontally(this ScrollViewer scrollViewer, UIElement element, bool disableAnimation)
        {
            GeneralTransform visual = element.TransformToVisual((UIElement)scrollViewer.Content);
            Point pos = visual.TransformPoint(new Point(0, 0));
            scrollViewer.ChangeView(pos.X, null, null, disableAnimation);
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
