using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Storage.Classes.Models.Omemo.Keys;

namespace Storage.Classes.Models.Omemo
{
    /// <summary>
    /// An established session for XEP-0384 (OMEMO Encryption).
    /// </summary>
    public class OmemoSessionModel: AbstractOmemoModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        /// <summary>
        /// Key pair for the sending ratchet.
        /// </summary>
        public ECKeyPairModel dhS { get; set; }
        /// <summary>
        /// Key pair for the receiving ratchet.
        /// </summary>
        public ECKeyPairModel dhR { get; set; }
        /// <summary>
        /// Ephemeral key used for initiating this session. 
        /// </summary>
        [Required]
        public byte[] ek { get; set; }
        /// <summary>
        /// 32 byte root key for encryption.
        /// </summary>
        [Required]
        public byte[] rk { get; set; }
        /// <summary>
        /// 32 byte Chain Keys for sending.
        /// </summary>
        public byte[] ckS { get; set; }
        /// <summary>
        /// 32 byte Chain Keys for receiving.
        /// </summary>
        public byte[] ckR { get; set; }
        /// <summary>
        /// Message numbers for sending.
        /// </summary>
        [Required]
        public uint nS { get; set; }
        /// <summary>
        /// Message numbers for receiving.
        /// </summary>
        [Required]
        public uint nR { get; set; }
        /// <summary>
        /// Number of messages in previous sending chain.
        /// </summary>
        [Required]
        public uint pn { get; set; }
        /// <summary>
        /// Skipped-over message keys, indexed by ratchet <see cref="ECPubKey"/> and message number. Raises an exception if too many elements are stored.
        /// </summary>
        public List<SkippedMessageKeyGroupModel> skippedMessageKeys { get; set; } = new List<SkippedMessageKeyGroupModel>();
        /// <summary>
        /// The id of the PreKey used to create establish this session.
        /// </summary>
        public uint preKeyId { get; set; }
        /// <summary>
        /// The id of the signed PreKey used to create establish this session.
        /// </summary>
        public uint signedPreKeyId { get; set; }
        /// <summary>
        /// The associated data is created by concatenating the IdentityKeys of Alice and Bob.
        /// <para/>
        /// AD = Encode(IK_A) || Encode(IK_B).
        /// <para/>
        /// Alice is the party that actively initiated the key exchange, while Bob is the party that passively accepted the key exchange.
        /// </summary>
        [Required]
        public byte[] assData { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoSessionModel() { }

        public OmemoSessionModel(OmemoSession session)
        {
            dhS = new ECKeyPairModel(session.dhS);
            dhR = new ECKeyPairModel(session.dhR);
            rk = session.rk;
            ckS = session.ckS;
            ckR = session.ckR;
            nS = session.nS;
            nR = session.nR;
            pn = session.pn;
            preKeyId = session.preKeyId;
            signedPreKeyId = session.signedPreKeyId;
            assData = session.assData;
            foreach (Tuple<ECPubKey, Dictionary<uint, byte[]>> group in session.MK_SKIPPED.MKS)
            {
                SkippedMessageKeyGroupModel tmp = new SkippedMessageKeyGroupModel(group.Item1.key);
                foreach (KeyValuePair<uint, byte[]> key in group.Item2)
                {
                    tmp.keys.Add(new SkippedMessageKeyModel(key.Key, key.Value));
                }
                skippedMessageKeys.Add(tmp);
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public OmemoSession ToOmemoSession()
        {
            OmemoSession session = new OmemoSession()
            {
                dhS = dhS.ToECKeyPair(),
                dhR = dhR.ToECKeyPair(),
                ek = new ECPubKey(ek),
                rk = rk,
                ckS = ckS,
                ckR = ckR,
                nS = nS,
                nR = nR,
                pn = pn,
                preKeyId = preKeyId,
                signedPreKeyId = signedPreKeyId,
                assData = assData
            };
            foreach (SkippedMessageKeyGroupModel group in skippedMessageKeys)
            {
                Tuple<ECPubKey, Dictionary<uint, byte[]>> entry = new Tuple<ECPubKey, Dictionary<uint, byte[]>>(new ECPubKey(group.dhr), new Dictionary<uint, byte[]>());
                foreach (SkippedMessageKeyModel key in group.keys)
                {
                    entry.Item2[key.nr] = key.mk;
                }
                session.MK_SKIPPED.MKS.Add(entry);
            }
            return session;
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
