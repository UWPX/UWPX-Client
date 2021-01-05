namespace Omemo.Classes
{
    public interface IOmemoStorage
    {
        /// <summary>
        /// Checks and returns the <see cref="OmemoSession"/> for the given <see cref="OmemoProtocolAddress"/> or null in case no session for this <see cref="OmemoProtocolAddress"/> exists.
        /// </summary>
        /// <param name="address">The <see cref="OmemoProtocolAddress"/> a session should be retrieved.</param>
        OmemoSession LoadSession(OmemoProtocolAddress address);
        /// <summary>
        /// Stores the session for the given <see cref="OmemoProtocolAddress"/>.
        /// </summary>
        /// <param name="address">The address for the session.</param>
        /// <param name="session">The session to store.</param>
        void StoreSession(OmemoProtocolAddress address, OmemoSession session);
    }
}
