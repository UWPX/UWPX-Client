using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPX_UI.Controls.Chat
{
    internal class EnterTextBox: TextBox
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        //
        // Summary:
        //     Occurs when a keyboard enter key is pressed while the UIElement has focus.
        public event KeyEventHandler EnterKeyDown;

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


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                EnterKeyDown?.Invoke(this, e);
                if (e.Handled)
                {
                    return;
                }
            }

            base.OnKeyDown(e);
        }

        #endregion
    }
}
