using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Logging;
using Windows.ApplicationModel.Core;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Phone.Devices.Notification;
using Windows.System.Profile;
using Windows.UI.Core;

namespace Shared.Classes
{
    public static class SharedUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns a hex string representing an unique device ID.
        /// </summary>
        public static string GetUniqueDeviceId()
        {
            SystemIdentificationInfo systemId = SystemIdentification.GetSystemIdForPublisher();
            if (systemId.Source != SystemIdentificationSource.None)
            {
                return ByteArrayToHexString(systemId.Id.ToArray());
            }

            // Fall back to generating a unique ID based on the hardware of the system.
            // This ID will change once the hardware changes.
            // Based on: https://montemagno.com/unique-device-id-for-mobile-apps/
            HardwareToken hwToken = HardwareIdentification.GetPackageSpecificToken(null);
            return ByteArrayToHexString(hwToken.Id.ToArray());
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Calls the UI thread dispatcher and executes the given callback on it.
        /// </summary>
        /// <param name="callback">The callback that should be executed in the UI thread.</param>
        public static async Task CallDispatcherAsync(DispatchedHandler callback)
        {
            if (CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
            {
                callback();
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, callback);
            }
        }

        /// <summary>
        /// Retries the given action once on exception.
        /// </summary>
        /// <param name="action">The action that should get executed.</param>
        public static void RetryOnException(Action action)
        {
            RetryOnException(action, 1);
        }

        /// <summary>
        /// Retries the given action retryCount times on exception.
        /// </summary>
        /// <param name="action">The action that should get executed.</param>
        /// <param name="retryCount">How many times should we try to execute the given action?</param>
        public static void RetryOnException(Action action, int retryCount)
        {
            int i = 0;
            do
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    if (retryCount <= i)
                    {
                        throw e;
                    }
                    else
                    {
                        Logger.Error("Retry exception: ", e);
                        i++;
                    }
                }
            } while (true);
        }

        /// <summary>
        /// Retries the given function once on exception.
        /// </summary>
        /// <param name="funct">The function that should get executed.</param>
        /// <param name="retryCount">How many times should we try to execute the given action?</param>
        /// <returns>The return value of the given function.</returns>
        public static T RetryOnException<T>(Func<T> funct)
        {
            return RetryOnException(funct, 1);
        }

        /// <summary>
        /// Retries the given function retryCount times on exception.
        /// </summary>
        /// <param name="funct">The function that should get executed.</param>
        /// <param name="retryCount">How many times should we try to execute the given action?</param>
        /// <returns>The return value of the given function.</returns>
        public static T RetryOnException<T>(Func<T> funct, int retryCount)
        {
            int i = 0;
            do
            {
                try
                {
                    return funct();
                }
                catch (Exception e)
                {
                    if (retryCount <= i)
                    {
                        throw e;
                    }
                    else
                    {
                        Logger.Error("Retry exception: ", e);
                        i++;
                    }
                }
            } while (true);
        }

        /// <summary>
        /// Creates a MediaPlayer object and plays the given sound.
        /// </summary>
        /// <param name="s">The URI sound path.</param>
        /// <returns>The created MediaPlayer object, that plays the requested sound.</returns>
        public static MediaPlayer PlaySoundFromUri(string uri)
        {
            MediaPlayer player = new MediaPlayer()
            {
                Source = MediaSource.CreateFromUri(new Uri(uri))
            };
            player.Play();
            return player;
        }

        /// <summary>
        /// Vibrates the device for the given timespan if the device supports phone vibration.
        /// </summary>
        /// <param name="duration">How long should the vibration persist. Max 5 seconds.</param>
        public static void VibratePress(TimeSpan duration)
        {
            if (DeviceFamilyHelper.SupportsVibration())
            {
                VibrationDevice.GetDefault().Vibrate(duration);
            }
            Logger.Debug("Vibration not supported.");
        }

        #endregion

        #region --Misc Methods (Private)--
        private static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
