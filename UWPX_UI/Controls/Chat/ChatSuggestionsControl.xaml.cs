using System.Threading.Tasks;
using Manager.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ChatSuggestionsControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Client Client
        {
            get => (Client)GetValue(ClientProperty);
            set => SetValue(ClientProperty, value);
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register(nameof(Client), typeof(Client), typeof(ChatSuggestionsControl), new PropertyMetadata(null, OnClientChanged));

        public string FilterText
        {
            get => (string)GetValue(FilterTextProperty);
            set => SetValue(FilterTextProperty, value);
        }
        public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register(nameof(FilterText), typeof(string), typeof(ChatSuggestionsControl), new PropertyMetadata("", OnFilterTextChanged));

        public readonly ChatSuggestionsControlContext VIEW_MODEL = new ChatSuggestionsControlContext();

        public delegate void SelectionChangedHandler(ChatSuggestionsControl sender, SelectionChangedEventArgs args);
        public event SelectionChangedHandler SelectionChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatSuggestionsControl()
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
        private async Task UpdateViewAsync()
        {
            await VIEW_MODEL.UpdateViewAsync(Client);
        }

        private void UpdateView()
        {
            VIEW_MODEL.UpdateView(FilterText);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async static void OnClientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatSuggestionsControl control)
            {
                await control.UpdateViewAsync();
            }
        }

        private static void OnFilterTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatSuggestionsControl control)
            {
                control.UpdateView();
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        #endregion
    }
}
