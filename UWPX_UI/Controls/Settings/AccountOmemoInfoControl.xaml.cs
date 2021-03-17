using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPX_UI.Controls.Settings
{
    public sealed partial class AccountOmemoInfoControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public AccountDataTemplate Account
        {
            get => (AccountDataTemplate)GetValue(AccountProperty);
            set => SetValue(AccountProperty, value);
        }
        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(AccountDataTemplate), typeof(AccountOmemoInfoControl), new PropertyMetadata(null, OnAccountChanged));

        public readonly AccountOmemoInfoControlContext VIEW_MODEL = new AccountOmemoInfoControlContext();
        private bool editingLabel = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountOmemoInfoControl()
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

        private void UpdateViewState(string state)
        {
            VisualStateManager.GoToState(this, state, true);
        }

        private void UpdateEditLabelViewState()
        {
            if (editingLabel)
            {
                editLabel_tbx.Text = VIEW_MODEL.MODEL.DeviceLabel;
                UpdateViewState(EditLabelState.Name);
                ToolTipService.SetToolTip(editLabel_btn, new ToolTip { Content = "Save" });
            }
            else
            {
                UpdateViewState(DisplayLabelState.Name);
                VIEW_MODEL.SaveDeviceLabel(editLabel_tbx.Text, Account.Client.dbAccount.omemoInfo);
                ToolTipService.SetToolTip(editLabel_btn, new ToolTip { Content = "Change device label" });
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnAccountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountOmemoInfoControl omemoInfoControl)
            {
                omemoInfoControl.UpdateView(e);
            }
        }

        private void EditLabel_tbx_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (!string.Equals(args.NewText, args.NewText.TrimStart()))
            {
                args.Cancel = true;
            }
        }

        private void EditLabel_tbx_EnterKeyDown(object sender, KeyRoutedEventArgs e)
        {
            editingLabel = false;
            UpdateEditLabelViewState();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateEditLabelViewState();
        }

        private void EditLabel_btn_Click(object sender, RoutedEventArgs e)
        {
            editingLabel = !editingLabel;
            UpdateEditLabelViewState();
        }

        #endregion
    }
}
