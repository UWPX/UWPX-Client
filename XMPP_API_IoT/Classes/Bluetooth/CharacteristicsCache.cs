using System;
using System.Collections.Generic;
using System.Linq;
using Logging;
using XMPP_API.Classes.Crypto;
using XMPP_API_IoT.Classes.Bluetooth.Events;

namespace XMPP_API_IoT.Classes.Bluetooth
{
    public class CharacteristicsCache
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly Guid[] SUBSCRIBE_TO_CHARACTERISTICS = new Guid[] {
            BTUtils.CHARACTERISTIC_LANGUAGE,
            BTUtils.CHARACTERISTIC_HARDWARE_REVISION,
            BTUtils.CHARACTERISTIC_SERIAL_NUMBER,
            BTUtils.CHARACTERISTIC_MANUFACTURER_NAME,

            BTUtils.CHARACTERISTIC_CHALLENGE_RESPONSE_READ,
            BTUtils.CHARACTERISTIC_CHALLENGE_RESPONSE_UNLOCKED
        };

        private readonly Dictionary<Guid, byte[]> CHARACTERISTICS = new Dictionary<Guid, byte[]>();
        private readonly object CHARACTERISTICS_LOCK = new object();

        public delegate void CharacteristicChangedHandler(CharacteristicsCache sender, CharacteristicChangedEventArgs args);
        public event CharacteristicChangedHandler CharacteristicChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static string GetString(byte[] value)
        {
            return value != null ? System.Text.Encoding.ASCII.GetString(value) : null;
        }

        public string GetString(Guid uuid)
        {
            byte[] value = GetBytes(uuid);
            return GetString(value);
        }

        public static uint GetUint(byte[] value)
        {
            if (value != null)
            {
                switch (value.Length)
                {
                    case 2:
                        return BitConverter.ToUInt16(value, 0);

                    case 4:
                        return BitConverter.ToUInt32(value, 0);
                }
            }
            return 0;
        }

        public uint GetUint(Guid uuid)
        {
            byte[] value = GetBytes(uuid);
            return GetUint(value);
        }

        public static ulong GetULong(byte[] value)
        {
            return value != null && value.Length == 8 ? BitConverter.ToUInt64(value, 0) : 0;
        }

        public ulong GetULong(Guid uuid)
        {
            byte[] value = GetBytes(uuid);
            return GetULong(value);
        }

        public static bool GetBool(byte[] value)
        {
            return value != null && value.Length >= 1 ? BitConverter.ToBoolean(value, 0) : false;
        }

        public bool GetBool(Guid uuid)
        {
            byte[] value = GetBytes(uuid);
            return GetBool(value);
        }

        public byte[] GetBytes(Guid uuid)
        {
            byte[] value = null;
            lock (CHARACTERISTICS_LOCK)
            {
                CHARACTERISTICS.TryGetValue(uuid, out value);
            }
            return value;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Adds the given value to the characteristics dictionary and triggers the BoardCharacteristicChanged event.
        /// </summary>
        /// <param name="uuid">The UUID of the characteristic.</param>
        /// <param name="value">The value of the characteristics.</param>
        /// <returns>True if the value is not the same as already stored for this characteristic.</returns>
        internal bool AddToDictionary(Guid uuid, byte[] value)
        {
            return AddToDictionary(uuid, value, DateTime.Now);
        }

        /// <summary>
        /// Adds the given value to the characteristics dictionary and triggers the BoardCharacteristicChanged event.
        /// </summary>
        /// <param name="uuid">The UUID of the characteristic.</param>
        /// <param name="value">The value of the characteristics.</param>
        /// <param name="timestamp">A when did the value change?</param>
        /// <returns>True if the value is not the same as already stored for this characteristic.</returns>
        internal bool AddToDictionary(Guid uuid, byte[] value, DateTime timestamp)
        {
            if (value is null)
            {
                return false;
            }

            byte[] oldValue = null;
            lock (CHARACTERISTICS_LOCK)
            {
                if (CHARACTERISTICS.ContainsKey(uuid))
                {
                    oldValue = CHARACTERISTICS[uuid];
                    CHARACTERISTICS[uuid] = value;
                    if (!oldValue.SequenceEqual(value))
                    {
                        Logger.Debug("Bluetooth characteristic " + uuid.ToString() + " has a new value: " + CryptoUtils.byteArrayToHexString(value));
                        CharacteristicChanged?.Invoke(this, new CharacteristicChangedEventArgs(uuid, oldValue, value));
                    }
                }
                else
                {
                    CHARACTERISTICS[uuid] = value;
                }
            }
            return true;
        }

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
