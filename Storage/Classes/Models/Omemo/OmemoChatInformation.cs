using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Storage.Classes.Models.Omemo
{
    /// <summary>
    /// Information about the XEP-0384 (OMEMO Encryption) chat status.
    /// </summary>
    public class OmemoChatInformation
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public string id { get; set; }
        /// <summary>
        /// The device list subscription states for this chat.
        /// </summary>
        public OmemoDeviceListSubscription deviceListSubscription { get; set; }
        /// <summary>
        /// A collection of devices involved in this chat.
        /// </summary>
        [Required]
        public List<OmemoDevice> devices { get; set; } = new List<OmemoDevice>();
        /// <summary>
        /// Has the OMEMO encryption been enabled for this chat.
        /// </summary>
        [Required]
        public bool enabled { get; set; }
        /// <summary>
        /// Allow and send messages from and to trusted devices only.
        /// </summary>
        [Required]
        public bool trustedKeysOnly { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoChatInformation()
        {
            deviceListSubscription = new OmemoDeviceListSubscription();
            devices = new List<OmemoDevice>();
            enabled = Settings.GetSettingBoolean(SettingsConsts.ENABLE_OMEMO_BY_DEFAULT_FOR_NEW_CHATS);
            trustedKeysOnly = false;
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
