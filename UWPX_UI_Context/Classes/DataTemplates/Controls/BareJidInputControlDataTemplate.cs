using Shared.Classes;
using Shared.Classes.Collections;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class BareJidInputControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { SetTextProperty(value); }
        }

        private bool _IsValid;
        public bool IsValid
        {
            get { return _IsValid; }
            set { SetProperty(ref _IsValid, value); }
        }

        public readonly CustomObservableCollection<string> SUGGESTIONS = new CustomObservableCollection<string>(true);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetTextProperty(string value)
        {
            if (SetProperty(ref _Text, value, nameof(Text)))
            {
                IsValid = Utils.isBareJid(value);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateSuggestions(string input)
        {
            SUGGESTIONS.Clear();
            int index = input.IndexOf('@');
            if (index >= 0)
            {
                string domain = input.Substring(index + 1);
                foreach (string s in Utils.COMMON_XMPP_SERVERS)
                {
                    if (s.StartsWith(domain) && !s.Equals(domain))
                    {
                        SUGGESTIONS.Add(input + s.Substring(domain.Length));
                    }
                }
            }
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
