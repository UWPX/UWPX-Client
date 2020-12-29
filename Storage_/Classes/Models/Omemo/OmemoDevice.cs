using System.ComponentModel.DataAnnotations;

namespace Storage.Classes.Models.Omemo
{
    /// <summary>
    /// Represents a single OMEMO capable device.
    /// </summary>
    public class OmemoDevice
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public string id { get; set; }
        /// <summary>
        /// The unique ID of the device
        /// </summary>
        [Required]
        public uint deviceId { get; set; }
        /// <summary>
        /// Not null in case there exists a session with this device.
        /// </summary>
        public OmemoSession session { get; set; }
        /// <summary>
        /// The fingerprint of this device.
        /// </summary>
        public OmemoFingerprint fingerprint { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


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
