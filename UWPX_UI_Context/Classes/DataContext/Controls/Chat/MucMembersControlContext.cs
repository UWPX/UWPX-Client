using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat
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
            if (e.NewValue is ChatTable chat)
            {
                LoadMembers(chat);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadMembers(ChatTable chat)
        {
            Task.Run(() =>
            {
                List<MUCOccupantTable> members = MUCDBManager.INSTANCE.getAllMUCMembers(chat.id);
                MODEL.MEMBERS.Clear();
                MODEL.MEMBERS.AddRange(members);
                MODEL.MembersFound = members.Count > 0;
                MODEL.HeaderText = "Members (" + members.Count + ')';
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
