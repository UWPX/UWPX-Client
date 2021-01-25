using System.ComponentModel.DataAnnotations;
using Omemo.Classes;

namespace Storage.Classes.Models.Omemo
{
    /// <summary>
    /// Represents a single OMEMO capable device.
    /// </summary>
    public class OmemoDeviceModel
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        /// <summary>
        /// The unique ID of the device
        /// </summary>
        [Required]
        public uint deviceId { get; set; }
        /// <summary>
        /// The local name for this device.
        /// </summary>
        public string deviceLabel { get; set; }
        /// <summary>
        /// Not null in case there exists a session with this device.
        /// </summary>
        public OmemoSessionModel session { get; set; }
        /// <summary>
        /// The fingerprint of this device.
        /// </summary>
        public OmemoFingerprintModel fingerprint { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoDeviceModel() { }

        public OmemoDeviceModel(OmemoProtocolAddress address)
        {
            deviceId = address.DEVICE_ID;
        }

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
