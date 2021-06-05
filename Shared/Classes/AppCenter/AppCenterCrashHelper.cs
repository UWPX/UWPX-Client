using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Microsoft.AppCenter.Crashes;

namespace Shared.Classes.AppCenter
{
    public class AppCenterCrashHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly AppCenterCrashHelper INSTANCE = new AppCenterCrashHelper();

        public event Func<AppCenterCrashHelper, TrackErrorEventArgs, Task> OnTrackError;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void TrackError(Exception ex, string descriptionMd, Dictionary<string, string> payload = null)
        {
            Task.Run(async () => await TrackErrorInTaskAsync(ex, descriptionMd, payload));
        }


        #endregion

        #region --Misc Methods (Private)--
        private async Task TrackErrorInTaskAsync(Exception ex, string descriptionMd, Dictionary<string, string> payload)
        {
            TrackErrorEventArgs args = new TrackErrorEventArgs(ex, descriptionMd, payload);
            Func<AppCenterCrashHelper, TrackErrorEventArgs, Task> handler = OnTrackError;
            if (!(handler is null))
            {
                Delegate[] invocationList = handler.GetInvocationList();
                Task[] handlerTasks = new Task[invocationList.Length];
                SemaphoreSlim sema = new SemaphoreSlim(0);

                // Call them in the UI thread to ensure we await everything:
                await SharedUtils.CallDispatcherAsync(() =>
                {
                    for (int i = 0; i < invocationList.Length; i++)
                    {
                        handlerTasks[i] = ((Func<AppCenterCrashHelper, TrackErrorEventArgs, Task>)invocationList[i])(this, args);
                    }
                    sema.Release();
                });
                await sema.WaitAsync();
                await Task.WhenAll(handlerTasks);
            }

            if (!args.Cancel)
            {
                if (Crashes.Instance.InstanceEnabled)
                {
                    Crashes.TrackError(ex);
                    Logger.Info("Crash has been reported.");
                }
                else
                {
                    Logger.Warn("Crash has not been reported. Crash reporting disabled!");
                }
            }
            else
            {
                Logger.Warn("Crash has not been reported. User declined!");
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
