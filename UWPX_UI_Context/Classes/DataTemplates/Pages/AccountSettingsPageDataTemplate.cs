﻿using System.Runtime.CompilerServices;
using Data_Manager2.Classes;
using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class AccountSettingsPageDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _PushEnabled;
        public bool PushEnabled
        {
            get => _PushEnabled;
            set => SetBoolProperty(ref _PushEnabled, value, SettingsConsts.PUSH_ENABLED);
        }

        private string _ChannelUri;
        public string ChannelUri
        {
            get => _ChannelUri;
            set => SetProperty(ref _ChannelUri, value);
        }

        private string _ChannelCreatedDate;
        public string ChannelCreatedDate
        {
            get => _ChannelCreatedDate;
            set => SetProperty(ref _ChannelCreatedDate, value);
        }

        private string _PushManagerState;
        public string PushManagerState
        {
            get => _PushManagerState;
            set => SetProperty(ref _PushManagerState, value);
        }

        private bool _DebugSettingsEnabled;
        public bool DebugSettingsEnabled
        {
            get => _DebugSettingsEnabled;
            set => SetProperty(ref _DebugSettingsEnabled, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private bool SetBoolProperty(ref bool storage, bool value, string settingsToken, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
                Settings.setSetting(settingsToken, value);
                return true;
            }
            return false;
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
