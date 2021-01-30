using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Classes;
using UWPX_UI_Context.Classes.DataTemplates;
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
        public void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ChatDataTemplate chat)
            {
                LoadMembers(chat);
            }
        }

        public void CopyLink(ChatDataTemplate chat)
        {
            Uri uri = UriUtils.buildUri(chat.Chat.chatJabberId, new Dictionary<string, string>() { { "join", null } });
            UiUtils.SetClipboardText(UriUtils.toXmppUriString(uri));
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadMembers(ChatDataTemplate chat)
        {
            Task.Run(async () =>
            {
                MODEL.IsLoading = true;
                List<MucOccupantModel> members = MUCDBManager.INSTANCE.getAllMUCMembers(chat.Chat.id);
                MODEL.MEMBERS.Clear();
                MODEL.MEMBERS.AddRange(members.Select((x) => new MucMemberDataTemplate() { Member = x, Chat = chat }));
                MODEL.MembersFound = members.Count > 0;
                await SharedUtils.CallDispatcherAsync(() => MODEL.MEMBERS_SORTED.Source = MODEL.MEMBERS);
                MODEL.HeaderText = "Members (" + members.Count + ')';
                MODEL.IsLoading = false;
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
