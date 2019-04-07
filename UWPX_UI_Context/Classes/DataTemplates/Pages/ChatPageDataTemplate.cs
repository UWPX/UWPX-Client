using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using Shared.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.Collections;
using UWPX_UI_Context.Classes.Collections.Toolkit;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class ChatPageDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ObservableChatDictionaryList CHATS = new ObservableChatDictionaryList();
        public readonly ChatFilterDataTemplate CHAT_FILTER;

        public readonly AdvancedCollectionView CHATS_ACV;

        private ChatDataTemplate _SelectedItem;
        public ChatDataTemplate SelectedItem
        {
            get { return _SelectedItem; }
            set { SetProperty(ref _SelectedItem, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatPageDataTemplate()
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
        public void EvaluateOnNavigatedToArgs(object parameter)
        {
            if (parameter is ChatToastActivation args)
            {
                SelectedItem = (ChatDataTemplate)CHATS_ACV.First((x) => x is ChatDataTemplate chat && string.Equals(chat?.Chat.id, args.CHAT_ID));
            }
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
                    // Only show chats with at least 1 chat message or that have been started:
                    if (!chat.isChatActive)
                    {
                        continue;
                    }

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

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void INSTANCE_MUCInfoChanged(MUCDBManager handler, Data_Manager2.Classes.Events.MUCInfoChangedEventArgs args)
        {
            CHATS.UpdateMUCInfo(args.MUC_INFO);
        }

        private async void INSTANCE_ChatChanged(ChatDBManager handler, Data_Manager2.Classes.Events.ChatChangedEventArgs args)
        {
            if (args.REMOVED || !args.CHAT.isChatActive)
            {
                CHATS.RemoveId(args.CHAT.id);
                args.Cancel = args.REMOVED;
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
