using System;
using System.Diagnostics;
using Omemo.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Controls.Chat;
using UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext.Controls.OMEMO
{
    public class OmemoTrustFingerprintControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoTrustFingerprintControlDataTemplate MODEL = new OmemoTrustFingerprintControlDataTemplate();

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
            if (e.NewValue is OmemoFingerprintDataTemplate fingerprint)
            {
                switch (fingerprint.State)
                {
                    case SessionState.READY:
                        MODEL.StatusTooltip = "Session keys have been exchanged and you are ready to go.";
                        MODEL.StatusText = "✅";
                        break;

                    case SessionState.RECEIVED:
                        MODEL.StatusTooltip = "You received a session request but you have not replied yes. Try sending a message.";
                        MODEL.StatusText = "📨";
                        break;

                    case SessionState.SEND:
                        MODEL.StatusTooltip = "You initiated a new session, but your contact has not yet confirmed the session.";
                        MODEL.StatusText = "📩";
                        break;

                    default:
                        Debug.Assert(false); // Should not happen
                        break;
                }
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
