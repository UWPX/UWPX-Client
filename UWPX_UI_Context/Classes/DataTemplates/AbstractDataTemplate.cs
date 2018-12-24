using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    /// <summary>
    /// Based o: https://github.com/Microsoft/Windows-appsample-trafficapp/blob/master/LocationHelper/BindableBase.cs
    /// </summary>
    public abstract class AbstractDataTemplate : INotifyPropertyChanged
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public event PropertyChangedEventHandler PropertyChanged;

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
            await UiUtils.CallDispatcherAsync(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
