using Shared.Classes;
using Windows.Devices.Lights;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public class CameraPreviewControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _LampAvailable;
        public bool LampAvailable
        {
            get => _LampAvailable;
            set => SetProperty(ref _LampAvailable, value);
        }

        private bool _MultipleCamerasAvailable;
        public bool MultipleCamerasAvailable
        {
            get => _MultipleCamerasAvailable;
            set => SetProperty(ref _MultipleCamerasAvailable, value);
        }

        private Lamp _Lamp;
        public Lamp Lamp
        {
            get => _Lamp;
            set => SetLampProperty(value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private void SetLampProperty(Lamp value)
        {
            if (SetProperty(ref _Lamp, value, nameof(Lamp)))
            {
                LampAvailable = !(value is null);
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
