using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Storage.Classes.Models.Chat
{
    public class ChatMessageModel: AbstractModel
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
        /// The unique and stable stanza ID (XEP-0359) used for MAM (XEP-0313).
        /// </summary>
        [Required]
        public string stableId
        {
            get => _stableId;
            set => SetProperty(ref _stableId, value);
        }
        [NotMapped]
        private string _stableId;

        [Required]
        public int chatId
        {
            get => _chatId;
            set => SetProperty(ref _chatId, value);
        }
        [NotMapped]
        private int _chatId;

        /// <summary>
        /// The type of the message e.g. 'error', 'chat', 'groupchat', ...
        /// </summary>
        [Required]
        public string type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }
        [NotMapped]
        private string _type;

        /// <summary>
        /// The actual chat message.
        /// </summary>
        [Required]
        public string message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        [NotMapped]
        private string _message;

        /// <summary>
        /// Represents the senders bare JID.
        /// </summary>
        [Required]
        public string fromBareJid
        {
            get => _fromBareJid;
            set => SetProperty(ref _fromBareJid, value);
        }
        [NotMapped]
        private string _fromBareJid;

        /// <summary>
        /// The nickname of the sender. Useful for group chats e.g. MUC or MIX.
        /// </summary>
        public string fromNickname
        {
            get => _fromNickname;
            set => SetProperty(ref _fromNickname, value);
        }
        [NotMapped]
        private string _fromNickname;

        /// <summary>
        /// The date and time the message got send.
        /// </summary>
        [Required]
        public DateTime date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }
        [NotMapped]
        private DateTime _date = DateTime.MinValue;

        /// <summary>
        /// The sate of the chat message e.g. send, read, sending, ...
        /// </summary>
        [Required]
        public MessageState state
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        [NotMapped]
        private MessageState _state;

        /// <summary>
        /// True in case the message represents an image.
        /// </summary>
        [Required]
        public bool isImage
        {
            get => _isImage;
            set => SetProperty(ref _isImage, value);
        }
        [NotMapped]
        private bool _isImage;

        /// <summary>
        /// True in case this message is a carbon copy (XEP-0280).
        /// </summary>
        [Required]
        public bool isCC
        {
            get => _isCC;
            set => SetProperty(ref _isCC, value);
        }
        [NotMapped]
        private bool _isCC;

        /// <summary>
        /// True in case the message has been send or received as an encrypted message.
        /// </summary>
        [Required]
        public bool isEncrypted
        {
            get => _isEncrypted;
            set => SetProperty(ref _isEncrypted, value);
        }
        [NotMapped]
        private bool _isEncrypted;

        /// <summary>
        /// True in case the message been marked as a favorite.
        /// </summary>
        [Required]
        public bool isFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }
        [NotMapped]
        private bool _isFavorite;

        /// <summary>
        /// In case <see cref="isImage"/> is true, this holds the image model representing the download status and path to the image.
        /// </summary>
        public ChatMessageImageModel image
        {
            get => _image;
            set => SetImageProperty(value);
        }
        [NotMapped]
        private ChatMessageImageModel _image;

        /// <summary>
        /// True in case the message is a dummy message used for the personalize settings page chat preview.
        /// </summary>
        [NotMapped]
        public bool isDummyMessage { get; set; }

        private static readonly Regex IMAGE_URL_REGEX = new Regex(@"^http[s]?:\/\/(([^\/:\.[:space:]]+(\.[^\/:\.[:space:]]+)*)|([0-9](\.[0-9]{3})))(:[0-9]+)?((\/[^?#[:space:]]+)(\?[^#[:space:]]+)?(\#.+)?)?\.(?:jpe?g|gif|png)$");

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMessageModel() { }

        public ChatMessageModel(MessageMessage msg, ChatModel chat)
        {
            fromBareJid = XMPP_API.Classes.Utils.getBareJidFromFullJid(msg.getFrom());
            fromNickname = msg.FROM_NICK;
            stableId = msg.ID;
            chatId = chat.id;
            type = msg.TYPE;
            message = msg.MESSAGE;
            date = msg.delay;
            isDummyMessage = false;
            if (date.Equals(DateTime.MinValue))
            {
                date = DateTime.Now;
            }
            state = msg.CC_TYPE == CarbonCopyType.SENT ? MessageState.SEND : MessageState.UNREAD;
            isImage = IsImageUrl(msg.MESSAGE);
            if (isImage)
            {
                image = new ChatMessageImageModel(msg.MESSAGE);
            }
            isCC = msg.CC_TYPE != CarbonCopyType.NONE;
            isEncrypted = msg is OmemoEncryptedMessage;
            isFavorite = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Checks if the given message text matches an image URL.
        /// </summary>
        /// <param name="msg">The message to check.</param>
        /// <returns>True in case the given message represents an image URL.</returns>
        private static bool IsImageUrl(string msg)
        {
            return !(msg is null) && IMAGE_URL_REGEX.IsMatch(msg.ToLowerInvariant());
        }

        private void SetImageProperty(ChatMessageImageModel value)
        {
            ChatMessageImageModel old = _image;
            if (SetProperty(ref _image, value, nameof(image)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnImagePropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnImagePropertyChanged;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public MessageMessage ToMessageMessage(string fromFullJid, string toBareJid)
        {
            MessageMessage msg;
            switch (type)
            {
                case MessageMessage.TYPE_GROUPCHAT:
                    msg = new MessageMessage(fromFullJid, toBareJid, message, type, fromNickname, true);
                    break;

                default:
                    if (isEncrypted)
                    {
                        msg = new OmemoEncryptedMessage(fromFullJid, toBareJid, message, type, true);
                    }
                    else
                    {
                        msg = new MessageMessage(fromFullJid, toBareJid, message, type, true);
                    }
                    break;
            }

            msg.addDelay(date);
            msg.chatMessageId = id;
            return msg;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnImagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(image) + '.' + e.PropertyName);
        }

        #endregion
    }
}
