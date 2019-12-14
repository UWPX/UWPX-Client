using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC;
using Windows.UI.Xaml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC
{
    public class MucAffiliationControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucAffiliationControlDataTemplate MODEL = new MucAffiliationControlDataTemplate();

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
            if (e.NewValue is MUCAffiliation affiliation)
            {
                switch (affiliation)
                {
                    case MUCAffiliation.OUTCAST:
                        break;

                    case MUCAffiliation.MEMBER:
                        MODEL.ToolTip = "Affiliation: Member";
                        MODEL.Glyph = "\u2659";
                        break;

                    case MUCAffiliation.ADMIN:
                        MODEL.ToolTip = "Affiliation: Admin";
                        MODEL.Glyph = "\u2656";
                        break;

                    case MUCAffiliation.OWNER:
                        MODEL.ToolTip = "Affiliation: Owner";
                        MODEL.Glyph = "\u2655";
                        break;

                    case MUCAffiliation.NONE:
                    default:
                        MODEL.ToolTip = "";
                        MODEL.Glyph = "";
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
