using Shared.Classes;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Dialogs
{
    public sealed class AddMucDialogDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _Confirmed;
        public bool Confirmed
        {
            get => _Confirmed;
            set => SetProperty(ref _Confirmed, value);
        }

        private bool _IsInputValid;
        public bool IsInputValid
        {
            get => _IsInputValid;
            set => SetProperty(ref _IsInputValid, value);
        }

        private bool _IsRoomBareJidValid;
        public bool IsRoomBareJidValid
        {
            get => _IsRoomBareJidValid;
            set => SetIsRoomBareJidValidProperty(value);
        }

        private bool _IsNicknameValid;
        public bool IsNicknameValid
        {
            get => _IsNicknameValid;
            set => SetIsNicknameValidProperty(value);
        }

        private string _Nickname;
        public string Nickname
        {
            get => _Nickname;
            set => SetNicknameProperty(value);
        }

        private string _RoomBareJid;
        public string RoomBareJid
        {
            get => _RoomBareJid;
            set => SetProperty(ref _RoomBareJid, value);
        }

        private string _Password;
        public string Password
        {
            get => _Password;
            set => SetProperty(ref _Password, value);
        }

        private XMPPClient _Client;
        public XMPPClient Client
        {
            get => _Client;
            set => SetClientProperty(value);
        }

        private bool _Bookmark;
        public bool Bookmark
        {
            get => _Bookmark;
            set => SetProperty(ref _Bookmark, value);
        }

        private bool _AutoJoin;
        public bool AutoJoin
        {
            get => _AutoJoin;
            set => SetProperty(ref _AutoJoin, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AddMucDialogDataTemplate()
        {
            AutoJoin = true;
            Bookmark = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void SetIsRoomBareJidValidProperty(bool value)
        {
            if (SetProperty(ref _IsRoomBareJidValid, value, nameof(IsRoomBareJidValid)))
            {
                UpdateIsInputValid();
            }
        }

        private void SetIsNicknameValidProperty(bool value)
        {
            if (SetProperty(ref _IsNicknameValid, value, nameof(IsNicknameValid)))
            {
                UpdateIsInputValid();
            }
        }

        private void UpdateIsInputValid()
        {
            IsInputValid = IsRoomBareJidValid && IsNicknameValid;
        }

        private void SetNicknameProperty(string value)
        {
            if (SetProperty(ref _Nickname, value, nameof(Nickname)))
            {
                IsNicknameValid = !string.IsNullOrWhiteSpace(value);
            }
        }

        private void SetClientProperty(XMPPClient value)
        {
            if (SetProperty(ref _Client, value, nameof(Client)) && !(value is null))
            {
                Nickname = value.getXMPPAccount().user.localPart;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
