using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC;
using Windows.UI.Xaml;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC
{
    public class MucMemberControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucMemberControlDataTemplate MODEL = new MucMemberControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(MucMemberDataTemplate member)
        {
            MODEL.BareJid = Utils.getBareJidFromFullJid(member.Member.jid);
            MODEL.Nickname = member.Member.nickname;
            MODEL.Role = member.Member.role;
            MODEL.Affiliation = member.Member.affiliation;
            MODEL.ImageBareJid = string.IsNullOrEmpty(MODEL.BareJid) ? MODEL.Nickname : MODEL.BareJid;
            MODEL.YouVisible = string.Equals(member.Chat.Client.getXMPPAccount().getBareJid(), MODEL.BareJid) ? Visibility.Visible : Visibility.Collapsed;
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
