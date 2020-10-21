using System.Linq;
using Shared.Classes;
using Shared.Classes.Collections;
using UWPX_UI_Context.Classes.Events;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.IoT
{
    public class DataFormDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private DataForm _Form;
        public DataForm Form
        {
            get => _Form;
            set => SetFormProperty(value);
        }

        public string Title
        {
            get => Form?.titel;
            set => SetProperty(ref Form.titel, value);
        }

        public string Instructions
        {
            get => Form?.instructions;
            set => SetProperty(ref Form.instructions, value);
        }

        public readonly CustomObservableCollection<FieldDataTemplate> FIELDS = new CustomObservableCollection<FieldDataTemplate>(true);

        public delegate void FieldValueChangedByUserEventHandler(DataFormDataTemplate sender, FieldValueChangedByUserEventArgs args);

        public event FieldValueChangedByUserEventHandler FieldValueChangedByUser;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DataFormDataTemplate(DataForm form)
        {
            Form = form;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetFormProperty(DataForm value)
        {
            if (SetProperty(ref _Form, value, nameof(Form)))
            {
                UnsubscribeFromAllFields();
                FIELDS.Clear();
                if (!(value is null))
                {
                    // Show only all non  hidden fields:
                    FIELDS.AddRange(value.fields.Where(x => x.type != FieldType.HIDDEN).Select(x =>
                    {
                        FieldDataTemplate field = new FieldDataTemplate(x);
                        field.ValueChangedByUser += Field_ValueChangedByUser;
                        return field;
                    }));
                }

                // Trigger changed to make sure they get updated:
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Instructions));
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UnsubscribeFromAllFields()
        {
            foreach (FieldDataTemplate field in FIELDS)
            {
                field.ValueChangedByUser -= Field_ValueChangedByUser;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Field_ValueChangedByUser(FieldDataTemplate sender, ValueChangedByUserEventArgs args)
        {
            FieldValueChangedByUser?.Invoke(this, new FieldValueChangedByUserEventArgs(sender));
        }

        #endregion
    }
}
