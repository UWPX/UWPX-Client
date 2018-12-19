using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using Microsoft.Toolkit.Uwp.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.Collections;
using UWPX_UI_Context.Classes.DataTemplates;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes.DataContext
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
            this.CHATS_ACV.SortDescriptions.Add(new SortDescription(nameof(ChatDataTemplate.Chat), SortDirection.Descending));
            this.CHAT_FILTER = new ChatFilterDataTemplate(this.CHATS_ACV);

            LoadChats();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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

            // Subscribe to toast events:
            ToastHelper.OnChatMessageToast += ToastHelper_OnChatMessageToast;

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
                foreach (ChatTable chat in ChatDBManager.INSTANCE.getAllChatsForClient(c.getXMPPAccount().getIdAndDomain()))
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

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ToastHelper_OnChatMessageToast(OnChatMessageToastEventArgs args)
        {
        }

        private void INSTANCE_MUCInfoChanged(MUCDBManager handler, Data_Manager2.Classes.Events.MUCInfoChangedEventArgs args)
        {
            CHATS.UpdateMUCInfo(args.MUC_INFO);
        }

        private async void INSTANCE_ChatChanged(ChatDBManager handler, Data_Manager2.Classes.Events.ChatChangedEventArgs args)
        {
            // Backup selected chat:
            ChatDataTemplate selectedChat = SelectedItem;

            if (args.REMOVED)
            {
                CHATS.RemoveId(args.CHAT.id);
                args.Cancel = true;

                // Restore selected chat:
                if (selectedChat != null && !string.Equals(args.CHAT.id, selectedChat.Chat.id))
                {
                    SelectedItem = selectedChat;
                }
                return;
            }
            else
            {
                if (CHATS.UpdateChat(args.CHAT))
                {
                    args.Cancel = true;
                    // Restore selected chat:
                    if (selectedChat != null)
                    {
                        SelectedItem = selectedChat;
                    }
                    return;
                }
            }

            ChatDataTemplate newChat = await Task.Run(() =>
            {
                // Add the new chat to the list of chats:
                foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
                {
                    if (Equals(args.CHAT.userAccountId, c.getXMPPAccount().getIdAndDomain()))
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
            SelectedItem = selectedChat;
        }

        #endregion
    }
}
