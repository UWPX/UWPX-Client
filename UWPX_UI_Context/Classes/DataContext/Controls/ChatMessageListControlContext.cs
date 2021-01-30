using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Toast;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0313;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public class ChatMessageListControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatMessageListControlDataTemplate MODEL = new ChatMessageListControlDataTemplate();

        private CancellationTokenSource loadMoreMessagesToken = null;
        private Task<List<ChatMessageDataTemplate>> loadMoreMessagesTask = null;
        private readonly SemaphoreSlim LOAD_MORE_MESSAGES_SEMA = new SemaphoreSlim(1);

        private const int MAX_MESSAGES_PER_REQUEST = 15;
        private bool mamRequested = false;
        private bool loadedAllLocalMessages = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(ChatDataTemplate oldChat, ChatDataTemplate newChat)
        {
            if (!(oldChat is null))
            {
                oldChat.PropertyChanged -= Chat_PropertyChanged;
                oldChat.ChatMessageChanged -= Chat_ChatMessageChanged;
                oldChat.NewChatMessage -= Chat_NewChatMessage;
            }

            if (!(newChat is null))
            {
                newChat.PropertyChanged += Chat_PropertyChanged;
                newChat.ChatMessageChanged += Chat_ChatMessageChanged;
                newChat.NewChatMessage += Chat_NewChatMessage;

                loadedAllLocalMessages = false;
                mamRequested = false;
            }

            MODEL.UpdateView(oldChat, newChat);
        }

        private async Task<List<ChatMessageDataTemplate>> LoadMoreMessagesInTaskAsync()
        {
            List<ChatMessageDataTemplate> tmpMsgs;
            if (loadedAllLocalMessages)
            {
                if (!mamRequested)
                {
                    tmpMsgs = await LoadMoreMamMessagesAsync();
                    MODEL.HasMoreMessages = tmpMsgs.Count > 0;
                    return tmpMsgs;
                }
                else
                {
                    tmpMsgs = new List<ChatMessageDataTemplate>();
                }
            }
            else
            {
                tmpMsgs = LoadMoreLocalMessages();
                loadedAllLocalMessages = tmpMsgs.Count < MAX_MESSAGES_PER_REQUEST;
                if (tmpMsgs.Count <= 0)
                {
                    return await LoadMoreMessagesInTaskAsync();
                }
            }

            MODEL.HasMoreMessages = (!loadedAllLocalMessages) || (MODEL.Chat.Chat.chatType == ChatType.MUC && !mamRequested);
            return tmpMsgs;
        }

        public Task LoadMoreMessagesAsync()
        {
            return Task.Run(async () =>
            {
                if (!(loadMoreMessagesTask is null))
                {
                    if (!(loadMoreMessagesToken is null) && !loadMoreMessagesToken.IsCancellationRequested)
                    {
                        loadMoreMessagesToken.Cancel();
                    }
                    await loadMoreMessagesTask;
                }

                await LOAD_MORE_MESSAGES_SEMA.WaitAsync();
                MODEL.IsLoading = true;
                loadMoreMessagesToken = new CancellationTokenSource();

                loadMoreMessagesTask = Task.Run(async () =>
                {
                    return await LoadMoreMessagesInTaskAsync();
                });

                List<ChatMessageDataTemplate> msgs = await loadMoreMessagesTask;
                if (!loadMoreMessagesToken.IsCancellationRequested && msgs.Count > 0)
                {
                    ToastHelper.removeToastGroup(MODEL.Chat.Chat.id);

                    await MODEL.InsertChatMessagesAsync(msgs);
                }
                MODEL.IsLoading = false;
                LOAD_MORE_MESSAGES_SEMA.Release();
            });
        }

        #endregion

        #region --Misc Methods (Private)--
        private QueryFilter GenerateMamQueryFilter()
        {
            QueryFilter filter = new QueryFilter();
            if (MODEL.CHAT_MESSAGES.Count > 0 && !string.IsNullOrEmpty(MODEL.CHAT_MESSAGES[0].Message.stableId))
            {
                if (MODEL.Chat.Client.connection.DISCO_HELPER.HasFeature(Consts.XML_XEP_0313_EXTENDED_NAMESPACE, MODEL.Chat.Client.getXMPPAccount().getBareJid()))
                {
                    // Only extended MAM supports setting the 'after-id' property.
                    // Reference: https://xmpp.org/extensions/xep-0313.html#support
                    filter.BeforeId(MODEL.CHAT_MESSAGES[0].Message.stableId);
                }
                else
                {
                    filter.End(MODEL.CHAT_MESSAGES[0].Message.date);
                }
            }
            filter.With(MODEL.Chat.Chat.chatJabberId);
            return filter;
        }

        private async Task<List<ChatMessageDataTemplate>> LoadMoreMamMessagesAsync()
        {
            Logger.Info("Requesting MAM messages for \"" + MODEL.Chat.Chat.id + "\".");
            QueryFilter filter = GenerateMamQueryFilter();
            // For MUCs ask the MUC server and for everything else ask our own server for messages:
            string target = MODEL.Chat.Chat.chatType == ChatType.CHAT ? MODEL.Chat.Client.getXMPPAccount().getBareJid() : MODEL.Chat.Chat.chatJabberId;
            MessageResponseHelperResult<MamResult> result = await MODEL.Chat.Client.GENERAL_COMMAND_HELPER.requestMamAsync(filter, target);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                Logger.Info("Found " + result.RESULT.COUNT + " MAM messages for \"" + MODEL.Chat.Chat.id + "\".");
                Logger.Debug("MAM result: " + result.RESULT.ToString());

                mamRequested = result.RESULT.COMPLETE;

                List<ChatMessageDataTemplate> msgs = new List<ChatMessageDataTemplate>();
                foreach (QueryArchiveResultMessage msg in result.RESULT.RESULTS)
                {
                    ChatMessageDataTemplate tmp = new ChatMessageDataTemplate
                    {
                        Message = new ChatMessageModel(msg.MESSAGE, MODEL.Chat.Chat),
                        Chat = MODEL.Chat.Chat,
                        MUC = MODEL.Chat.MucInfo
                    };
                    msgs.Add(tmp);
                    await ChatDBManager.INSTANCE.setChatMessageAsync(tmp.Message, false, false);
                }
                return msgs;
            }
            else
            {
                Logger.Error("Failed to load more MAM messages for \"" + MODEL.Chat.Chat.id + "\". Failed with: " + result.STATE);
                return new List<ChatMessageDataTemplate>();
            }
        }

        private List<ChatMessageDataTemplate> LoadMoreLocalMessages()
        {
            List<ChatMessageDataTemplate> tmpMsgs = new List<ChatMessageDataTemplate>();
            IList<ChatMessageModel> list = ChatDBManager.INSTANCE.getNextNChatMessages(MODEL.Chat.Chat.id, MODEL.GetLastMessageId(), MAX_MESSAGES_PER_REQUEST + 1); // Load one item more than we use laster to determine if there are more items available
            for (int i = 0; i < list.Count && i < MAX_MESSAGES_PER_REQUEST && !loadMoreMessagesToken.IsCancellationRequested; i++)
            {
                tmpMsgs.Add(new ChatMessageDataTemplate
                {
                    Message = list[i],
                    Chat = MODEL.Chat.Chat,
                    MUC = MODEL.Chat.MucInfo
                });
            }
            return tmpMsgs;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Chat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ChatDataTemplate chat)
            {
                UpdateView(MODEL.Chat, chat);
            }
        }

        private async void Chat_NewChatMessage(ChatDataTemplate chat, NewChatMessageEventArgs args)
        {
            if (!MODEL.IsDummy)
            {
                await MODEL.OnNewChatMessageAsync(args.MESSAGE, chat.Chat, chat.MucInfo);
                if (args.MESSAGE.state == MessageState.UNREAD)
                {
                    // Mark message as read and update the badge notification count:
                    await Task.Run(() =>
                    {
                        ChatDBManager.INSTANCE.markMessageAsRead(args.MESSAGE);
                        ToastHelper.UpdateBadgeNumber();
                    });
                }
            }
        }

        private async void Chat_ChatMessageChanged(ChatDataTemplate chat, ChatMessageChangedEventArgs args)
        {
            if (!MODEL.IsDummy)
            {
                await MODEL.OnChatMessageChangedAsync(args.MESSAGE, args.REMOVED);
            }
        }

        #endregion
    }
}
