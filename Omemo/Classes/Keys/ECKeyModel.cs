using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Classes;

namespace Omemo.Classes.Keys
{
    public class ECKeyModel: AbstractDataTemplate
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
        public byte[] key
        {
            get => _key;
            set => SetProperty(ref _key, value);
        }
        [NotMapped]
        private byte[] _key;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ECKeyModel() { }

        public ECKeyModel(byte[] key)
        {
            Debug.Assert(!(key is null));
            this.key = key;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            return obj is ECKeyModel ecKey && ecKey.key.SequenceEqual(key);
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }

        /// <summary>
        /// Creates a copy of the object, not including <see cref="id"/>.
        /// </summary>
        public ECKeyModel Clone()
        {
            return new ECKeyModel(key);
        }

        /// <summary>
        /// Removes the current model in the <see cref="DbContext"/> either recursively or not.
        /// </summary>
        /// <param name="ctx">The <see cref="MainDbContext"/> the model should be removed from.</param>
        /// <param name="recursive">Recursively remove the current model.</param>
        public void Remove(DbContext ctx, bool recursive)
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
