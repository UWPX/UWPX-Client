using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls
{
    public sealed partial class BareJidInputControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly BareJidInputControlContext VIEW_MODEL = new BareJidInputControlContext();

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(BareJidInputControl), new PropertyMetadata(""));

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(BareJidInputControl), new PropertyMetadata("alice@example.com"));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(BareJidInputControl), new PropertyMetadata(""));

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }
        public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register(nameof(IsValid), typeof(bool), typeof(BareJidInputControl), new PropertyMetadata(false));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public BareJidInputControl()
        {
            this.InitializeComponent();
            VIEW_MODEL.MODEL.PropertyChanged += MODEL_PropertyChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                VIEW_MODEL.MODEL.UpdateSuggestions(sender.Text);
            }
        }

        private void MODEL_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(VIEW_MODEL.MODEL.IsValid):
                    IsValid = VIEW_MODEL.MODEL.IsValid;
                    break;

                case nameof(VIEW_MODEL.MODEL.Text):
                    Text = VIEW_MODEL.MODEL.Text;
                    break;
            }
        }

        #endregion
    }
}
