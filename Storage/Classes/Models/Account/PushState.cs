namespace Storage.Classes.Models.Account
{
    /// <summary>
    /// Represents the current state of enabling/disabling XEP 0357 push at the XMPP server.
    /// </summary>
    public enum PushState
    {
        /// <summary>
        /// Push has been disabled successfully.
        /// </summary>
        DISABLED,
        /// <summary>
        /// Push should be enabled, but has not, or it failed the last time.
        /// </summary>
        ENABLING,
        /// <summary>
        /// Push has been enabled successfully.
        /// </summary>
        ENABLED,
        /// <summary>
        /// Push should be disabled, but has not, or it failed the last time.
        /// </summary>
        DISABLING,
        /// <summary>
        /// The server does not support push (XEP 0357).
        /// </summary>
        NOT_SUPPORTED
    }
}
