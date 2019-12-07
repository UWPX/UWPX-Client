using Data_Manager2.Classes.DBTables;
using UWPX_UI_Context.Classes.DataContext.Controls.Chat;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class MucMemberControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucMemberControlContext VIEW_MODEL = new MucMemberControlContext();

        public MUCOccupantTable Member
        {
            get { return (MUCOccupantTable)GetValue(MemberProperty); }
            set { SetValue(MemberProperty, value); }
        }
        public static readonly DependencyProperty MemberProperty = DependencyProperty.Register(nameof(Member), typeof(MUCOccupantTable), typeof(MucMemberControl), new PropertyMetadata(null, OnMemberChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucMemberControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UdpateView(MUCOccupantTable member)
        {
            if (!(member is null))
            {

            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnMemberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MucMemberControl control && e.NewValue is MUCOccupantTable member)
            {
                control.UdpateView(member);
            }
        }

        #endregion
    }
}
