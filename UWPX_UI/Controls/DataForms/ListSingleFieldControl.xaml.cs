using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0336;

namespace UWPX_UI.Controls.DataForms
{
    public sealed partial class ListSingleFieldControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public FieldDataTemplate Field
        {
            get => (FieldDataTemplate)GetValue(FieldProperty);
            set => SetValue(FieldProperty, value);
        }
        public static readonly DependencyProperty FieldProperty = DependencyProperty.Register(nameof(Field), typeof(FieldDataTemplate), typeof(ListSingleFieldControl), new PropertyMetadata(null, OnFieldChanged));

        private bool supressValueChanged;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ListSingleFieldControl()
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
            // Update subscriptions:
            if (e.OldValue is FieldDataTemplate oldField)
            {
                oldField.PropertyChanged -= Field_PropertyChanged;
            }
            if (e.NewValue is FieldDataTemplate newField)
            {
                newField.PropertyChanged += Field_PropertyChanged;
            }
            UpdateUi();
        }

        private void UpdateUi()
        {
            supressValueChanged = true;
            Visibility = Field is null ? Visibility.Collapsed : Visibility.Visible;
            if (!(Field is null))
            {
                // General:
                listSingle_cmbx.ItemsSource = Field.Field.options;
                if (Field.Field.selectedOptions.Count > 0)
                {
                    listSingle_cmbx.SelectedItem = Field.Field.selectedOptions[0];
                }
                listSingle_cmbx.Header = Field.Label;

                // Options:
                listSingle_cmbx.IsEnabled = !Field.Field.dfConfiguration.flags.HasFlag(DynamicFormsFlags.READ_ONLY);
            }
            supressValueChanged = false;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListSingleFieldControl control)
            {
                control.UpdateView(e);
            }
        }

        private void ListSingle_cmbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!supressValueChanged)
            {
                if ((Field.Field.selectedOptions.Count <= 0 && listSingle_cmbx.SelectedItem is null) || (Field.Field.selectedOptions[0] == listSingle_cmbx.SelectedItem))
                {
                    return;
                }

                Field.Field.selectedOptions.Clear();
                if (!(listSingle_cmbx.SelectedItem is null))
                {
                    Field.Field.selectedOptions.Add((FieldOption)listSingle_cmbx.SelectedItem);
                }
                Field.OnValueChangedByUser();
            }
        }

        private void Field_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateUi();
        }

        #endregion
    }
}
