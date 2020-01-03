using Shared.Classes;
using UWPX_UI_Context.Classes.Events;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.IoT
{
    public class FieldDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private Field _Field;
        public Field Field
        {
            get => _Field;
            set => SetFieldProperty(value);
        }

        public string Label
        {
            get => Field?.label;
            set => SetProperty(ref Field.label, value);
        }

        public FieldType Type
        {
            get => Field is null ? FieldType.NONE : Field.type;
            set => SetProperty(ref Field.type, value);
        }

        public object Value
        {
            get => Field?.value;
            set => SetProperty(ref Field.value, value);
        }

        public string Var
        {
            get => Field?.var;
            set => SetProperty(ref Field.var, value);
        }

        public delegate void ValueChangedByUserEventHandler(FieldDataTemplate sender, ValueChangedByUserEventArgs args);

        public event ValueChangedByUserEventHandler ValueChangedByUser;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public FieldDataTemplate(Field field)
        {
            Field = field;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetFieldProperty(Field value)
        {
            if (SetProperty(ref _Field, value, nameof(Field)))
            {
                // Trigger changed to make sure they get updated:
                OnPropertyChanged(nameof(Label));
                OnPropertyChanged(nameof(Type));
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(Var));
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Should be called once the user has changed the value of the field.
        /// Triggers the ValueChangedByUser event.
        /// </summary>
        public void OnValueChangedByUser()
        {
            ValueChangedByUser?.Invoke(this, new ValueChangedByUserEventArgs());
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
