using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class MUCMembersControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(chatProperty); }
            set
            {
                SetValue(chatProperty, value);
                loadMembers();
            }
        }
        public static readonly DependencyProperty chatProperty = DependencyProperty.Register("Chat", typeof(ChatTable), typeof(MUCMembersControl), null);

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(clientProperty); }
            set { SetValue(clientProperty, value); }
        }
        public static readonly DependencyProperty clientProperty = DependencyProperty.Register("Client", typeof(XMPPClient), typeof(MUCMembersControl), null);

        private ObservableCollection<MUCMemberTemplate> members;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCMembersControl()
        {
            this.members = new ObservableCollection<MUCMemberTemplate>();
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void loadMembers()
        {
            if (Chat != null)
            {
                foreach (MUCMemberTable m in ChatManager.INSTANCE.getAllMUCMembers(Chat.id))
                {
                    members.Add(new MUCMemberTemplate() { member = m });
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            remove_btn.IsEnabled = members_lstv.SelectedItems.Count > 0;
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void remove_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
