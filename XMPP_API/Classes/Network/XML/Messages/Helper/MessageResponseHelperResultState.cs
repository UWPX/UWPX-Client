﻿namespace XMPP_API.Classes.Network.XML.Messages.Helper
{
    public enum MessageResponseHelperResultState
    {
        /// <summary>
        /// The request was successful.
        /// </summary>
        SUCCESS,
        /// <summary>
        /// A timeout occurred.
        /// </summary>
        TIMEOUT,
        /// <summary>
        /// The helper has been disposed.
        /// </summary>
        DISPOSED,
        /// <summary>
        /// An error occurred during sending the message.
        /// </summary>
        SEND_FAILED,
        /// <summary>
        /// A general error occurred.
        /// </summary>
        ERROR,
        /// <summary>
        /// Sending the message failed, but the message has been cached and will be send later.
        /// </summary>
        WILL_SEND_LATER
    }
}
