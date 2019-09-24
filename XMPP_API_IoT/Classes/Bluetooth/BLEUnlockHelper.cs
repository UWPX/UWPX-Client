using System;
using System.Threading.Tasks;
using Logging;

namespace XMPP_API_IoT.Classes.Bluetooth
{
    internal class BLEUnlockHelper: IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly BLEDevice DEVICE;

        /// <summary>
        /// Default timeout for unlocking is 5 seconds.
        /// </summary>
        private readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(5.0);

        private Task timeoutTask;
        private TaskCompletionSource<bool> completionSource;
        private bool disposed;
        private bool done;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public BLEUnlockHelper(BLEDevice device)
        {
            DEVICE = device;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task<bool> UnlockAsync()
        {
            done = false;
            completionSource = new TaskCompletionSource<bool>();
            DEVICE.CACHE.CharacteristicChanged -= CACHE_CharacteristicChanged;
            DEVICE.CACHE.CharacteristicChanged += CACHE_CharacteristicChanged;

            // At the moment a simple "ready" unlocks the ESP32.
            // This will change in future versions (public/private key authentication).
            await DEVICE.WriteStringAsync(BTUtils.CHARACTERISTIC_CHALLENGE_RESPONSE_WRITE, "ready");
            return await WaitForCompletionAsync();
        }

        public void stop()
        {
            if (!disposed)
            {
                done = true;
                if (!(completionSource is null) && !completionSource.Task.IsCanceled && !completionSource.Task.IsCompleted && !completionSource.Task.IsFaulted)
                {
                    completionSource.SetResult(false);
                }

                if (!(DEVICE is null))
                {
                    DEVICE.CACHE.CharacteristicChanged -= CACHE_CharacteristicChanged;
                }
            }
        }

        public void Dispose()
        {
            stop();
            disposed = true;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task<bool> WaitForCompletionAsync()
        {
            timeoutTask = Task.Delay(TIMEOUT);

            // Wait for completion:
            Task resultTask = await Task.WhenAny(new Task[] { completionSource.Task, timeoutTask });

            bool result = false;
            // Evaluate and return result:
            if (resultTask == completionSource.Task)
            {
                if (completionSource.Task.IsCompleted)
                {
                    Logger.Info("Bluetooth unlock helper finished with: " + completionSource.Task.Result);
                    result = completionSource.Task.Result;
                }
                else
                {
                    Logger.Warn("Bluetooth unlock helper failed with: UNKNOWN");
                    result = false;
                }
            }
            else
            {
                Logger.Warn("Bluetooth unlock helper failed with: TIMEOUT");
                result = false;
            }

            stop();
            return result;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void CACHE_CharacteristicChanged(CharacteristicsCache sender, Events.CharacteristicChangedEventArgs args)
        {
            if (args.UUID.Equals(BTUtils.CHARACTERISTIC_CHALLENGE_RESPONSE_UNLOCKED))
            {
                if (DEVICE.CACHE.GetBool(BTUtils.CHARACTERISTIC_CHALLENGE_RESPONSE_UNLOCKED))
                {
                    completionSource.TrySetResult(true);
                    done = true;
                }
            }
        }

        #endregion
    }
}
