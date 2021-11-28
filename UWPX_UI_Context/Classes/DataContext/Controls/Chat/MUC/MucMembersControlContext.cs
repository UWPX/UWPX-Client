using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Manager.Classes.Chat;
using Shared.Classes;
using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC;
using Windows.UI.Xaml;
using XMPP_API.Classes.XmppUri;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC
{
    public class MucMembersControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucMembersControlDataTemplate MODEL = new MucMembersControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue is ChatDataTemplate oldChat)
            {
                if (oldChat.Chat.muc is not null)
                {
                    oldChat.Chat.muc.PropertyChanged -= OnMucPropertyChanged;
                }
            }

            ChatDataTemplate newChat = null;
            if (args.NewValue is ChatDataTemplate tmp)
            {
                newChat = tmp;
                if (newChat.Chat.muc is not null)
                {
                    newChat.Chat.muc.PropertyChanged += OnMucPropertyChanged;
                }
                LoadMembers(newChat);
            }
            MODEL.chat = newChat;
        }

        public void CopyLink(ChatDataTemplate chat)
        {
            Uri uri = UriUtils.buildUri(chat.Chat.bareJid, new Dictionary<string, string>() { { "join", null } });
            UiUtils.SetClipboardText(UriUtils.toXmppUriString(uri));
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadMembers(ChatDataTemplate chat)
        {
            Task.Run(async () =>
            {
                MODEL.IsLoading = true;
                MODEL.MEMBERS.Clear();
                MODEL.MEMBERS.AddRange(chat.Chat.muc.occupants.Select((x) => new MucMemberDataTemplate() { Member = x, Chat = chat }));
                MODEL.MembersFound = chat.Chat.muc.occupants.Count > 0;
                await SharedUtils.CallDispatcherAsync(() => MODEL.MEMBERS_SORTED.Source = MODEL.MEMBERS);
                MODEL.HeaderText = "Members (" + chat.Chat.muc.occupants.Count + ')';
                MODEL.IsLoading = false;
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnMucPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is MucInfoModel && MODEL.chat is not null && (string.Equals(e.PropertyName, nameof(MucInfoModel.occupants)) || string.Equals(e.PropertyName, nameof(MucInfoModel.state))))
            {
                LoadMembers(MODEL.chat);
            }
        }

        #endregion
    }
}
