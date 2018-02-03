using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System;
using System.Threading.Tasks;
using Thread_Save_Components.Classes.Collections;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048_1_0;

namespace Data_Manager2.Classes
{
    public class MUCHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly MUCHandler INSTANCE = new MUCHandler();
        private TSTimedList<MUCJoinHelper> timedList;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/01/2018 Created [Fabian Sauter]
        /// </history>
        public MUCHandler()
        {
            timedList = new TSTimedList<MUCJoinHelper>
            {
                itemTimeoutInMs = (int)TimeSpan.FromSeconds(20).TotalMilliseconds
            };
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void onClientConnected(XMPPClient client)
        {
            ChatManager.INSTANCE.resetMUCEnterState(client.getXMPPAccount().getIdAndDomain());
            enterAllMUCs(client);
        }

        public void onClientDisconnected(XMPPClient client)
        {

        }

        public void onClientDisconnecting(XMPPClient client)
        {
            ChatManager.INSTANCE.resetMUCEnterState(client.getXMPPAccount().getIdAndDomain());
        }

        public async Task enterMUCAsync(ChatTable muc, MUCChatInfoTable info, XMPPClient client)
        {
            MUCJoinHelper helper = new MUCJoinHelper(client, muc, info);
            timedList.addTimed(helper);

            if(info.password != null)
            {
                await helper.enterRoomAsync();
            }
            else
            {
                await helper.enterRoomAsync();
            }
        }

        /// <summary>
        /// Creates a new Task and sends all bookmarks to the server.
        /// </summary>
        /// <param name="client">The XMPPClient which bookmarks should get updated.</param>
        /// <param name="cI">The ConferenceItem that should get updated.</param>
        /// <returns>Returns the Task created by this call.</returns>
        public Task updateBookmarks(XMPPClient client, ConferenceItem cI)
        {
            return client.setBookmarkAsync(cI);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void enterAllMUCs(XMPPClient client)
        {
            Task.Factory.StartNew(async () => {
                foreach (ChatTable muc in ChatManager.INSTANCE.getAllMUCs(client.getXMPPAccount().getIdAndDomain()))
                {
                    MUCChatInfoTable info = ChatManager.INSTANCE.getMUCInfo(muc.id);
                    if(info == null)
                    {
                        info = new MUCChatInfoTable()
                        {
                            chatId = muc.id,
                            enterState = MUCEnterState.DISCONNECTED,
                            nickname = muc.userAccountId,
                            autoEnterRoom = true
                        };
                    }
                    if (info.autoEnterRoom)
                    {
                        await enterMUCAsync(muc, info, client);
                    }
                }
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
