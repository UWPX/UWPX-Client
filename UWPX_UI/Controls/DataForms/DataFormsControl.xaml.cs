using Shared.Classes.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace UWPX_UI.Controls.DataForms
{
    public sealed partial class DataFormsControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public DataForm Form
        {
            get { return (DataForm)GetValue(FormProperty); }
            set { SetValue(FormProperty, value); }
        }
        public static readonly DependencyProperty FormProperty = DependencyProperty.Register(nameof(Form), typeof(DataForm), typeof(DataFormsControl), new PropertyMetadata(null, OnFormChanged));

        public readonly CustomObservableCollection<Field> FIELDS = new CustomObservableCollection<Field>(true);

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
            if (!(Form is null) && !string.IsNullOrEmpty(Form.titel))
            {
                title_tblck.Visibility = Visibility.Visible;
                title_tblck.Text = Form.titel;
            }
            else
            {
                title_tblck.Visibility = Visibility.Collapsed;
            }
        }

        private void SetInstructions()
        {
            if (!(Form is null) && !string.IsNullOrEmpty(Form.instructions))
            {
                instructions_tblck.Visibility = Visibility.Visible;
                instructions_tblck.Text = Form.instructions;
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
            FIELDS.Clear();
            if (!(Form is null))
            {
                FIELDS.AddRange(Form.FIELDS);
            }
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
