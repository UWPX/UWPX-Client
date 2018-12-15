using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes
{
    static class UiUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static SolidColorBrush getPresenceBrush(Presence presence)
        {
            switch (presence)
            {
                case Presence.Online:
                    return (SolidColorBrush)Application.Current.Resources["PresenceOnline"];

                case Presence.Chat:
                    return (SolidColorBrush)Application.Current.Resources["PresenceChat"];

                case Presence.Away:
                    return (SolidColorBrush)Application.Current.Resources["PresenceAway"];

                case Presence.Xa:
                    return (SolidColorBrush)Application.Current.Resources["PresenceXa"];

                case Presence.Dnd:
                    return (SolidColorBrush)Application.Current.Resources["PresenceDnd"];

                default:
                    return (SolidColorBrush)Application.Current.Resources["PresenceUnavailable"];

            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
