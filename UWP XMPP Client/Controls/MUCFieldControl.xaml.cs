using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045.Configuration;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class MUCFieldControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public MUCInfoField Field
        {
            get { return (MUCInfoField)GetValue(FieldProperty); }
            set
            {
                SetValue(FieldProperty, value);
                showField();
            }
        }
        public static readonly DependencyProperty FieldProperty = DependencyProperty.Register("Field", typeof(MUCInfoField), typeof(MUCFieldControl), null);

        public bool InputEnabled
        {
            get { return (bool)GetValue(ReadonlyProperty); }
            set { SetValue(ReadonlyProperty, value); }
        }
        public static readonly DependencyProperty ReadonlyProperty = DependencyProperty.Register("InputEnabled", typeof(bool), typeof(MUCFieldControl), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/02/2018 Created [Fabian Sauter]
        /// </history>
        public MUCFieldControl()
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
                    case MUCInfoFieldType.TEXT_SINGLE:
                    case MUCInfoFieldType.TEXT_MULTI:
                        textField_tbx.Text = (string)(Field.value ?? "");
                        textField_tbx.Visibility = Visibility.Visible;
                        break;

                    case MUCInfoFieldType.TEXT_PRIVATE:
                        passwordField_pswd.Password = (string)(Field.value ?? "");
                        passwordField_pswd.Visibility = Visibility.Visible;
                        break;

                    case MUCInfoFieldType.FIXED:
                        label_tblck.Text = (string)(Field.value ?? "");
                        break;

                    case MUCInfoFieldType.BOOLEAN:
                        toggleField_tgls.IsOn = (bool)Field.value;
                        toggleField_tgls.Visibility = Visibility.Visible;
                        break;

                    case MUCInfoFieldType.LIST_MULTI:
                        listField_cmbb.ItemsSource = Field.options;
                        if (Field.selectedOptions.Count > 0)
                        {
                            listField_cmbb.SelectedItem = Field.selectedOptions[0];
                        }
                        listField_cmbb.Visibility = Visibility.Visible;
                        break;

                    case MUCInfoFieldType.LIST_SINGLE:
                        label_tblck.Visibility = Visibility.Collapsed;

                        listMulti_msc.header = Field.label ?? (Field.var ?? "No description / 'var' given!");
                        listMulti_msc.setItems(new List<object>(Field.options));
                        listMulti_msc.setSelectedItems(new List<object>(Field.selectedOptions));

                        listMulti_msc.Visibility = Visibility.Visible;
                        break;

                    case MUCInfoFieldType.HIDDEN:
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
            if (listField_cmbb.SelectedItem is MUCInfoOption)
            {
                Field.selectedOptions.Add(listField_cmbb.SelectedItem as MUCInfoOption);
            }
        }

        private void listMulti_msc_SelectionChanged(MultiSelectControl sender, Classes.Events.SelectionChangedMultiEventArgs args)
        {
            List<MUCInfoOption> list = new List<MUCInfoOption>();
            foreach (object o in args.SELECTED_ITEMS)
            {
                if (o is MUCInfoOption)
                {
                    list.Add(o as MUCInfoOption);
                }
            }
            Field.selectedOptions = list;
        }

        #endregion
    }
}
