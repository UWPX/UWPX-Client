﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Storage.Classes.Contexts;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Storage.Classes.Models.Omemo
{
    public class OmemoFingerprintModel: AbstractModel
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

        [Required]
        public DateTime lastSeen
        {
            get => _lastSeen;
            set => SetProperty(ref _lastSeen, value);
        }
        [NotMapped]
        private DateTime _lastSeen;

        [Required]
        public bool trusted
        {
            get => _trusted;
            set => SetProperty(ref _trusted, value);
        }
        [NotMapped]
        private bool _trusted;

        [Required]
        public ECPubKeyModel identityKey
        {
            get => _identityKey;
            set => SetIdentityKeyProperty(value);
        }
        [NotMapped]
        private ECPubKeyModel _identityKey;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoFingerprintModel()
        {
            lastSeen = DateTime.MinValue;
        }

        public OmemoFingerprintModel(OmemoFingerprint fingerprint) : this()
        {
            FromOmemoFingerprint(fingerprint);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetIdentityKeyProperty(ECPubKeyModel value)
        {
            ECPubKeyModel old = _identityKey;
            if (SetProperty(ref _identityKey, value, nameof(identityKey)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnIdentityKeyPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnIdentityKeyPropertyChanged;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public OmemoFingerprint ToOmemoFingerprint(OmemoProtocolAddress address)
        {
            return new OmemoFingerprint(identityKey, address, lastSeen, trusted);
        }

        public void FromOmemoFingerprint(OmemoFingerprint fingerprint)
        {
            lastSeen = fingerprint.lastSeen;
            trusted = fingerprint.trusted;
            identityKey = fingerprint.IDENTITY_KEY;
        }

        public override void Remove(MainDbContext ctx, bool recursive)
        {
            if (recursive)
            {
                identityKey?.Remove(ctx, recursive);
            }
            ctx.Remove(this);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnIdentityKeyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(identityKey) + '.' + e.PropertyName);
        }

        #endregion
    }
}
