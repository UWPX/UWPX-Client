using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Omemo.Classes.Keys;
using Shared.Classes;
using Shared.Classes.Collections;

namespace Omemo.Classes
{
    public class SkippedMessageKeyGroupsModel: AbstractDataTemplate
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

        public readonly CustomObservableCollection<SkippedMessageKeyGroupModel> MKS = new CustomObservableCollection<SkippedMessageKeyGroupModel>(true);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SkippedMessageKeyGroupsModel()
        {
            MKS.PropertyChanged += OnMKSPropertyChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Adds the given message key (<paramref name="mk"/>) to the stored message keys.
        /// </summary>
        public void SetMessageKey(ECPubKeyModel dhr, uint nr, byte[] mk)
        {
            SkippedMessageKeyGroupModel group = MKS.Where(g => g.dh.Equals(dhr)).FirstOrDefault();
            if (group is null)
            {
                group = new SkippedMessageKeyGroupModel(dhr);
                MKS.Add(group);
            }
            group.SetKey(nr, mk);
        }

        /// <summary>
        /// Tries to find the requested message key. If found it will be returned and removed.
        /// </summary>
        public byte[] GetMessagekey(ECPubKeyModel dhr, uint nr)
        {
            SkippedMessageKeyGroupModel group = MKS.Where(g => g.dh.Equals(dhr)).FirstOrDefault();
            return group?.RemoveKey(nr)?.mk;
        }

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
        private void OnMKSPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(MKS) + '.' + e.PropertyName);
        }

        #endregion
    }
}
