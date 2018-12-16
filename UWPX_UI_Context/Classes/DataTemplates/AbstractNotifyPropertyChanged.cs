using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public abstract class AbstractNotifyPropertyChanged : INotifyPropertyChanged
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
        protected async void OnPropertyChanged([CallerMemberName] string name = "")
        {
            await UiUtils.CallDispatcherAsync(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
