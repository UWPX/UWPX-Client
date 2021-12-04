using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Classes;

namespace Omemo.Classes.Keys
{
    public class SignedPreKeyModel: AbstractDataTemplate
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

        [Required, ForeignKey(nameof(preKey) + "Id")]
        public PreKeyModel preKey
        {
            get => _preKey;
            set => SetPreKeyProperty(value);
        }
        [NotMapped]
        private PreKeyModel _preKey;

        [Required]
        public byte[] signature
        {
            get => _signature;
            set => SetProperty(ref _signature, value);
        }
        [NotMapped]
        private byte[] _signature;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SignedPreKeyModel() { }

        public SignedPreKeyModel(PreKeyModel preKey, byte[] signature)
        {
            this.preKey = preKey;
            this.signature = signature;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetPreKeyProperty(PreKeyModel value)
        {
            PreKeyModel old = _preKey;
            if (SetProperty(ref _preKey, value, nameof(preKey)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnPreKeyPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnPreKeyPropertyChanged;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            return obj is SignedPreKeyModel signedPreKey && signedPreKey.preKey.Equals(preKey) && signedPreKey.signature.SequenceEqual(signature);
        }

        public override int GetHashCode()
        {
            return preKey.GetHashCode() ^ signature.GetHashCode();
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
                preKey?.Remove(ctx, recursive);
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
        private void OnPreKeyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(preKey) + '.' + e.PropertyName);
        }

        #endregion
    }
}
