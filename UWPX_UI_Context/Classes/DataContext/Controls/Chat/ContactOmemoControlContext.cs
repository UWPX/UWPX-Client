using System.Threading.Tasks;
using Data_Manager2.Classes.DBManager.Omemo;
using Data_Manager2.Classes.DBTables;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat;
using Windows.UI.Xaml;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

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

        public void OnFingerprintTrustedChanged(OmemoFingerprint fingerprint)
        {
            if (fingerprint.trusted)
            {
                MODEL.TrustedOnly = true;
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
            Task.Run(() =>
            {
                MODEL.Loading = true;
                MODEL.FINGERPRINTS.Clear();
                MODEL.FINGERPRINTS.AddRange(OmemoSignalKeyDBManager.INSTANCE.getFingerprints(MODEL.Chat.id));
                MODEL.Loading = false;
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
