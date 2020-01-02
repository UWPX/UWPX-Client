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
                title_tblck.Visibility = Visibility.Visible;
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
                instructions_tblck.Visibility = Visibility.Visible;
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
