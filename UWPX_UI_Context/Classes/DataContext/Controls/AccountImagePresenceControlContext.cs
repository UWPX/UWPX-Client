using System.Threading.Tasks;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes.Network.XML.Messages.XEP_0392;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class AccountImagePresenceControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly AccountImagePresenceControlDataTemplate MODEL = new AccountImagePresenceControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task UpdateImageAsync(ImageModel image, string bareJid)
        {
            MODEL.Image = image is null ? null : await image.GetSoftwareBitmapSourceAsync();
        }

        public void UpdateBareJid(string bareJid)
        {
            Color color = MODEL.Image is null ? ConsistentColorGenerator.GenForegroundColor(bareJid ?? "", false, false) : Colors.Transparent;
            MODEL.Background = GenerateLinearGradientBrush(color);
        }

        public void UpdateChatType(ChatType chatType, string bareJid)
        {
            switch (chatType)
            {
                case ChatType.CHAT:
                    MODEL.Initials = string.IsNullOrEmpty(bareJid) ? "" : bareJid[0].ToString().ToUpperInvariant();
                    break;

                case ChatType.MUC:
                    MODEL.Initials = "\uE902";
                    break;

                case ChatType.IOT_DEVICE:
                    MODEL.Initials = "\uE957";
                    break;

                case ChatType.IOT_HUB:
                    MODEL.Initials = "\uF22C";
                    break;
            }


        }

        #endregion

        #region --Misc Methods (Private)--
        private LinearGradientBrush GenerateLinearGradientBrush(Color color)
        {
            LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection(), 90);

            brush.GradientStops.Add(new GradientStop
            {
                Color = ChangeColorBrightness(color, 0.5),
                Offset = 0
            });

            brush.GradientStops.Add(new GradientStop
            {
                Color = color,
                Offset = 1
            });
            return brush;
        }

        /// <summary>
        /// Creates color with corrected brightness.
        /// Source: https://gist.github.com/zihotki/09fc41d52981fb6f93a81ebf20b35cd5
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        public static Color ChangeColorBrightness(Color color, double correctionFactor)
        {
            double red = color.R;
            double green = color.G;
            double blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = ((255 - red) * correctionFactor) + red;
                green = ((255 - green) * correctionFactor) + green;
                blue = ((255 - blue) * correctionFactor) + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
