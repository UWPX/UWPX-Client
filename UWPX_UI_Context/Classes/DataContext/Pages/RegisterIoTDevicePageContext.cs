using System;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using XMPP_API.Classes.XmppUri;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public class RegisterIoTDevicePageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly RegisterIoTDevicePageDataTemplate MODEL = new RegisterIoTDevicePageDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool TryParseUri(string uri)
        {
            IUriAction action = UriUtils.parse(new Uri(uri));
            if (action is RegisterIoTUriAction registerIoTUriAction)
            {
                MODEL.RegisterIoTUriAction = registerIoTUriAction;
                return true;
            }
            return false;
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
