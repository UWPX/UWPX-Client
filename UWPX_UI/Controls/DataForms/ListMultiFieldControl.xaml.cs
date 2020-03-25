using System.Linq;
using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0336;

namespace UWPX_UI.Controls.DataForms
{
    public sealed partial class ListMultiFieldControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public FieldDataTemplate Field
        {
            get => (FieldDataTemplate)GetValue(FieldProperty);
            set => SetValue(FieldProperty, value);
        }
        public static readonly DependencyProperty FieldProperty = DependencyProperty.Register(nameof(Field), typeof(FieldDataTemplate), typeof(ListMultiFieldControl), new PropertyMetadata(null, OnFieldChanged));

        private bool supressValueChanged;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ListMultiFieldControl()
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
                listMulti_msc.Header = Field.Label ?? Field.Var ?? "No description / 'var' given!";
                listMulti_msc.ItemSource.Clear();
                listMulti_msc.ItemSource.AddRange(Field.Field.options);
                listMulti_msc.SetSelectedItems(Field.Field.selectedOptions.ToList<object>());

                // Options:
                listMulti_msc.IsEnabled = !Field.Field.dfConfiguration.flags.HasFlag(DynamicFormsFlags.READ_ONLY);
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
            if (d is ListMultiFieldControl control)
            {
                control.UpdateView(e);
            }
        }

        private void Field_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateUi();
        }

        private void MultiSelectControl_SelectionChanged(MultiSelectControl sender, Classes.Events.MultiSelectChangedEventArgs args)
        {
            if (!supressValueChanged)
            {
                Field.Field.selectedOptions.Clear();
                foreach (object item in args.SELECTED_ITEMS)
                {
                    Field.Field.selectedOptions.Add((FieldOption)item);
                }
                Field.OnValueChangedByUser();
            }
        }

        #endregion
    }
}
