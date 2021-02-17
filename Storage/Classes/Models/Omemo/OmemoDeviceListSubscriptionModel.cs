using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Storage.Classes.Models.Omemo
{
    public class OmemoDeviceListSubscriptionModel: AbstractDataTemplate
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
        /// The bare JID e.g. 'coven@chat.shakespeare.lit' we received the update from.
        /// </summary>
        [Required]
        public string bareJid
        {
            get => _bareJid;
            set => SetProperty(ref _bareJid, value);
        }
        [NotMapped]
        private string _bareJid;

        /// <summary>
        /// The <see cref="DateTime"/> we received the last update for this entry.
        /// </summary>
        [Required]
        public DateTime lastUpdateReceived
        {
            get => _lastUpdateReceived;
            set => SetProperty(ref _lastUpdateReceived, value);
        }
        [NotMapped]
        private DateTime _lastUpdateReceived;

        /// <summary>
        /// The current state of the subscription.
        /// </summary>
        public OmemoDeviceListSubscriptionState state
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        [NotMapped]
        private OmemoDeviceListSubscriptionState _state;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoDeviceListSubscriptionModel()
        {
            state = OmemoDeviceListSubscriptionState.NONE;
            lastUpdateReceived = DateTime.MinValue;
        }

        public OmemoDeviceListSubscriptionModel(string bareJid) : this()
        {
            this.bareJid = bareJid;
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
