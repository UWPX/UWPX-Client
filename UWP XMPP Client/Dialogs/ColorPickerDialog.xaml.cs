using Logging;
using UWP_XMPP_Client.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class ColorPickerDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool canceled { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 29/10/2018 Created [Fabian Sauter]
        /// </history>
        public ColorPickerDialog(string hexColor)
        {
            this.canceled = true;
            this.InitializeComponent();

            if (UiUtils.isHexColor(hexColor))
            {
                color_cp.Color = UiUtils.convertHexStringToColor(hexColor);
            }
            else
            {
                Logger.Warn(nameof(ColorPickerDialog) + " failed to parse the given hex color - wrong format: " + hexColor);
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getHexColor()
        {
            return UiUtils.convertColorToHexString(color_cp.Color);
        }

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
        private void cancel_ibtn_Click(Controls.IconButtonControl sender, RoutedEventArgs args)
        {
            canceled = true;
            Hide();
        }

        private void accept_ibtn_Click(Controls.IconButtonControl sender, RoutedEventArgs args)
        {
            canceled = false;
            Hide();
        }

        private void randomColor_btn_Click(object sender, RoutedEventArgs e)
        {
            color_cp.Color = UiUtils.getRandomColor();
        }
        #endregion
    }
}
