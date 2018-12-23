using Data_Manager2.Classes.DBTables;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class SpeechBubbleContentDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _Text;

        public string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                OnPropertyChanged();
            }
        }


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(ChatTable chat, ChatMessageTable message)
        {
            if (chat is null || message is null)
            {
                return;
            }

            Text = message.message;
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
