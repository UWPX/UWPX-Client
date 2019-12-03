using System;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0336
{
    [Flags]
    public enum DynamicFormsFlags
    {
        /// <summary>
        /// Flags a field as requiring server post-back after having been edited.
        /// </summary>
        POST_BACK,
        /// <summary>
        /// Flags a field as being read-only.
        /// </summary>
        READ_ONLY,
        /// <summary>
        /// Flags a field as having an undefined or uncertain value.
        /// </summary>
        NOT_SAME,
        /// <summary>
        /// Flags a field as having an error.
        /// </summary>
        ERROR
    }
}
