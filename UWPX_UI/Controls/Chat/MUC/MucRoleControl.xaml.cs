using UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace UWPX_UI.Controls.Chat.MUC
{
    public sealed partial class MucRoleControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucRoleControlContext VIEW_MODEL = new MucRoleControlContext();

        public MUCRole Role
        {
            get { return (MUCRole)GetValue(RoleProperty); }
            set { SetValue(RoleProperty, value); }
        }
        public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(nameof(Role), typeof(MUCRole), typeof(MucRoleControl), new PropertyMetadata(MUCRole.NONE, OnRoleChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucRoleControl()
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
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            VIEW_MODEL.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnRoleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MucRoleControl control)
            {
                control.UpdateView(e);
            }
        }

        #endregion
    }
}
