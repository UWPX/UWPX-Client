using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.Collections;
using UWPX_UI_Context.Classes.Collections.Toolkit;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Dialogs;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public sealed class ChatsPageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ObservableChatDictionaryList CHATS = new ObservableChatDictionaryList();
        public readonly ChatFilterDataTemplate CHAT_FILTER;

        public readonly AdvancedCollectionView CHATS_ACV;
        public ChatDataTemplate SelectedItem;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatsPageContext()
        {
            this.CHATS_ACV = new AdvancedCollectionView(CHATS, true)
            {
                Filter = AcvFilter
            };

            this.CHATS_ACV.ObserveFilterProperty(nameof(ChatDataTemplate.Chat));
            this.CHATS_ACV.SortDescriptions.Add(new Microsoft.Toolkit.Uwp.UI.SortDescription(nameof(ChatDataTemplate.Chat), Microsoft.Toolkit.Uwp.UI.SortDirection.Descending));
            this.CHAT_FILTER = new ChatFilterDataTemplate(this.CHATS_ACV);

            LoadChats();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task OnAddChatAsync(AddChatDialogDataTemplate dataTemplate)
        {
            if (dataTemplate.Confirmed)
            {
                await AddChatAsync(dataTemplate.Client, dataTemplate.ChatBareJid, dataTemplate.AddToRoster, dataTemplate.SubscribeToPresence);
            }
        }

        public void OnNavigatedTo()
        {
            // Subscribe to toast events:
            ToastHelper.OnChatMessageToast -= ToastHelper_OnChatMessageToast;
            ToastHelper.OnChatMessageToast += ToastHelper_OnChatMessageToast;
        }

        public void OnNavigatedFrom()
        {
            // Subscribe to toast events:
            ToastHelper.OnChatMessageToast -= ToastHelper_OnChatMessageToast;
        }

        #endregion

        #region --Misc Methods (Private)--
        private bool AcvFilter(object o)
        {
            return CHAT_FILTER.Filter(o);
        }

        private void LoadChats()
        {
            // Subscribe to chat and MUC info changed events:
            ChatDBManager.INSTANCE.ChatChanged += INSTANCE_ChatChanged;
            MUCDBManager.INSTANCE.MUCInfoChanged += INSTANCE_MUCInfoChanged;

            ChatDataTemplate selectedChat = null;
            List<ChatDataTemplate> chats = LoadChatsFromDB();

            // Clear list:
            CHATS.Clear();

            // Add chats:
            using (CHATS_ACV.DeferRefresh())
            {
                CHATS.AddRange(chats, false);
            }
            if (SelectedItem is null && selectedChat != null)
            {
                SelectedItem = selectedChat;
            }
        }

        private List<ChatDataTemplate> LoadChatsFromDB()
        {
            List<ChatDataTemplate> list = new List<ChatDataTemplate>();
            foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
            {
                foreach (ChatTable chat in ChatDBManager.INSTANCE.getAllChatsForClient(c.getXMPPAccount().getBareJid()))
                {
                    if (chat.chatType == ChatType.MUC)
                    {
                        list.Add(new ChatDataTemplate()
                        {
                            Chat = chat,
                            Client = c,
                            MucInfo = MUCDBManager.INSTANCE.getMUCInfo(chat.id)
                        });
                    }
                    else
                    {
                        list.Add(new ChatDataTemplate()
                        {
                            Chat = chat,
                            Client = c
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Adds and starts a new chat.
        /// </summary>
        /// <param name="client">Which account/client owns this chat?</param>
        /// <param name="bareJid">The bare JID of the opponent.</param>
        /// <param name="addToRoster">Should the chat get added to the users roster?</param>
        /// <param name="requestSubscription">Request a presence subscription?</param>
        private async Task AddChatAsync(XMPPClient client, string bareJid, bool addToRoster, bool requestSubscription)
        {
            if (client is null || string.IsNullOrEmpty(bareJid))
            {
                Logger.Error("Unable to add chat! client ?= " + (client is null) + " bareJid ?=" + (bareJid is null));
                return;
            }

            await Task.Run(async () =>
            {
                if (addToRoster)
                {
                    await client.GENERAL_COMMAND_HELPER.addToRosterAsync(bareJid);
                }
                if (requestSubscription)
                {
                    await client.GENERAL_COMMAND_HELPER.requestPresenceSubscriptionAsync(bareJid);
                }
                ChatDBManager.INSTANCE.setChat(new ChatTable(bareJid, client.getXMPPAccount().getBareJid())
                {
                    inRoster = addToRoster,
                    subscription = requestSubscription ? "pending" : null
                }, false, true);
            });

        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ToastHelper_OnChatMessageToast(OnChatMessageToastEventArgs args)
        {
            if (!args.Cancel && args.toasterTypeOverride == ChatMessageToasterType.FULL && UiUtils.IsWindowActivated)
            {
                args.toasterTypeOverride = ChatMessageToasterType.REDUCED;
            }
        }

        private void INSTANCE_MUCInfoChanged(MUCDBManager handler, Data_Manager2.Classes.Events.MUCInfoChangedEventArgs args)
        {
            CHATS.UpdateMUCInfo(args.MUC_INFO);
        }

        private async void INSTANCE_ChatChanged(ChatDBManager handler, Data_Manager2.Classes.Events.ChatChangedEventArgs args)
        {
            if (args.REMOVED)
            {
                CHATS.RemoveId(args.CHAT.id);
                args.Cancel = true;
                return;
            }
            else
            {
                if (CHATS.UpdateChat(args.CHAT))
                {
                    args.Cancel = true;
                    return;
                }
            }

            ChatDataTemplate newChat = await Task.Run(() =>
            {
                // Add the new chat to the list of chats:
                foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
                {
                    if (Equals(args.CHAT.userAccountId, c.getXMPPAccount().getBareJid()))
                    {
                        if (args.CHAT.chatType == ChatType.MUC)
                        {
                            return new ChatDataTemplate()
                            {
                                Chat = args.CHAT,
                                Client = c,
                                MucInfo = MUCDBManager.INSTANCE.getMUCInfo(args.CHAT.id)
                            };
                        }
                        else
                        {
                            return new ChatDataTemplate()
                            {
                                Chat = args.CHAT,
                                Client = c
                            };
                        }
                    }
                }
                return null;
            });

            if (!(newChat is null))
            {
                CHATS.Add(newChat);
            }
        }

        #endregion
    }
}
