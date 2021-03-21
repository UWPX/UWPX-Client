using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Omemo.Classes.Keys;
using Omemo.Classes.Messages;
using Shared.Classes;

namespace Omemo.Classes
{
    public class OmemoSessionModel: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// DB key for storing a session in a DB.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        [NotMapped]
        private int _id;

        /// <summary>
        /// Key pair for the sending ratchet.
        /// </summary>
        public AbstractECKeyPairModel dhS
        {
            get => _dhS;
            set => SetDhsProperty(value);
        }
        [NotMapped]
        private AbstractECKeyPairModel _dhS;

        /// <summary>
        /// Key pair for the receiving ratchet.
        /// </summary>
        public AbstractECKeyPairModel dhR
        {
            get => _dhR;
            set => SetDhrProperty(value);
        }
        [NotMapped]
        private AbstractECKeyPairModel _dhR;

        /// <summary>
        /// Ephemeral key used for initiating this session. 
        /// </summary>
        [Required]
        public ECPubKeyModel ek
        {
            get => _ek;
            set => SetEkProperty(value);
        }
        [NotMapped]
        private ECPubKeyModel _ek;

        /// <summary>
        /// 32 byte root key for encryption.
        /// </summary>
        [Required]
        public byte[] rk
        {
            get => _rk;
            set => SetProperty(ref _rk, value);
        }
        [NotMapped]
        private byte[] _rk;

        /// <summary>
        /// 32 byte Chain Keys for sending.
        /// </summary>
        public byte[] ckS
        {
            get => _ckS;
            set => SetProperty(ref _ckS, value);
        }
        [NotMapped]
        private byte[] _ckS;

        /// <summary>
        /// 32 byte Chain Keys for receiving.
        /// </summary>
        public byte[] ckR
        {
            get => _ckR;
            set => SetProperty(ref _ckR, value);
        }
        [NotMapped]
        private byte[] _ckR;

        /// <summary>
        /// Message numbers for sending.
        /// </summary>
        [Required]
        public uint nS
        {
            get => _nS;
            set => SetProperty(ref _nS, value);
        }
        [NotMapped]
        private uint _nS;

        /// <summary>
        /// Message numbers for receiving.
        /// </summary>
        [Required]
        public uint nR
        {
            get => _nR;
            set => SetProperty(ref _nR, value);
        }
        [NotMapped]
        private uint _nR;

        /// <summary>
        /// Number of messages in previous sending chain.
        /// </summary>
        [Required]
        public uint pn
        {
            get => _pn;
            set => SetProperty(ref _pn, value);
        }
        [NotMapped]
        private uint _pn;

        /// <summary>
        /// Skipped-over message keys, indexed by ratchet <see cref="ECPubKeyModel"/> and message number. Raises an exception if too many elements are stored.
        /// </summary>
        [Required]
        public readonly SkippedMessageKeyGroupsModel MK_SKIPPED = new SkippedMessageKeyGroupsModel();

        /// <summary>
        /// The id of the PreKey used to create establish this session.
        /// </summary>
        public uint preKeyId
        {
            get => _preKeyId;
            set => SetProperty(ref _preKeyId, value);
        }
        [NotMapped]
        private uint _preKeyId;

        /// <summary>
        /// The id of the signed PreKey used to create establish this session.
        /// </summary>
        public uint signedPreKeyId
        {
            get => _signedPreKeyId;
            set => SetProperty(ref _signedPreKeyId, value);
        }
        [NotMapped]
        private uint _signedPreKeyId;

        /// <summary>
        /// The associated data is created by concatenating the IdentityKeys of Alice and Bob.
        /// <para/>
        /// AD = Encode(IK_A) || Encode(IK_B).
        /// <para/>
        /// Alice is the party that actively initiated the key exchange, while Bob is the party that passively accepted the key exchange.
        /// </summary>
        [Required]
        public byte[] assData
        {
            get => _assData;
            set => SetProperty(ref _assData, value);
        }
        [NotMapped]
        private byte[] _assData;

        /// <summary>
        /// Max number of skipped message keys to prevent DOS attacks.
        /// </summary>
        public const uint MAX_SKIP = 100;


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Creates a new <see cref="OmemoSessionModel"/> for sending a new message and initiating a new key exchange.
        /// </summary>
        /// <param name="receiverBundle">The <see cref="Bundle"/> of the receiving end.</param>
        /// /// <param name="receiverPreKeyIndex">The index of the <see cref="Bundle.preKeys"/> to use.</param>
        /// <param name="senderIdentityKeyPair">Our own <see cref="IdentityKeyPairModel"/>.</param>
        public OmemoSessionModel(Bundle receiverBundle, int receiverPreKeyIndex, IdentityKeyPairModel senderIdentityKeyPair)
        {
            EphemeralKeyPairModel ephemeralKeyPair = KeyHelper.GenerateEphemeralKeyPair();
            byte[] sk = CryptoUtils.GenerateSenderSessionKey(senderIdentityKeyPair.privKey, ephemeralKeyPair.privKey, receiverBundle.identityKey, receiverBundle.signedPreKey, receiverBundle.preKeys[receiverPreKeyIndex].pubKey);

            // We are only interested in the public key and discard the private key.
            ek = ephemeralKeyPair.pubKey;
            dhS = KeyHelper.GenerateKeyPair();
            dhR = new GenericECKeyPairModel(null, receiverBundle.identityKey);
            Tuple<byte[], byte[]> tmp = LibSignalUtils.KDF_RK(sk, CryptoUtils.SharedSecret(dhS.privKey, dhR.pubKey));
            rk = tmp.Item1;
            ckS = tmp.Item2;
            signedPreKeyId = receiverBundle.signedPreKeyId;
            preKeyId = receiverBundle.preKeys[receiverPreKeyIndex].keyId;
            assData = CryptoUtils.Concat(senderIdentityKeyPair.pubKey.key, receiverBundle.identityKey.key);

            MK_SKIPPED.PropertyChanged += OnMK_SKIPPEDPropertyChanged;
        }

        /// <summary>
        /// Creates a new <see cref="OmemoSessionModel"/> for receiving a new message and accepting a new key exchange.
        /// </summary>
        /// <param name="receiverIdentityKey">The receivers <see cref="IdentityKeyPairModel"/>.</param>
        /// <param name="receiverSignedPreKey">The receivers <see cref="SignedPreKeyModel"/>.</param>
        /// <param name="receiverPreKey">The receivers <see cref="PreKeyModel"/> selected by the sender.</param>
        /// <param name="keyExchangeMsg">The received <see cref="OmemoKeyExchangeMessage"/>.</param>
        public OmemoSessionModel(IdentityKeyPairModel receiverIdentityKey, SignedPreKeyModel receiverSignedPreKey, PreKeyModel receiverPreKey, OmemoKeyExchangeMessage keyExchangeMsg)
        {
            dhS = receiverIdentityKey;
            rk = CryptoUtils.GenerateReceiverSessionKey(keyExchangeMsg.IK, keyExchangeMsg.EK, receiverIdentityKey.privKey, receiverSignedPreKey.preKey.privKey, receiverPreKey.privKey);
            assData = CryptoUtils.Concat(keyExchangeMsg.IK.key, receiverIdentityKey.pubKey.key);
            ek = keyExchangeMsg.EK;

            MK_SKIPPED.PropertyChanged += OnMK_SKIPPEDPropertyChanged;
        }

        public OmemoSessionModel()
        {
            MK_SKIPPED.PropertyChanged += OnMK_SKIPPEDPropertyChanged;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetEkProperty(ECPubKeyModel value)
        {
            ECPubKeyModel old = _ek;
            if (SetProperty(ref _ek, value, nameof(ek)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnEkPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnEkPropertyChanged;
                }
            }
        }

        private void SetDhrProperty(AbstractECKeyPairModel value)
        {
            AbstractECKeyPairModel old = _dhR;
            if (SetProperty(ref _dhR, value, nameof(dhR)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnDhrPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnDhrPropertyChanged;
                }
            }
        }

        private void SetDhsProperty(AbstractECKeyPairModel value)
        {
            AbstractECKeyPairModel old = _dhS;
            if (SetProperty(ref _dhS, value, nameof(dhS)))
            {
                if (!(old is null))
                {
                    old.PropertyChanged -= OnDhsPropertyChanged;
                }
                if (!(value is null))
                {
                    value.PropertyChanged += OnDhsPropertyChanged;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Initiates the DH ratchet for decrypting the first message.
        /// </summary>
        /// <param name="msg">The received <see cref="OmemoMessage"/>.</param>
        public void InitDhRatchet(OmemoMessage msg)
        {
            pn = nS;
            nS = 0;
            nR = 0;
            dhR = new GenericECKeyPairModel(null, msg.DH);
            Tuple<byte[], byte[]> tmp = LibSignalUtils.KDF_RK(rk, CryptoUtils.SharedSecret(dhS.privKey, dhR.pubKey));
            rk = tmp.Item1;
            ckR = tmp.Item2;
            dhS = KeyHelper.GenerateKeyPair();
            tmp = LibSignalUtils.KDF_RK(rk, CryptoUtils.SharedSecret(dhS.privKey, dhR.pubKey));
            rk = tmp.Item1;
            ckS = tmp.Item2;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnEkPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(ek) + '.' + e.PropertyName);
        }

        private void OnDhrPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(dhR) + '.' + e.PropertyName);
        }

        private void OnDhsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(dhS) + '.' + e.PropertyName);
        }

        private void OnMK_SKIPPEDPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(nameof(MK_SKIPPED) + '.' + e.PropertyName);
        }

        #endregion
    }
}
