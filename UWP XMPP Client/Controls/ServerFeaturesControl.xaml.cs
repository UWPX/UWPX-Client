using Data_Manager2.Classes.DBTables;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class ServerFeaturesControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private XMPPClient client;
        private ChatTable chat;

        private string discoId;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/11/2017 Created [Fabian Sauter]
        /// </history>
        public ServerFeaturesControl()
        {
            this.discoId = null;
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void setClient(XMPPClient client)
        {
            this.client = client;
        }

        public void setChat(ChatTable chat)
        {
            this.chat = chat;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private async Task<int> sendDiscoToServerAsync()
        {

        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
