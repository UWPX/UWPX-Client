using System.ComponentModel;
using System.Threading.Tasks;
using Manager.Classes.Chat;
using UWPX_UI.Dialogs;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat.MUC
{
    public sealed partial class MucConfigurationControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucConfigurationControlContext VIEW_MODEL = new MucConfigurationControlContext();

        public ChatDataTemplate Chat
        {
            get => (ChatDataTemplate)GetValue(ChatProperty);
            set => SetValue(ChatProperty, value);
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(Chat), typeof(ChatDataTemplate), typeof(MucConfigurationControl), new PropertyMetadata(null, OnChatChanged));

        private Button error_btn;
        private Button reload_btn;
        private Button save_btn;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucConfigurationControl()
        {
            VIEW_MODEL.MODEL.PropertyChanged += MODEL_PropertyChanged;
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

        private async Task ShowErrorDialogAsync()
        {
            InfoDialog dialog = new InfoDialog("Error", VIEW_MODEL.MODEL.ErrorMarkdownText);
            await UiUtils.ShowDialogAsync(dialog);
        }

        private async Task UpdateErrorButtonAsync()
        {
            if (error_btn is null)
            {
                return;
            }

            if (string.IsNullOrEmpty(VIEW_MODEL.MODEL.ErrorMarkdownText))
            {
                error_btn.Visibility = Visibility.Collapsed;
                return;
            }

            error_btn.Visibility = Visibility.Visible;
            await ShowErrorDialogAsync();
        }

        private void UpdateSaveButton()
        {
            if (!(save_btn is null))
            {
                save_btn.IsEnabled = VIEW_MODEL.MODEL.Success;
            }
        }

        private void UpdateReloadButton()
        {
            if (!(reload_btn is null))
            {
                reload_btn.IsEnabled = !VIEW_MODEL.MODEL.IsLoading;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnChatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MucConfigurationControl control)
            {
                control.UpdateView(e);
            }
        }

        private void Save_btn_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.Save(Chat);
        }

        private void Reload_btn_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.Reload(Chat);
        }

        private async void Error_btn_Click(object sender, RoutedEventArgs e)
        {
            await ShowErrorDialogAsync();
        }

        private async void error_btn_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button error_btn)
            {
                this.error_btn = error_btn;
                await UpdateErrorButtonAsync();
            }
        }

        private void reload_btn_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button reload_btn)
            {
                this.reload_btn = reload_btn;
                UpdateReloadButton();
            }
        }

        private void save_btn_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button save_btn)
            {
                this.save_btn = save_btn;
                UpdateSaveButton();
            }
        }

        private async void MODEL_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(VIEW_MODEL.MODEL.ErrorMarkdownText):
                    await UpdateErrorButtonAsync();
                    break;

                case nameof(VIEW_MODEL.MODEL.Success):
                    UpdateSaveButton();
                    break;

                case nameof(VIEW_MODEL.MODEL.IsLoading):
                    UpdateReloadButton();
                    UpdateSaveButton();
                    break;

                default:
                    break;
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateReloadButton();
            UpdateSaveButton();
            await UpdateErrorButtonAsync();
        }

        #endregion
    }
}
