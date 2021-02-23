using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;

namespace Storage.Classes.Models.Chat
{
    public class MucInfoModel: AbstractModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        [NotMapped]
        private int _id;

        /// <summary>
        /// The <see cref="ChatModel"/> associated with this MUC.
        /// </summary>
        [Required, ForeignKey(nameof(chat) + "Id")]
        public ChatModel chat
        {
            get => _chat;
            set => SetChatProperty(value);
        }
        [NotMapped]
        private ChatModel _chat;

        /// <summary>
        /// The users nickname for the chat.
        /// </summary>
        [Required]
        public string nickname
        {
            get => _nickname;
            set => SetProperty(ref _nickname, value);
        }
        [NotMapped]
        private string _nickname;

        /// <summary>
        /// The current state of the MUC e.g. 'ENTERING'.
        /// </summary>
        [Required]
        public MucState state
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        [NotMapped]
        private MucState _state;

        /// <summary>
        /// True in case we should automatically enter the room as soon as the client is connected.
        /// </summary>
        [Required]
        public bool autoEnterRoom
        {
            get => _autoEnterRoom;
            set => SetProperty(ref _autoEnterRoom, value);
        }
        [NotMapped]
        private bool _autoEnterRoom;

        /// <summary>
        /// The users affiliation to the room.
        /// </summary>
        [Required]
        public MUCAffiliation affiliation
        {
            get => _affiliation;
            set => SetProperty(ref _affiliation, value);
        }
        [NotMapped]
        private MUCAffiliation _affiliation;

        /// <summary>
        /// The users role to the room.
        /// </summary>
        [Required]
        public MUCRole role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }
        [NotMapped]
        private MUCRole _role;

        /// <summary>
        /// An optional friendly name for the room.
        /// </summary>
        public string name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        [NotMapped]
        private string _name;

        /// <summary>
        /// The current room subject.
        /// </summary>
        public string subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }
        [NotMapped]
        private string _subject;

        /// <summary>
        /// The room password.
        /// </summary>
        public string password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        [NotMapped]
        private string _password;

        /// <summary>
        /// A list of all currently known MUC occupants.
        /// </summary>
        [Required]
        public List<MucOccupantModel> occupants
        {
            get => _occupants;
            set => SetProperty(ref _occupants, value);
        }
        [NotMapped]
        private List<MucOccupantModel> _occupants;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucInfoModel()
        {
            occupants = new List<MucOccupantModel>();
        }

        public MucInfoModel(ChatModel chat) : this()
        {
            this.chat = chat;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public Presence GetMucPresence()
        {
            switch (state)
            {
                case MucState.ENTERING:
                case MucState.DISCONNECTING:
                    return Presence.Chat;

                case MucState.ENTERD:
                    return Presence.Online;

                case MucState.ERROR:
                case MucState.KICKED:
                case MucState.BANED:
                    return Presence.Xa;

                case MucState.DISCONNECTED:
                default:
                    return Presence.Unavailable;
            }
        }

        private void SetChatProperty(ChatModel value)
        {
            ChatModel old = _chat;
            if (SetProperty(ref _chat, value, nameof(chat)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnChatPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnChatPropertyChanged;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public ConferenceItem ToConferenceItem()
        {
            return new ConferenceItem
            {
                autoJoin = autoEnterRoom,
                name = name,
                nick = nickname,
                password = password,
                jid = chat.bareJid
            };
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnChatPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Do not propagate own events and prevent circular propagation:
            if (!e.PropertyName.StartsWith(nameof(ChatModel.muc)))
            {
                base.OnPropertyChanged(nameof(chat) + '.' + e.PropertyName);
            }
        }

        #endregion
    }
}
