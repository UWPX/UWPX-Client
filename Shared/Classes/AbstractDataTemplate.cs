using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shared.Classes
{
    /// <summary>
    /// Based on: https://github.com/Microsoft/Windows-appsample-trafficapp/blob/master/LocationHelper/BindableBase.cs
    /// </summary>
    public abstract class AbstractDataTemplate: INotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Decides whether the property changed event should be fired in the UI thread.
        /// Default: true
        /// </summary>
        protected bool invokeInUiThread = true;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual async void OnPropertyChanged([CallerMemberName] string name = "")
        {
            // Prevent interrupting the dispatcher thread in case there is nothing to do:
            if (PropertyChanged is null)
            {
                return;
            }

            if (invokeInUiThread)
            {
                // Make sure we call the PropertyChanged event from the UI thread:
                await SharedUtils.CallDispatcherAsync(() =>
                {
                    try
                    {
                        // Keep the null check since this can happen later:
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                });
            }
            else
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        protected virtual async void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            // Prevent interrupting the dispatcher thread in case there is nothing to do:
            if (PropertyChanged is null)
            {
                return;
            }

            if (invokeInUiThread)
            {
                // Make sure we call the PropertyChanged event from the UI thread:
                await SharedUtils.CallDispatcherAsync(() =>
                {
                    try
                    {
                        // Keep the null check since this can happen later:
                        PropertyChanged?.Invoke(this, args);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                });
            }
            else
            {
                PropertyChanged.Invoke(this, args);
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
