using Shared.Classes;
using Shared.Classes.Collections;
using System.Collections.Generic;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class OmemoDeviceListControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CustomObservableCollection<uint> DEVICES = new CustomObservableCollection<uint>(true);

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

            IEnumerable<uint> devices = account.Client.getOmemoHelper()?.DEVICES?.IDS;
            if (!(devices is null))
            {
                DEVICES.AddRange(account.Client.getOmemoHelper().DEVICES.IDS);
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
