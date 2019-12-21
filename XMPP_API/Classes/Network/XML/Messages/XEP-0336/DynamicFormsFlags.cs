using System;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0336
{
    [Flags]
    public enum DynamicFormsFlags
    {
        /// <summary>
        /// Flags a field as requiring server post-back after having been edited.
        /// </summary>
        POST_BACK = 0b1,
        /// <summary>
        /// Flags a field as being read-only.
        /// </summary>
        READ_ONLY = 0b10,
        /// <summary>
        /// Flags a field as having an undefined or uncertain value.
        /// </summary>
        NOT_SAME = 0b100,
        /// <summary>
        /// Flags a field as having an error.
        /// </summary>
        ERROR = 0b1000
    }
}
