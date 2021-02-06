using System.ComponentModel;
using Manager.Classes.Chat;
using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public class ChatMessageListControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatMessageCacheList CHAT_MESSAGES = DataCache.INSTANCE.CHAT_MESSAGES;

        private bool _IsDummy;
        public bool IsDummy
        {
            get => _IsDummy;
            set => SetProperty(ref _IsDummy, value);
        }

        public event PropertyChangedEventHandler ChatChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


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


        #endregion
    }
}
