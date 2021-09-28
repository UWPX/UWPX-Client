using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO;

namespace UWPX_UI_Context.Classes.DataContext.Controls.OMEMO
{
    public class OmemoTroubleshooterControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoTroubleshooterControlDataTemplate MODEL = new OmemoTroubleshooterControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Fix(AccountDataTemplate account)
        {
            if (MODEL.Fixing)
            {
                return;
            }
            MODEL.Fixing = true;
            MODEL.Working = true;
            MODEL.StatusText = "Fixing...";
            _ = Task.Run(async () =>
            {
                await Task.Delay(5000);
                MODEL.Fixing = false;
                MODEL.Working = false;
                MODEL.StatusText = "";
            });
        }

        public void Troubleshoote(AccountDataTemplate account)
        {
            if (MODEL.Troubleshooting)
            {
                return;
            }
            MODEL.Troubleshooting = true;
            MODEL.Working = true;
            MODEL.StatusText = "Troubleshooting...";
            _ = Task.Run(async () =>
              {
                  await Task.Delay(5000);
                  MODEL.Troubleshooting = false;
                  MODEL.Working = false;
                  MODEL.StatusText = "";
              });
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
