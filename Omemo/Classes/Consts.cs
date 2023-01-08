using System;

namespace Omemo.Classes
{
    public static class Consts
    {
        /// <summary>
        /// Defines the X3DH info string.
        /// <para/>
        /// Read more: https://xmpp.org/extensions/xep-0384.html#protocol-key_exchange
        /// </summary>
        public static readonly string X3DH_INFO_STRING = "OMEMO X3DH";
        /// <summary>
        /// Defines how long a signed PreKey should is valid until it should be replaced by a new one.
        /// <para/>
        /// Read more: https://xmpp.org/extensions/xep-0384.html#protocol-key_exchange
        /// </summary>
        public static readonly TimeSpan SIGNED_KEY_ROTATION_PERIOD = TimeSpan.FromDays(15);
        /// <summary>
        /// How many PreKeys should a bundle contain.
        /// <para/>
        /// Read more: https://xmpp.org/extensions/xep-0384.html#protocol-key_exchange
        /// </summary>
        public static uint NUM_PRE_KEYS_PROVIDED = 100;
    }
}
