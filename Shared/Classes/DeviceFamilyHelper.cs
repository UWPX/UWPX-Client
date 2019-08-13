using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace Shared.Classes
{
    public enum DeviceFamilyType
    {
        Mobile,
        Desktop,
        Tablet,
        IoT,
        SurfaceHub,
        HoloLens,
        Xbox,
        Other
    }

    public static class DeviceFamilyHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static bool IsMouseInteractionMode()
        {
            return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse;
        }

        public static DeviceFamilyType GetDeviceFamilyType()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Desktop":
                    return IsMouseInteractionMode()
                        ? DeviceFamilyType.Desktop
                        : DeviceFamilyType.Tablet;

                case "Windows.Mobile":
                    return DeviceFamilyType.Mobile;

                case "Windows.IoT":
                    return DeviceFamilyType.IoT;

                case "Windows.Xbox":
                    return DeviceFamilyType.Xbox;

                case "Windows.Holographic":
                    return DeviceFamilyType.HoloLens;

                case "Windows.Team":
                    return DeviceFamilyType.SurfaceHub;

                default:
                    return DeviceFamilyType.Other;
            }
        }

        /// <summary>
        /// Checks whether the current device is a Windows 10 Desktop device.
        /// </summary>
        public static bool IsRunningOnDesktopDevice()
        {
            DeviceFamilyType type = GetDeviceFamilyType();
            return type == DeviceFamilyType.Desktop
                || type == DeviceFamilyType.Tablet;
        }

        /// <summary>
        /// Returns whether it is recommend to show a navigation back button.
        /// </summary>
        /// <returns>True if it's recommend to show a navigation back button.</returns>
        public static bool ShouldShowBackButton()
        {
            DeviceFamilyType type = GetDeviceFamilyType();
            return type == DeviceFamilyType.Desktop
                || type == DeviceFamilyType.Tablet
                || type == DeviceFamilyType.SurfaceHub
                || type == DeviceFamilyType.HoloLens
                || type == DeviceFamilyType.Mobile
                || type == DeviceFamilyType.IoT;
        }

        /// <summary>
        /// Checks whether the device supports vibration and has the specific APIs available.
        /// </summary>
        /// <returns>True if the device has a vibration motor and the required APIs available.</returns>
        public static bool SupportsVibration()
        {
            return ApiInformation.IsTypePresent("Windows.Phone.Devices.Notification.VibrationDevice");
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
