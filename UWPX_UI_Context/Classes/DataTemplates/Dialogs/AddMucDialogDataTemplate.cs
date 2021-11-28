﻿using Manager.Classes;
using Shared.Classes;

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
            set => SetRoomBareJidProperty(value);
        }

        private string _Password;
        public string Password
        {
            get => _Password;
            set => SetProperty(ref _Password, value);
        }

        private Client _Client;
        public Client Client
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

        private bool _IsAdding;
        public bool IsAdding
        {
            get => _IsAdding;
            set => SetProperty(ref _IsAdding, value);
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
        private void SetRoomBareJidProperty(string value)
        {
            // Make sure we only allow lower case JIDs:
            value = value?.ToLowerInvariant();
            SetProperty(ref _RoomBareJid, value, nameof(RoomBareJid));
        }

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

        private void SetNicknameProperty(string value)
        {
            if (SetProperty(ref _Nickname, value, nameof(Nickname)))
            {
                IsNicknameValid = !string.IsNullOrWhiteSpace(value);
            }
        }

        private void SetClientProperty(Client value)
        {
            if (SetProperty(ref _Client, value, nameof(Client)) && value is not null)
            {
                Nickname = value.dbAccount.fullJid.localPart;
            }
        }
        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UpdateIsInputValid()
        {
            IsInputValid = IsRoomBareJidValid && IsNicknameValid;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
