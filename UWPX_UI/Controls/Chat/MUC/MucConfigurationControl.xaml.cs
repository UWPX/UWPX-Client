using System.ComponentModel;
using System.Threading.Tasks;
using Shared.Classes;
using UWPX_UI.Dialogs;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.Events;
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

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucConfigurationControl()
        {
            VIEW_MODEL.OnError += VIEW_MODEL_OnError;
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

        private async Task ShowErrorDialogAsync(string errorText)
        {
            InfoDialog dialog = new InfoDialog("Error", errorText);
            await UiUtils.ShowDialogAsync(dialog);
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
            await ShowErrorDialogAsync(VIEW_MODEL.MODEL.ErrorText);
        }

        private async void VIEW_MODEL_OnError(MucConfigurationControlContext sender, MucConfigurationErrorEventArgs args)
        {
            await SharedUtils.CallDispatcherAsync(async () => await ShowErrorDialogAsync(args.ERROR_TEXT));
        }

        private void error_btn_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button error_btn)
            {
                this.error_btn = error_btn;
            }
        }

        private void MODEL_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(VIEW_MODEL.MODEL.HasError) when !(error_btn is null):
                    error_btn.Visibility = VIEW_MODEL.MODEL.HasError ? Visibility.Visible : Visibility.Collapsed;
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}
