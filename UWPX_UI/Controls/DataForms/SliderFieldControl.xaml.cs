using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages.XEP_0336;
using XMPP_API.Classes.Network.XML.Messages.XEP_IoT.Controls;

namespace UWPX_UI.Controls.DataForms
{
    public sealed partial class SliderFieldControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public FieldDataTemplate Field
        {
            get => (FieldDataTemplate)GetValue(FieldProperty);
            set => SetValue(FieldProperty, value);
        }
        public static readonly DependencyProperty FieldProperty = DependencyProperty.Register(nameof(Field), typeof(FieldDataTemplate), typeof(SliderFieldControl), new PropertyMetadata(null, OnFieldChanged));

        private bool supressValueChanged;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SliderFieldControl()
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
                slider.Header = Field.Label;
                // Value:
                if (Field.Value is double val)
                {
                    slider.Value = val;
                }
                else if (Field.Value is string s)
                {
                    if (double.TryParse(s, out double tmp))
                    {
                        Field.Value = tmp;
                        slider.Value = tmp;
                    }
                }
                // Properties:
                if (Field.Field.addIoTProps is SliderFieldProperties props)
                {
                    slider.Minimum = props.MIN;
                    slider.Maximum = props.MAX;
                    slider.TickFrequency = (props.MAX - props.MIN) / 10;
                    slider.StepFrequency = props.STEPS;
                }
                // Options:
                slider.IsEnabled = !Field.Field.dfConfiguration.flags.HasFlag(DynamicFormsFlags.READ_ONLY);
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
            if (d is SliderFieldControl control)
            {
                control.UpdateView(e);
            }
        }

        private void Slider_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            if (!supressValueChanged)
            {
                double val = 0;
                if (Field.Value is double d)
                {
                    val = d;
                }
                else if (Field.Value is string s)
                {
                    double.TryParse(s, out val);
                }

                if (val != slider.Value)
                {
                    Field.Value = slider.Value;
                    Field.OnValueChangedByUser();
                }
            }
        }

        private void Field_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateUi();
        }
        #endregion
    }
}
