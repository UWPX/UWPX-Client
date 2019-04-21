using System;
using System.Runtime.CompilerServices;
using Data_Manager2.Classes;
using Shared.Classes;
using UWPX_UI_Context.Classes.Collections.Toolkit;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public class ChatFilterDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _ChatQuery;
        public string ChatQuery
        {
            get => _ChatQuery;
            set => SetSettingsProperty(ref _ChatQuery, value, SettingsConsts.CHAT_FILTER_QUERY);
        }
        private bool _ChatQueryEnabled;
        public bool ChatQueryEnabled
        {
            get => _ChatQueryEnabled;
            set => SetSettingsProperty(ref _ChatQueryEnabled, value, SettingsConsts.CHAT_FILTER_QUERY_ENABLED);
        }

        private bool _NotOnline;
        public bool NotOnline
        {
            get => _NotOnline;
            set => SetNotOnlineProperty(value);
        }
        private bool _NotUnavailable;
        public bool NotUnavailable
        {
            get => _NotUnavailable;
            set => SetNotUnavailableProperty(value);
        }
        private bool _ChatsOnly;
        public bool ChatsOnly
        {
            get => _ChatsOnly;
            set => SetChatsOnlyProperty(value);
        }
        private bool _MucsOnly;
        public bool MucsOnly
        {
            get => _MucsOnly;
            set => SetMucsOnlyProperty(value);
        }
        private bool _PresenceOnline;
        public bool PresenceOnline
        {
            get => _PresenceOnline;
            set => SetPresenceOnlineProperty(value);
        }
        private bool _PresenceAway;
        public bool PresenceAway
        {
            get => _PresenceAway;
            set => SetPresenceAwayProperty(value);
        }
        private bool _PresenceUnavailable;
        public bool PresenceUnavailable
        {
            get => _PresenceUnavailable;
            set => SetPresenceUnavailableProperty(value);
        }
        private bool _PresenceDnd;
        public bool PresenceDnd
        {
            get => _PresenceDnd;
            set => SetPresenceDndProperty(value);
        }
        private bool _PresenceXa;
        public bool PresenceXa
        {
            get => _PresenceXa;
            set => SetPresenceXaProperty(value);
        }
        private bool _PresenceChat;
        public bool PresenceChat
        {
            get => _PresenceChat;
            set => SetPresenceChatProperty(value);
        }

        private readonly AdvancedCollectionView CHATS_ACV;

        private uint skipOnFilterChangedCount = 0;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatFilterDataTemplate(AdvancedCollectionView chatsACV)
        {
            using (DeferRefresh())
            {
                CHATS_ACV = chatsACV;

                _PresenceOnline = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_PRESENCE_ONLINE);
                _PresenceAway = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_PRESENCE_AWAY);
                _PresenceUnavailable = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_NOT_UNAVAILABLE);
                _PresenceXa = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_PRESENCE_XA);
                _PresenceDnd = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_PRESENCE_DND);
                _PresenceChat = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_PRESENCE_CHAT);


                _ChatQuery = Settings.getSettingString(SettingsConsts.CHAT_FILTER_QUERY, string.Empty);
                _ChatQueryEnabled = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_QUERY_ENABLED);
                _NotOnline = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_NOT_ONLINE);
                _NotUnavailable = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_NOT_UNAVAILABLE);
                _ChatsOnly = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_CHATS_ONLY);
                _MucsOnly = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_MUCS_ONLY);
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetPresences(bool b)
        {
            PresenceOnline = b;
            PresenceAway = b;
            PresenceDnd = b;
            PresenceChat = b;
            PresenceXa = b;
            PresenceUnavailable = b;
        }

        private void SetGeneralPresencesTo(bool b)
        {
            NotOnline = b;
            NotUnavailable = b;
        }

        private bool SetSettingsProperty<T>(ref T storage, T value, string settingsToken, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
                Settings.setSetting(settingsToken, value);
                return true;
            }
            return false;
        }

        private bool SetBoolInversedProperty(ref bool storage, bool value, string settingsToken, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
                Settings.setSetting(settingsToken, !value);
                return true;
            }
            return false;
        }

        private void SetNotOnlineProperty(bool value)
        {
            if (SetSettingsProperty(ref _NotOnline, value, SettingsConsts.CHAT_FILTER_NOT_ONLINE, nameof(NotOnline)) && value)
            {
                using (DeferRefresh())
                {
                    SetPresences(false);
                    NotUnavailable = false;
                }
            }
        }

        private void SetNotUnavailableProperty(bool value)
        {
            if (SetSettingsProperty(ref _NotUnavailable, value, SettingsConsts.CHAT_FILTER_NOT_UNAVAILABLE, nameof(NotUnavailable)) && value)
            {
                using (DeferRefresh())
                {
                    SetPresences(false);
                    NotOnline = false;
                }
            }
        }

        private void SetMucsOnlyProperty(bool value)
        {
            if (SetSettingsProperty(ref _MucsOnly, value, SettingsConsts.CHAT_FILTER_MUCS_ONLY, nameof(MucsOnly)) && value)
            {
                using (DeferRefresh())
                {
                    ChatsOnly = false;
                }
            }
        }

        private void SetChatsOnlyProperty(bool value)
        {
            if (SetSettingsProperty(ref _ChatsOnly, value, SettingsConsts.CHAT_FILTER_CHATS_ONLY, nameof(ChatsOnly)) && value)
            {
                using (DeferRefresh())
                {
                    MucsOnly = false;
                }
            }
        }

        private void SetPresenceChatProperty(bool value)
        {
            if (SetSettingsProperty(ref _PresenceChat, value, SettingsConsts.CHAT_FILTER_PRESENCE_CHAT, nameof(PresenceChat)) && value)
            {
                using (DeferRefresh())
                {
                    SetGeneralPresencesTo(false);
                }
            }
        }

        private void SetPresenceOnlineProperty(bool value)
        {
            if (SetSettingsProperty(ref _PresenceOnline, value, SettingsConsts.CHAT_FILTER_PRESENCE_ONLINE, nameof(PresenceOnline)) && value)
            {
                using (DeferRefresh())
                {
                    SetGeneralPresencesTo(false);
                }
            }
        }

        private void SetPresenceAwayProperty(bool value)
        {
            if (SetSettingsProperty(ref _PresenceAway, value, SettingsConsts.CHAT_FILTER_PRESENCE_AWAY, nameof(PresenceAway)) && value)
            {
                using (DeferRefresh())
                {
                    SetGeneralPresencesTo(false);
                }
            }
        }

        private void SetPresenceXaProperty(bool value)
        {
            if (SetSettingsProperty(ref _PresenceXa, value, SettingsConsts.CHAT_FILTER_PRESENCE_XA, nameof(PresenceXa)) && value)
            {
                using (DeferRefresh())
                {
                    SetGeneralPresencesTo(false);
                }
            }
        }

        private void SetPresenceDndProperty(bool value)
        {
            if (SetSettingsProperty(ref _PresenceDnd, value, SettingsConsts.CHAT_FILTER_PRESENCE_CHAT, nameof(PresenceDnd)) && value)
            {
                using (DeferRefresh())
                {
                    SetGeneralPresencesTo(false);
                }
            }
        }

        private void SetPresenceUnavailableProperty(bool value)
        {
            if (SetSettingsProperty(ref _PresenceUnavailable, value, SettingsConsts.CHAT_FILTER_PRESENCE_CHAT, nameof(PresenceUnavailable)) && value)
            {
                using (DeferRefresh())
                {
                    SetGeneralPresencesTo(false);
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool Filter(object o)
        {
            return o is ChatDataTemplate chat
                    && (!ChatQueryEnabled
                        || string.IsNullOrEmpty(ChatQuery)
                        || FilterChatQuery(chat))
                    && FilterPresence(chat)
                    && FilterChatType(chat);
        }

        public void ClearFilter()
        {
            using (DeferRefresh())
            {
                SetPresences(false);
                SetGeneralPresencesTo(false);
                MucsOnly = false;
                ChatsOnly = false;
            }
        }

        public IDisposable DeferRefresh()
        {
            return new FilterChangedDeferrer(this);
        }

        public void RefreshFilter()
        {
            CHATS_ACV.RefreshFilter();
        }

        #endregion

        #region --Misc Methods (Private)--
        private bool FilterChatQuery(ChatDataTemplate chat)
        {
            // For searching we could also use something like this: https://www.codeproject.com/Articles/11157/An-improvement-on-capturing-similarity-between-str
            return chat.Chat.chatJabberId.ToLowerInvariant().Contains(ChatQuery)
                || (chat.MucInfo != null
                    && chat.MucInfo.name != null
                    && chat.MucInfo.name.ToLowerInvariant().Contains(ChatQuery));
        }

        private bool FilterPresence(ChatDataTemplate chat)
        {
            if (NotOnline)
            {
                if (chat.Chat.chatType == ChatType.CHAT)
                {
                    return chat.Chat.presence != Presence.Online;
                }
                else
                {
                    return chat.MucInfo != null && chat.MucInfo.state != MUCState.ENTERD;
                }
            }
            else if (NotUnavailable)
            {
                if (chat.Chat.chatType == ChatType.CHAT)
                {
                    return chat.Chat.presence != Presence.Unavailable;
                }
                else
                {
                    return chat.MucInfo != null && chat.MucInfo.state != MUCState.DISCONNECTED;
                }
            }

            Presence chatPresence = chat.Chat.presence;

            if (PresenceOnline || PresenceAway || PresenceUnavailable || PresenceChat || PresenceDnd || PresenceXa)
            {
                return (PresenceOnline && chatPresence == Presence.Online)
                || (PresenceAway && chatPresence == Presence.Away)
                || (PresenceUnavailable && chatPresence == Presence.Unavailable)
                || (PresenceChat && chatPresence == Presence.Chat)
                || (PresenceDnd && chatPresence == Presence.Dnd)
                || (PresenceXa && chatPresence == Presence.Xa);
            }
            return true;
        }

        private bool FilterChatType(ChatDataTemplate chat)
        {
            if (ChatsOnly || MucsOnly)
            {
                return (chat.Chat.chatType == ChatType.CHAT && ChatsOnly) || (chat.Chat.chatType == ChatType.MUC && MucsOnly);
            }
            return true;
        }
        #endregion

        #region --Misc Methods (Protected)--
        protected override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(name);

            if (skipOnFilterChangedCount <= 0)
            {
                RefreshFilter();
            }
        }

        #endregion

        #region --Misc Methods (Internal)--
        internal void DeferRefreshUp()
        {
            skipOnFilterChangedCount++;
        }

        internal void DeferRefreshDown()
        {
            if (skipOnFilterChangedCount > 0)
            {
                if (--skipOnFilterChangedCount > 0)
                {
                    RefreshFilter();
                }
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
