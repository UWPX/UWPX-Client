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
            MODEL.Background = new SolidColorBrush(color);
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
