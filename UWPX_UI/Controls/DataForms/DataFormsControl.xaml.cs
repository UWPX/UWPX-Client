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
