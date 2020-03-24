using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.DataForms
{
    public sealed partial class DataFormsControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DataFormDataTemplate Form
        {
            get => (DataFormDataTemplate)GetValue(FormProperty);
            set => SetValue(FormProperty, value);
        }
        public static readonly DependencyProperty FormProperty = DependencyProperty.Register(nameof(Form), typeof(DataFormDataTemplate), typeof(DataFormsControl), new PropertyMetadata(null, OnFormChanged));

        public bool ShowTitle
        {
            get => (bool)GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }
        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(nameof(ShowTitle), typeof(bool), typeof(DataFormsControl), new PropertyMetadata(true));

        public bool ShowInstructions
        {
            get => (bool)GetValue(ShowInstructionsProperty);
            set => SetValue(ShowInstructionsProperty, value);
        }
        public static readonly DependencyProperty ShowInstructionsProperty = DependencyProperty.Register(nameof(ShowInstructions), typeof(bool), typeof(DataFormsControl), new PropertyMetadata(true));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DataFormsControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetTitle()
        {
            if (!(Form is null) && !string.IsNullOrEmpty(Form.Title))
            {
                title_tblck.Visibility = ShowTitle ? Visibility.Visible : Visibility.Collapsed;
                title_tblck.Text = Form.Title;
            }
            else
            {
                title_tblck.Visibility = Visibility.Collapsed;
            }
        }

        private void SetInstructions()
        {
            if (!(Form is null) && !string.IsNullOrEmpty(Form.Instructions))
            {
                instructions_tblck.Visibility = ShowInstructions ? Visibility.Visible : Visibility.Collapsed;
                instructions_tblck.Text = Form.Instructions;
            }
            else
            {
                instructions_tblck.Visibility = Visibility.Collapsed;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView()
        {
            SetTitle();
            SetInstructions();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnFormChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataFormsControl control)
            {
                control.UpdateView();
            }
        }

        #endregion
    }
}
