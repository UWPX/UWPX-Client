using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace UWP_XMPP_Client.Controls.Muc
{
    public sealed partial class MucFieldControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Field Field
        {
            get { return (Field)GetValue(FieldProperty); }
            set
            {
                SetValue(FieldProperty, value);
                showField();
            }
        }
        public static readonly DependencyProperty FieldProperty = DependencyProperty.Register("Field", typeof(Field), typeof(MucFieldControl), null);

        public bool InputEnabled
        {
            get { return (bool)GetValue(ReadonlyProperty); }
            set { SetValue(ReadonlyProperty, value); }
        }
        public static readonly DependencyProperty ReadonlyProperty = DependencyProperty.Register("InputEnabled", typeof(bool), typeof(MucFieldControl), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/02/2018 Created [Fabian Sauter]
        /// </history>
        public MucFieldControl()
        {
            this.InitializeComponent();
            this.InputEnabled = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showField()
        {
            if (Field != null)
            {
                label_tblck.Text = Field.label ?? (Field.var ?? "No description / 'var' given!");
                switch (Field.type)
                {
                    case FieldType.TEXT_SINGLE:
                    case FieldType.TEXT_MULTI:
                        textField_tbx.Text = (string)(Field.value ?? "");
                        textField_tbx.Visibility = Visibility.Visible;
                        break;

                    case FieldType.TEXT_PRIVATE:
                        passwordField_pswd.Password = (string)(Field.value ?? "");
                        passwordField_pswd.Visibility = Visibility.Visible;
                        break;

                    case FieldType.FIXED:
                        label_tblck.Text = (string)(Field.value ?? "");
                        break;

                    case FieldType.BOOLEAN:
                        toggleField_tgls.IsOn = (bool)Field.value;
                        toggleField_tgls.Visibility = Visibility.Visible;
                        break;

                    case FieldType.LIST_SINGLE:
                        listField_cmbb.ItemsSource = Field.options;
                        if (Field.selectedOptions.Count > 0)
                        {
                            listField_cmbb.SelectedItem = Field.selectedOptions[0];
                        }
                        listField_cmbb.Visibility = Visibility.Visible;
                        break;

                    case FieldType.LIST_MULTI:
                        label_tblck.Visibility = Visibility.Collapsed;

                        listMulti_msc.header = Field.label ?? (Field.var ?? "No description / 'var' given!");
                        listMulti_msc.itemSource = new List<object>(Field.options);
                        listMulti_msc.setSelectedItems(new List<object>(Field.selectedOptions));

                        listMulti_msc.Visibility = Visibility.Visible;
                        break;

                    case FieldType.HIDDEN:
                    default:
                        Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void hideAllControls()
        {
            passwordField_pswd.Visibility = Visibility.Collapsed;
            textField_tbx.Visibility = Visibility.Collapsed;
            toggleField_tgls.Visibility = Visibility.Collapsed;
            listField_cmbb.Visibility = Visibility.Collapsed;
            label_tblck.Visibility = Visibility.Visible;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void passwordField_pswd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Field.value = passwordField_pswd.Password;
        }

        private void toggleField_tgls_Toggled(object sender, RoutedEventArgs e)
        {
            Field.value = toggleField_tgls.IsOn;
        }

        private void textField_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            Field.value = textField_tbx.Text;
        }

        private void listField_cmbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listField_cmbb.SelectedItem is FieldOption)
            {
                Field.selectedOptions.Clear();
                Field.selectedOptions.Add(listField_cmbb.SelectedItem as FieldOption);
            }
        }

        private void listMulti_msc_SelectionChanged(MultiSelectControl sender, Classes.Events.SelectionChangedMultiEventArgs args)
        {
            List<FieldOption> list = new List<FieldOption>();
            foreach (object o in args.SELECTED_ITEMS)
            {
                if (o is FieldOption)
                {
                    list.Add(o as FieldOption);
                }
            }
            Field.selectedOptions = list;
        }

        #endregion
    }
}
