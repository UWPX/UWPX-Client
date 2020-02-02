using UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat.MUC
{
    public sealed partial class MucMemberControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucMemberControlContext VIEW_MODEL = new MucMemberControlContext();

        public MucMemberDataTemplate Member
        {
            get => (MucMemberDataTemplate)GetValue(MemberProperty);
            set => SetValue(MemberProperty, value);
        }
        public static readonly DependencyProperty MemberProperty = DependencyProperty.Register(nameof(Member), typeof(MucMemberDataTemplate), typeof(MucMemberControl), new PropertyMetadata(null, OnMemberChanged));

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
        private void UdpateView(MucMemberDataTemplate member)
        {
            if (!(member is null))
            {
                VIEW_MODEL.UpdateView(member);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnMemberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MucMemberControl control && e.NewValue is MucMemberDataTemplate member)
            {
                control.UdpateView(member);
            }
        }

        #endregion
    }
}
