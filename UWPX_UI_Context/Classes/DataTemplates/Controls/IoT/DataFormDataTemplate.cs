using System.Linq;
using Shared.Classes;
using Shared.Classes.Collections;
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
            get => Form?.titel;
            set => SetProperty(ref Form.titel, value);
        }

        public readonly CustomObservableCollection<FieldDataTemplate> FIELDS = new CustomObservableCollection<FieldDataTemplate>(true);

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
                FIELDS.Clear();
                if (!(value is null))
                {
                    FIELDS.AddRange(value.fields.Select(x => new FieldDataTemplate(x)));
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
