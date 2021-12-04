using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Shared.Classes;

namespace Omemo.Classes.Keys
{
    public abstract class AbstractECKeyPairModel: AbstractDataTemplate
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

        public ECPrivKeyModel privKey
        {
            get => _privKey;
            set => SetPrivKeyProperty(value);
        }
        [NotMapped]
        private ECPrivKeyModel _privKey;

        public ECPubKeyModel pubKey
        {
            get => _pubKey;
            set => SetPubKeyProperty(value);
        }
        [NotMapped]
        private ECPubKeyModel _pubKey;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AbstractECKeyPairModel() { }

        public AbstractECKeyPairModel(ECPrivKeyModel privKey, ECPubKeyModel pubKey)
        {
            this.privKey = privKey;
            this.pubKey = pubKey;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetPrivKeyProperty(ECPrivKeyModel value)
        {
            ECPrivKeyModel old = _privKey;
            if (SetProperty(ref _privKey, value, nameof(privKey)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnPrivKeyPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnPrivKeyPropertyChanged;
                }
            }
        }

        private void SetPubKeyProperty(ECPubKeyModel value)
        {
            ECPubKeyModel old = _pubKey;
            if (SetProperty(ref _pubKey, value, nameof(pubKey)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnPubKeyPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnPubKeyPropertyChanged;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            return obj is AbstractECKeyPairModel pair && ((pair.privKey is null && privKey is null) || pair.privKey.Equals(privKey)) && ((pair.pubKey is null && pubKey is null) || pair.pubKey.Equals(pubKey));
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (!(privKey is null))
            {
                hash = privKey.GetHashCode();
            }

            if (!(privKey is null))
            {
                hash ^= pubKey.GetHashCode();
            }

            return hash;
        }

        /// <summary>
        /// Removes the current model in the <see cref="DbContext"/> either recursively or not.
        /// </summary>
        /// <param name="ctx">The <see cref="MainDbContext"/> the model should be removed from.</param>
        /// <param name="recursive">Recursively remove the current model.</param>
        public void Remove(DbContext ctx, bool recursive)
        {
            if (recursive)
            {
                privKey?.Remove(ctx, recursive);
                pubKey?.Remove(ctx, recursive);
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
        private void OnPrivKeyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(privKey) + '.' + e.PropertyName);
        }

        private void OnPubKeyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(pubKey) + '.' + e.PropertyName);
        }

        #endregion
    }
}
