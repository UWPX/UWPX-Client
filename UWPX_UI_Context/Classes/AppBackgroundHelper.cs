using Data_Manager2.Classes;
using System.Threading.Tasks;

namespace UWPX_UI_Context.Classes
{
    public static class AppBackgroundHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async Task InitAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            TrySetDefaultBackground();
        }

        #endregion

        #region --Misc Methods (Private)--
        private static void TrySetDefaultBackground()
        {
            // Set default background:
            if (!Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
            {
                Settings.setSetting(SettingsConsts.CHAT_EXAMPLE_BACKGROUND_IMAGE_NAME, "light_bulb.jpeg");
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
