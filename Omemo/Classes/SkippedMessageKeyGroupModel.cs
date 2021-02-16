using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Omemo.Classes.Keys;
using Shared.Classes;

namespace Omemo.Classes
{
    public class SkippedMessageKeyGroupModel: AbstractDataTemplate
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
        public ECPubKeyModel dh
        {
            get => _dh;
            set => SetDhProperty(value);
        }
        [NotMapped]
        private ECPubKeyModel _dh;

        // We do not need to subscribe to changed events of this hash set since it's just not interesting.
        [Required]
        public HashSet<SkippedMessageKeyModel> messageKeys { get; set; } = new HashSet<SkippedMessageKeyModel>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SkippedMessageKeyGroupModel() { }

        public SkippedMessageKeyGroupModel(ECPubKeyModel dh)
        {
            this.dh = dh;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetDhProperty(ECPubKeyModel value)
        {
            ECPubKeyModel old = _dh;
            if (SetProperty(ref _dh, value, nameof(dh)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnDhPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnDhPropertyChanged;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public SkippedMessageKeyModel RemoveKey(uint nr)
        {
            SkippedMessageKeyModel skippedMessageKey = GetKey(nr);
            if (!(skippedMessageKey is null))
            {
                messageKeys.Remove(skippedMessageKey);
            }
            return skippedMessageKey;
        }

        public SkippedMessageKeyModel GetKey(uint nr)
        {
            return messageKeys.Where(k => k.nr == nr).FirstOrDefault();
        }

        public void SetKey(uint nr, byte[] mk)
        {
            SkippedMessageKeyModel skippedMessageKey = GetKey(nr);
            if (skippedMessageKey is null)
            {
                messageKeys.Add(new SkippedMessageKeyModel(nr, mk));
            }
            else
            {
                skippedMessageKey.mk = mk;
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnDhPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(dh) + '.' + e.PropertyName);
        }

        #endregion
    }
}
