using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class ChatBackgroundControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _ImagePath;
        public string ImagePath
        {
            get { return _ImagePath; }
            set { SetProperty(ref _ImagePath, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatBackgroundControlDataTemplate()
        {
            ChatBackgroundHelper.INSTANCE.PropertyChanged += INSTANCE_PropertyChanged;
            ImagePath = ChatBackgroundHelper.INSTANCE.ImagePath;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


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
        private void INSTANCE_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(ChatBackgroundHelper.ImagePath)))
            {
                ImagePath = ChatBackgroundHelper.INSTANCE.ImagePath;
            }
        }

        #endregion
    }
}
