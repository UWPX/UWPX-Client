using Shared.Classes;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.Chat.SpeechBubbles
{
    public class SpeechBubbleMucDirectInvitationControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _Room;
        public string Room
        {
            get => _Room;
            set => SetProperty(ref _Room, value);
        }

        private string _Sender;
        public string Sender
        {
            get => _Sender;
            set => SetProperty(ref _Sender, value);
        }

        private string _Reason;
        public string Reason
        {
            get => _Reason;
            set => SetProperty(ref _Reason, value);
        }

        private string _Header;
        public string Header
        {
            get => _Header;
            set => SetProperty(ref _Header, value);
        }

        private bool _Accepted;
        public bool Accepted
        {
            get => _Accepted;
            set => SetProperty(ref _Accepted, value);
        }

        private bool _Declined;
        public bool Declined
        {
            get => _Declined;
            set => SetProperty(ref _Declined, value);
        }

        private MucDirectInvitationModel _Invite;
        public MucDirectInvitationModel Invite
        {
            get => _Invite;
            set => SetProperty(ref _Invite, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


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
