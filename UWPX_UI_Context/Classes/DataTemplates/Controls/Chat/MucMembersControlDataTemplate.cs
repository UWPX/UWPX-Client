using Data_Manager2.Classes.DBTables;
using Shared.Classes;
using Shared.Classes.Collections;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat
{
    public class MucMembersControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _MembersFound;
        public bool MembersFound
        {
            get => _MembersFound;
            set => SetProperty(ref _MembersFound, value);
        }

        private string _HeaderText;
        public string HeaderText
        {
            get => _HeaderText;
            set => SetProperty(ref _HeaderText, value);
        }

        public readonly CustomObservableCollection<MUCOccupantTable> MEMBERS = new CustomObservableCollection<MUCOccupantTable>(true);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucMembersControlDataTemplate()
        {
            HeaderText = "Members (0)";
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
