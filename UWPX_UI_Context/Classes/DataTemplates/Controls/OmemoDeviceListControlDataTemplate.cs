using System.Linq;
using Shared.Classes;
using Shared.Classes.Collections;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class OmemoDeviceListControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CustomObservableCollection<UintDataTemplate> DEVICES = new CustomObservableCollection<UintDataTemplate>(true);

        private bool _RefreshingDevices;
        public bool RefreshingDevices
        {
            get => _RefreshingDevices;
            set => SetProperty(ref _RefreshingDevices, value);
        }
        private bool _ResettingDevices;
        public bool ResettingDevices
        {
            get => _ResettingDevices;
            set => SetProperty(ref _ResettingDevices, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(AccountDataTemplate account)
        {
            DEVICES.Clear();
            if (account is null)
            {
                return;
            }

            DEVICES.AddRange(account.Client.xmppClient.getOmemoHelper().DEVICES.DEVICES.Select(d => new UintDataTemplate { Value = d.ID }));
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
