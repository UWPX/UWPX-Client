﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Storage.Classes.Contexts;

namespace Storage.Classes.Models.Account
{
    /// <summary>
    /// Represents the current state of the XEP 0357 push integration.
    /// </summary>
    public class PushAccountModel: AbstractModel
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
        /// The name of the node, where our server should publish to.
        /// </summary>
        public string node
        {
            get => _node;
            set => SetProperty(ref _node, value);
        }
        [NotMapped]
        private string _node;

        /// <summary>
        /// The secret for publishing.
        /// </summary>
        public string secret
        {
            get => _secret;
            set => SetProperty(ref _secret, value);
        }
        [NotMapped]
        private string _secret;

        /// <summary>
        /// The bare Jabber ID e.g. 'pushServer@chat.shakespeare.lit' of the push server, where our server should publish updates for us to.
        /// </summary>
        public string bareJid
        {
            get => _bareJid;
            set => SetProperty(ref _bareJid, value);
        }
        [NotMapped]
        private string _bareJid;

        /// <summary>
        /// Represents the current state of enabling/disabling push at the XMPP server.
        /// </summary>
        [Required]
        public PushState state
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        [NotMapped]
        private PushState _state;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public PushAccountModel()
        {
            state = PushState.DISABLED;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override void Remove(MainDbContext ctx, bool recursive)
        {
            ctx.Remove(this);
        }

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
