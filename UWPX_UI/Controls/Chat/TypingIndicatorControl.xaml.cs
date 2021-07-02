using UWPX_UI_Context.Classes.DataContext.Controls.Chat;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class TypingIndicatorControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly TypingIndicatorControlContext VIEW_MODEL = new TypingIndicatorControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public TypingIndicatorControl()
        {
            InitializeComponent();
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


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TypingStoryboard.Begin();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            TypingStoryboard.Stop();
        }

        #endregion
    }
}
