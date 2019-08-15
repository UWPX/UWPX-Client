using System.Threading.Tasks;
using Data_Manager2.Classes.DBTables;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat;
using Windows.UI.Xaml;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataContext.Controls.Chat
{
    public class ContactOmemoControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ContactOmemoControlDataTemplate MODEL = new ContactOmemoControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ContactOmemoControlContext()
        {
            LoadFingerprints();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ChatTable chat)
            {
                MODEL.Chat = chat;

                if (!(MODEL.Client is null))
                {
                    LoadFingerprints();
                }
            }
            else if (e.NewValue is XMPPClient client)
            {
                MODEL.Client = client;

                if (!(MODEL.Chat is null))
                {
                    LoadFingerprints();
                }
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadFingerprints()
        {
            if (MODEL.Loading)
            {
                return;
            }
            Task.Run(async () =>
            {
                MODEL.Loading = true;
                await LoadFingerprintsAsync();
                MODEL.Loading = false;
            });
        }

        private async Task LoadFingerprintsAsync()
        {
            OmemoHelper helper = MODEL.Client.getOmemoHelper();
            MODEL.FINGERPRINTS.Clear();
            if (!(helper is null))
            {

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
