using Data_Manager2.Classes;
using Microsoft.Toolkit.Uwp.UI;
using Shared.Classes;
using System;
using System.Runtime.CompilerServices;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public class ChatFilterDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _ChatQuery;
        public string ChatQuery
        {
            get { return _ChatQuery; }
            set
            {
                if (!string.Equals(_ChatQuery, value.ToLowerInvariant()))
                {
                    _ChatQuery = value.ToLowerInvariant();
                    OnChatQueryChanged();
                }
            }
        }
        private bool _ChatQueryEnabled;
        public bool ChatQueryEnabled
        {
            get { return _ChatQueryEnabled; }
            set
            {
                if (_ChatQueryEnabled != value)
                {
                    _ChatQueryEnabled = value;
                    OnChatQueryEnabledChanged();
                }
            }
        }

        private bool _NotOnline;
        public bool NotOnline
        {
            get { return _NotOnline; }
            set
            {
                if (_NotOnline != value)
                {
                    _NotOnline = value;
                    OnNotOnlineChanged();
                }
            }
        }
        private bool _NotUnavailable;
        public bool NotUnavailable
        {
            get { return _NotUnavailable; }
            set
            {
                if (_NotUnavailable != value)
                {
                    _NotUnavailable = value;
                    OnNotUnavailableChanged();
                }
            }
        }
        private bool _ChatsOnly;
        public bool ChatsOnly
        {
            get { return _ChatsOnly; }
            set
            {
                if (_ChatsOnly != value)
                {
                    _ChatsOnly = value;
                    OnChatsOnlyChanged();
                }
            }
        }
        private bool _MucsOnly;
        public bool MucsOnly
        {
            get { return _MucsOnly; }
            set
            {
                if (_MucsOnly != value)
                {
                    _MucsOnly = value;
                    OnMucsOnlyChanged();
                }
            }
        }
        private bool _PresenceOnline;
        public bool PresenceOnline
        {
            get { return _PresenceOnline; }
            set
            {
                if (_PresenceOnline != value)
                {
                    _PresenceOnline = value;
                    OnPresenceOnlineChanged();
                }
            }
        }
        private bool _PresenceAway;
        public bool PresenceAway
        {
            get { return _PresenceAway; }
            set
            {
                if (_PresenceAway != value)
                {
                    _PresenceAway = value;
                    OnPresenceUnavailableChanged();
                }
            }
        }
        private bool _PresenceUnavailable;
        public bool PresenceUnavailable
        {
            get { return _PresenceUnavailable; }
            set
            {
                if (_PresenceUnavailable != value)
                {
                    _PresenceUnavailable = value;
                    OnPresenceUnavailableChanged();
                }
            }
        }
        private bool _PresenceDnd;
        public bool PresenceDnd
        {
            get { return _PresenceDnd; }
            set
            {
                if (_PresenceDnd != value)
                {
                    _PresenceDnd = value;
                    OnPresenceDndChanged();
                }
            }
        }
        private bool _PresenceXa;
        public bool PresenceXa
        {
            get { return _PresenceXa; }
            set
            {
                if (_PresenceXa != value)
                {
                    _PresenceXa = value;
                    OnPresenceXaChanged();
                }
            }
        }
        private bool _PresenceChat;
        public bool PresenceChat
        {
            get { return _PresenceChat; }
            set
            {
                if (_PresenceChat != value)
                {
                    _PresenceChat = value;
                    OnPresenceChatChanged();
                }
            }
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
                this.CHATS_ACV = chatsACV;

                this._PresenceOnline = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_PRESENCE_ONLINE);
                this._PresenceAway = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_PRESENCE_AWAY);
                this._PresenceUnavailable = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_NOT_UNAVAILABLE);
                this._PresenceXa = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_PRESENCE_XA);
                this._PresenceDnd = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_PRESENCE_DND);
                this._PresenceChat = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_PRESENCE_CHAT);


                this._ChatQuery = Settings.getSettingString(SettingsConsts.CHAT_FILTER_QUERY, string.Empty);
                this._ChatQueryEnabled = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_QUERY_ENABLED);
                this._NotOnline = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_NOT_ONLINE);
                this._NotUnavailable = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_NOT_UNAVAILABLE);
                this._ChatsOnly = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_CHATS_ONLY);
                this._MucsOnly = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_MUCS_ONLY);
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetPresencesTo(bool b)
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

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool Filter(object o)
        {
            return (o is ChatDataTemplate chat
                    && (!ChatQueryEnabled
                        || string.IsNullOrEmpty(ChatQuery)
                        || FilterChatQuery(chat))
                    && FilterPresence(chat))
                    && FilterChatType(chat);
        }

        public void ClearFilter()
        {
            using (DeferRefresh())
            {
                SetPresencesTo(false);
                SetGeneralPresencesTo(false);
                MucsOnly = false;
                ChatsOnly = false;
            }
        }

        #region --Filter Object Changed--
        private void OnChatQueryChanged()
        {
            SaveChatQuery();
            OnPropertyChanged(nameof(ChatQuery));
        }

        private void OnChatQueryEnabledChanged()
        {
            using (DeferRefresh())
            {
                OnPropertyChanged(nameof(ChatQueryEnabled));
            }
            SaveChatQueryEnabled();
        }

        private void OnNotOnlineChanged()
        {
            if (NotOnline)
            {
                using (DeferRefresh())
                {
                    SetPresencesTo(false);
                    NotUnavailable = false;
                }
            }
            SaveNotOnline();
            OnPropertyChanged(nameof(NotOnline));
        }

        private void OnNotUnavailableChanged()
        {
            if (NotUnavailable)
            {
                using (DeferRefresh())
                {
                    SetPresencesTo(false);
                    NotOnline = false;
                }
            }
            SaveNotUnavailable();
            OnPropertyChanged(nameof(NotUnavailable));
        }

        private void OnPresenceOnlineChanged()
        {
            if (PresenceOnline)
            {
                using (DeferRefresh())
                {
                    SetGeneralPresencesTo(false);
                }
            }
            SavePresenceOnline();
            OnPropertyChanged(nameof(PresenceOnline));
        }

        private void OnPresenceAwayChanged()
        {
            if (PresenceAway)
            {
                using (DeferRefresh())
                {
                    SetGeneralPresencesTo(false);
                }
            }
            SavePresenceAway();
            OnPropertyChanged(nameof(PresenceAway));
        }

        private void OnPresenceUnavailableChanged()
        {
            if (PresenceUnavailable)
            {
                using (DeferRefresh())
                {
                    SetGeneralPresencesTo(false);
                }
            }
            SavePresenceUnavailable();
            OnPropertyChanged(nameof(PresenceUnavailable));
        }

        private void OnPresenceXaChanged()
        {
            if (PresenceXa)
            {
                using (DeferRefresh())
                {
                    SetGeneralPresencesTo(false);
                }
            }
            SavePresenceOnline();
            OnPropertyChanged(nameof(PresenceXa));
        }

        private void OnPresenceChatChanged()
        {
            if (PresenceChat)
            {
                using (DeferRefresh())
                {
                    SetGeneralPresencesTo(false);
                }
            }
            SavePresenceOnline();
            OnPropertyChanged(nameof(PresenceChat));
        }

        private void OnPresenceDndChanged()
        {
            if (PresenceDnd)
            {
                using (DeferRefresh())
                {
                    SetGeneralPresencesTo(false);
                }
            }
            SavePresenceDnd();
            OnPropertyChanged(nameof(PresenceDnd));
        }

        private void OnMucsOnlyChanged()
        {
            if (MucsOnly)
            {
                using (DeferRefresh())
                {
                    ChatsOnly = false;
                }
            }
            SaveMucsOnly();
            OnPropertyChanged(nameof(MucsOnly));
        }

        private void OnChatsOnlyChanged()
        {
            if (ChatsOnly)
            {
                using (DeferRefresh())
                {
                    MucsOnly = false;
                }
            }
            SaveChatsOnly();
            OnPropertyChanged(nameof(ChatsOnly));
        }

        #endregion
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
                return chat.Chat.chatType == ChatType.CHAT && ChatsOnly || chat.Chat.chatType == ChatType.MUC && MucsOnly;
            }
            return true;
        }

        #region --Save Objects--
        private void SaveChatQuery()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_QUERY, ChatQuery);
        }

        private void SaveChatQueryEnabled()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_QUERY_ENABLED, ChatQueryEnabled);
        }

        private void SaveNotOnline()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_NOT_ONLINE, NotOnline);
        }

        private void SaveNotUnavailable()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_NOT_UNAVAILABLE, NotUnavailable);
        }

        private void SavePresenceOnline()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_QUERY, ChatQuery);
        }

        private void SavePresenceUnavailable()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_PRESENCE_UNAVAILABLE, PresenceUnavailable);
        }

        private void SavePresenceChat()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_PRESENCE_CHAT, PresenceChat);
        }

        private void SavePresenceAway()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_PRESENCE_AWAY, PresenceAway);
        }

        private void SavePresenceXa()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_PRESENCE_XA, PresenceXa);
        }

        private void SavePresenceDnd()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_PRESENCE_DND, PresenceDnd);
        }

        private void SaveMucsOnly()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_MUCS_ONLY, MucsOnly);
        }

        private void SaveChatsOnly()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_CHATS_ONLY, ChatsOnly);
        }

        #endregion

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
