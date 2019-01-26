using Shared.Classes;
using Windows.UI.Xaml;
using XMPP_API.Classes.Network.XML;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class AccountInfoGeneralControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private MessageParserStats _ParserStats;
        public MessageParserStats ParserStats
        {
            get { return _ParserStats; }
            set { SetProperty(ref _ParserStats, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateViewModel(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is AccountDataTemplate account)
            {
                ParserStats = account.Client.getMessageParserStats();
            }
            else
            {
                ParserStats = null;
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
