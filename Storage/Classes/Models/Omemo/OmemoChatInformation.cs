using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data_Manager2.Classes;

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
        /// A collection of all device list subscription states for this chat.
        /// </summary>
        [Required]
        public List<OmemoDeviceListSubscription> deviceListSubscriptions { get; set; }
        /// <summary>
        /// A collection of devices involved in this chat.
        /// </summary>
        [Required]
        public List<OmemoDevice> devices { get; set; }
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
            deviceListSubscriptions = new List<OmemoDeviceListSubscription>();
            devices = new List<OmemoDevice>();
            enabled = Settings.getSettingBoolean(SettingsConsts.ENABLE_OMEMO_BY_DEFAULT_FOR_NEW_CHATS);
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
