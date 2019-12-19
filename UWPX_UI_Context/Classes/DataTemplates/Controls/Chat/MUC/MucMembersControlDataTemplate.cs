using Shared.Classes;
using Shared.Classes.Collections;
using UWPX_UI_Context.Classes.Collections.Toolkit;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.MUC
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

        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }

        public readonly CustomObservableCollection<MucMemberDataTemplate> MEMBERS = new CustomObservableCollection<MucMemberDataTemplate>(true);
        public readonly AdvancedCollectionView MEMBERS_SORTED;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucMembersControlDataTemplate()
        {
            HeaderText = "Members (0)";
            MEMBERS_SORTED = new AdvancedCollectionView();
            MEMBERS_SORTED.SortDescriptions.Add(new Microsoft.Toolkit.Uwp.UI.SortDescription(Microsoft.Toolkit.Uwp.UI.SortDirection.Ascending));
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
