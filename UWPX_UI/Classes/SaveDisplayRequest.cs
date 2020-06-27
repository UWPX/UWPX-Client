using Windows.System.Display;

namespace UWPX_UI.Classes
{
    /// <summary>
    /// A simpel wrapper arround <see cref="DisplayRequest"/> that prevents an exception
    /// if <see cref="DisplayRequest.RequestRelease"/> is beeing called without <see cref="DisplayRequest.RequestActive"/> before.
    /// </summary>
    public class SaveDisplayRequest
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private DisplayRequest displayRequest = new DisplayRequest();
        private int count = 0;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void RequestActive()
        {
            count++;
            displayRequest.RequestActive();
        }

        public void RequestRelease()
        {
            if (count > 0)
            {
                displayRequest.RequestRelease();
                count--;
            }
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
