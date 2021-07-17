using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Logging;
using Storage.Classes;
using Storage.Classes.Models.Chat;
using Windows.System.Threading;
using XMPP_API.Classes.Network.XML.Messages.XEP_0085;

namespace Manager.Classes.Chat
{
    /// <summary>
    /// Helper for managing the chat state for a chat.
    /// <para/>
    /// https://xmpp.org/extensions/xep-0085.html
    /// </summary>
    public class ChatStateHelper: IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatModel CHAT;
        public readonly Client CLIENT;

        private ChatState state = ChatState.INACTIVE;
        private DateTime lasKeyDown = DateTime.MinValue;

        private ThreadPoolTimer ownTimer;
        private ThreadPoolTimer otherTimer;
        private bool disposed = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatStateHelper(ChatModel chat, Client client)
        {
            CHAT = chat;
            CLIENT = client;
            CHAT.PropertyChanged += OnChatPropertyChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Sets the chat state to <see cref="ChatState.ACTIVE"/> and sends the updated state to the participant.
        /// <para/>
        /// User is actively participating in the chat session.
        /// <para/>
        /// User accepts an initial content message, sends a content message, gives focus to the chat session interface (perhaps after being inactive), or is otherwise paying attention to the conversation.
        /// </summary>
        public void SetActive()
        {
            SetState(ChatState.ACTIVE);
        }

        /// <summary>
        /// Sets the chat state to <see cref="ChatState.INACTIVE"/> and sends the updated state to the participant.
        /// <para/>
        /// User has not been actively participating in the chat session.
        /// <para/>
        /// User has not interacted with the chat session interface for an intermediate period of time (e.g., 2 minutes).
        /// </summary>
        public void SetInactive()
        {
            SetState(ChatState.INACTIVE);
        }

        /// <summary>
        /// Sets the chat state to <see cref="ChatState.GONE"/> and sends the updated state to the participant.
        /// <para/>
        /// User has effectively ended their participation in the chat session.
        /// <para/>
        /// User has not interacted with the chat session interface, system, or device for a relatively long period of time (e.g., 10 minutes).
        /// </summary>
        public void SetGone()
        {
            SetState(ChatState.GONE);
        }

        /// <summary>
        /// Sets the chat state to <see cref="ChatState.COMPOSING"/> and sends the updated state to the participant.
        /// <para/>
        /// User is composing a message.
        /// <para/>
        /// User is actively interacting with a message input interface specific to this chat session (e.g., by typing in the input area of a chat window).
        /// </summary>
        public void SetComposing()
        {
            SetState(ChatState.COMPOSING);
        }

        /// <summary>
        /// Sets the chat state to <see cref="ChatState.PAUSED"/> and sends the updated state to the participant.
        /// <para/>
        /// User had been composing but now has stopped.
        /// <para/>
        /// User was composing but has not interacted with the message input interface for a short period of time (e.g., 30 seconds).
        /// </summary>
        public void SetPaused()
        {
            SetState(ChatState.PAUSED);
        }

        /// <summary>
        /// Should be called once the user pressed a key inside the message box to keep the typing indicator up to date for the other participants.
        /// </summary>
        public void OnMessageBoxKeyDown()
        {
            lasKeyDown = DateTime.Now;
            if (state != ChatState.COMPOSING)
            {
                SetState(ChatState.COMPOSING);
            }
        }

        public void Dispose()
        {
            disposed = true;
            CancelTimer(ref ownTimer);
            CancelTimer(ref otherTimer);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void SetState(ChatState state)
        {
            if (state == ChatState.COMPOSING)
            {
                StartOwnTimer(TimeSpan.FromSeconds(30));
            }
            else if (state == ChatState.ACTIVE || state == ChatState.PAUSED)
            {
                StartOwnTimer(TimeSpan.FromSeconds(90));
            }
            else
            {
                CancelTimer(ref ownTimer);
            }

            if (this.state != state)
            {
                ChatState oldState = this.state;
                this.state = state;
                SendUpdatedState(state, oldState);
            }
        }

        private void CancelTimer(ref ThreadPoolTimer timer)
        {
            if (!(timer is null))
            {
                timer.Cancel();
                timer = null;
            }
        }

        private void StartOwnTimer(TimeSpan timeout)
        {
            if (!(ownTimer is null))
            {
                CancelTimer(ref ownTimer);
            }
            ownTimer = ThreadPoolTimer.CreateTimer(onOwnTimeout, timeout);
        }

        private void StartOtherTimer(TimeSpan timeout)
        {
            if (!(otherTimer is null))
            {
                CancelTimer(ref otherTimer);
            }
            otherTimer = ThreadPoolTimer.CreateTimer(onOtherTimeout, timeout);
        }

        private void SendUpdatedState(ChatState state)
        {
            SendUpdatedState(state, state);
        }

        private void SendUpdatedState(ChatState newState, ChatState oldState)
        {
            // Do not send chat state to contacts without a presence subscription or MUCs:
            if (CHAT.chatType != ChatType.CHAT || Settings.GetSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE) || (!string.Equals(CHAT.subscription, "both") && !string.Equals(CHAT.subscription, "from")))
            {
                return;
            }

            Task.Run(async () => await CLIENT.xmppClient.GENERAL_COMMAND_HELPER.sendChatStateAsync(CHAT.bareJid, state));
            if (newState == oldState)
            {
                Logger.Debug($"Own chat state ({state}) for chat '{CHAT.bareJid}' send.");
            }
            else
            {
                Logger.Debug($"Own chat state for chat '{CHAT.bareJid}' updated from {oldState} to {state}.");
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnChatPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(ChatModel.chatState)))
            {
                switch (CHAT.chatState)
                {
                    case ChatState.COMPOSING:
                        StartOtherTimer(TimeSpan.FromSeconds(30));
                        break;
                    case ChatState.ACTIVE:
                    case ChatState.PAUSED:
                        StartOtherTimer(TimeSpan.FromSeconds(90));
                        break;
                    case ChatState.INACTIVE:
                    case ChatState.GONE:
                    case ChatState.UNKNOWN:
                    default:
                        break;
                }
            }
        }

        private void onOwnTimeout(ThreadPoolTimer timer)
        {
            if (timer is null || disposed)
            {
                return;
            }
            ChatState oldState = state;
            if (state == ChatState.COMPOSING)
            {
                // Send composing while we are typing every 30 seconds:
                double secSinceLastKeyDown = (DateTime.Now - lasKeyDown).TotalSeconds;
                if (secSinceLastKeyDown < 28)
                {
                    SendUpdatedState(ChatState.COMPOSING);
                    StartOwnTimer(TimeSpan.FromSeconds(30 - secSinceLastKeyDown));
                }

                SetState(ChatState.PAUSED);
            }
            else if (CHAT.chatState == ChatState.ACTIVE || CHAT.chatState == ChatState.PAUSED)
            {
                SetState(ChatState.INACTIVE);
            }
            if (oldState != CHAT.chatState)
            {
                Logger.Debug($"Automated update of the own chat state from {oldState} to {CHAT.chatState} for chat '{CHAT.bareJid}'.");
            }
        }

        private void onOtherTimeout(ThreadPoolTimer timer)
        {
            if (timer is null || disposed)
            {
                return;
            }
            ChatState oldState = CHAT.chatState;
            if (CHAT.chatState == ChatState.COMPOSING && (DateTime.Now - CHAT.lastChatStateUpdate).Seconds > 30)
            {
                CHAT.chatState = ChatState.PAUSED;
            }
            else if ((CHAT.chatState == ChatState.ACTIVE || CHAT.chatState == ChatState.PAUSED) && (DateTime.Now - CHAT.lastChatStateUpdate).Seconds > 90)
            {
                CHAT.chatState = ChatState.INACTIVE;
            }
            if (oldState != CHAT.chatState)
            {
                Logger.Debug($"Automated update of chat state from {oldState} to {CHAT.chatState} for '{CHAT.bareJid}'.");
            }
        }

        #endregion
    }
}
