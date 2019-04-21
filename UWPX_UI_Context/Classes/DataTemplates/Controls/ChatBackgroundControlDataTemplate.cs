using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class ChatBackgroundControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _CustomImagePath;
        public string CustomImagePath
        {
            get => _CustomImagePath;
            set => SetProperty(ref _CustomImagePath, value);
        }

        private bool _IsSplashImageEnabled;
        public bool IsSplashImageEnabled
        {
            get => _IsSplashImageEnabled;
            set => SetProperty(ref _IsSplashImageEnabled, value);
        }

        private bool _IsCustomImageEnabled;
        public bool IsCustomImageEnabled
        {
            get => _IsCustomImageEnabled;
            set => SetProperty(ref _IsCustomImageEnabled, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatBackgroundControlDataTemplate()
        {
            ChatBackgroundHelper.INSTANCE.PropertyChanged += INSTANCE_PropertyChanged;
            LoadImagePath();
            LoadBackgroundMode();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadImagePath()
        {
            switch (ChatBackgroundHelper.INSTANCE.BackgroundMode)
            {
                case ChatBackgroundMode.CustomImage:
                    CustomImagePath = ChatBackgroundHelper.INSTANCE.CustomImagePath;
                    break;

                case ChatBackgroundMode.Image:
                    CustomImagePath = ChatBackgroundHelper.INSTANCE.ImagePath;
                    break;
            }
        }

        private void LoadBackgroundMode()
        {
            switch (ChatBackgroundHelper.INSTANCE.BackgroundMode)
            {
                case ChatBackgroundMode.SplashImage:
                    IsSplashImageEnabled = true;
                    IsCustomImageEnabled = false;
                    break;

                case ChatBackgroundMode.Image:
                case ChatBackgroundMode.CustomImage:
                    IsSplashImageEnabled = false;
                    IsCustomImageEnabled = true;
                    break;

                case ChatBackgroundMode.None:
                    IsSplashImageEnabled = false;
                    IsCustomImageEnabled = false;
                    break;
            }

        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void INSTANCE_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ChatBackgroundHelper.CustomImagePath):
                case nameof(ChatBackgroundHelper.ImagePath):
                    LoadImagePath();
                    break;

                case nameof(ChatBackgroundHelper.BackgroundMode):
                    LoadBackgroundMode();
                    LoadImagePath();
                    break;
            }
        }

        #endregion
    }
}
