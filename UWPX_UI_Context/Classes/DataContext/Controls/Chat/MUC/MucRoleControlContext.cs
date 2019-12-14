using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC;
using Windows.UI.Xaml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC
{
    public class MucRoleControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucRoleControlDataTemplate MODEL = new MucRoleControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is MUCRole role)
            {
                switch (role)
                {
                    case MUCRole.VISITOR:
                        MODEL.Glyph = "V";
                        MODEL.ToolTip = "Role: Visitor";
                        break;

                    case MUCRole.MODERATOR:
                        MODEL.Glyph = "M";
                        MODEL.ToolTip = "Role: Moderator";
                        break;

                    case MUCRole.PARTICIPANT:
                    case MUCRole.NONE:
                    default:
                        MODEL.Glyph = "";
                        MODEL.ToolTip = "";
                        break;
                }
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
