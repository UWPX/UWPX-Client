using System.Threading.Tasks;
using Manager.Classes.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Pages;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public class ContactInfoPageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ContactInfoPageDataTemplate MODEL = new ContactInfoPageDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task OnChatChanged(ChatDataTemplate chat)
        {
            if (chat is not null)
            {
                await chat.Client.CheckForAvatarUpdatesAsync(chat.Chat.contactInfo, chat.Chat.bareJid, chat.Chat.bareJid);
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
