using Data_Manager2.Classes;
using Microsoft.Toolkit.Uwp.UI;
using System.Collections.Generic;
using UWP_XMPP_Client.DataTemplates;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Classes
{
    class ChatFilter
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string chatQuery { private set; get; }
        private string chatQueryLow;
        public bool chatQueryEnabled { private set; get; }

        public bool notOnline { private set; get; }
        public bool notUnavailable { private set; get; }

        private readonly HashSet<Presence> PRESENCES;

        public bool chat { private set; get; }
        public bool muc { private set; get; }


        private readonly AdvancedCollectionView CHATS_ACV;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/10/2018 Created [Fabian Sauter]
        /// </history>
        public ChatFilter(AdvancedCollectionView chatsACV)
        {
            this.CHATS_ACV = chatsACV;
            this.PRESENCES = Settings.LOCAL_OBJECT_STORAGE_HELPER.Read(SettingsConsts.CHAT_FILTER_PRESENCES, new HashSet<Presence>());

            this.chatQuery = Settings.getSettingString(SettingsConsts.CHAT_FILTER_QUERY, string.Empty);
            this.chatQueryLow = this.chatQuery.ToLower();
            this.chatQueryEnabled = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_QUERY_ENABLED);
            this.notOnline = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_NOT_ONLINE);
            this.notUnavailable = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_NOT_UNAVAILABLE);
            this.chat = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_CHAT);
            this.muc = Settings.getSettingBoolean(SettingsConsts.CHAT_FILTER_MUC);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool setChatQuery(string chatQueryNew)
        {
            string chatQueryNewLow = chatQueryNew.ToLower();
            if (string.Equals(chatQueryLow, chatQueryNewLow))
            {
                return true;
            }
            chatQuery = chatQueryNew;
            chatQueryLow = chatQueryNewLow;
            saveChatQuery();
            onFilterChanged();
            return false;
        }

        public void setNotOnline(bool notOnline)
        {
            if (this.notOnline != notOnline)
            {
                this.notOnline = notOnline;
                saveNotOnline();

                if (notOnline)
                {
                    notUnavailable = false;
                    saveNotUnavailable();
                    PRESENCES.Clear();
                    savePresences();
                }
                onFilterChanged();
            }
        }

        public void setNotUnavailable(bool notUnavailable)
        {
            if (this.notUnavailable != notUnavailable)
            {
                this.notUnavailable = notUnavailable;
                saveNotUnavailable();

                if (notUnavailable)
                {
                    notOnline = false;
                    saveNotOnline();
                    PRESENCES.Clear();
                    savePresences();
                }
                onFilterChanged();
            }
        }

        public bool hasPresenceFilter(Presence p)
        {
            return PRESENCES.Contains(p);
        }

        public void setPresenceFilter(Presence p, bool add)
        {
            if (add && PRESENCES.Add(p))
            {
                notOnline = false;
                notUnavailable = false;

                saveNotOnline();
                saveNotUnavailable();

                onFilterChanged();
                savePresences();
            }
            else if (PRESENCES.Remove(p))
            {
                onFilterChanged();
                savePresences();
            }
        }

        public void setChatQueryEnabled(bool chatQueryEnabled)
        {
            if (this.chatQueryEnabled != chatQueryEnabled)
            {
                this.chatQueryEnabled = chatQueryEnabled;
                saveChatQueryEnabled();
            }
        }

        public void setChatOnly(bool chatOnly)
        {
            if (this.chat != chatOnly)
            {
                this.chat = chatOnly;
                saveChat();
                onFilterChanged();
            }
        }

        public void setMUCOnly(bool mucOnly)
        {
            if (this.muc != mucOnly)
            {
                this.muc = mucOnly;
                saveMUC();
                onFilterChanged();
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool filter(object o)
        {
            return (o is ChatTemplate chat
                    && (!chatQueryEnabled
                        || string.IsNullOrEmpty(chatQueryLow)
                        || filterChatQuery(chat))
                    && filterPresence(chat))
                    && filterChatType(chat);
        }

        public void clearPresenceFilter()
        {
            if (notOnline || notUnavailable || PRESENCES.Count > 0)
            {
                notOnline = false;
                notUnavailable = false;
                PRESENCES.Clear();

                saveNotOnline();
                saveNotUnavailable();
                savePresences();

                onFilterChanged();

                chat = false;
                muc = false;
                saveChat();
                saveMUC();
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private bool filterChatQuery(ChatTemplate chat)
        {
            // For searching we could also use something like this: https://www.codeproject.com/Articles/11157/An-improvement-on-capturing-similarity-between-str
            return chat.chat.chatJabberId.ToLower().Contains(chatQueryLow)
                || (chat.mucInfo != null
                    && chat.mucInfo.name != null
                    && chat.mucInfo.name.ToLower().Contains(chatQueryLow));
        }

        private bool filterPresence(ChatTemplate chat)
        {
            if (notOnline)
            {
                return chat.chat.presence != Presence.Online;
            }
            else if (notUnavailable)
            {
                return chat.chat.presence != Presence.Unavailable;
            }
            return PRESENCES.Count <= 0 || PRESENCES.Contains(chat.chat.presence);
        }

        private bool filterChatType(ChatTemplate chat)
        {
            if (this.chat || muc)
            {
                return chat.chat.chatType == ChatType.CHAT && this.chat || chat.chat.chatType == ChatType.MUC && muc;
            }
            return true;
        }

        private void onFilterChanged()
        {
            CHATS_ACV.RefreshFilter();
        }

        private void savePresences()
        {
            Settings.LOCAL_OBJECT_STORAGE_HELPER.Save(SettingsConsts.CHAT_FILTER_PRESENCES, PRESENCES);
        }

        private void saveNotOnline()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_NOT_ONLINE, notOnline);
        }

        private void saveNotUnavailable()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_NOT_UNAVAILABLE, notUnavailable);
        }

        private void saveChatQuery()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_QUERY, chatQuery);
        }

        private void saveChatQueryEnabled()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_QUERY_ENABLED, chatQueryEnabled);
        }

        private void saveChat()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_CHAT, chat);
        }

        private void saveMUC()
        {
            Settings.setSetting(SettingsConsts.CHAT_FILTER_MUC, muc);
        }
        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
