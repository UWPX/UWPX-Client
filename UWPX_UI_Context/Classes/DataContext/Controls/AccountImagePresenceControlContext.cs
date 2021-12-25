using System.Threading.Tasks;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls;

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
        public async Task UpdateViewAsync(ChatType chatType, string bareJid, ImageModel image)
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
            MODEL.Image = image is null ? null : await image.GetSoftwareBitmapSourceAsync();
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
