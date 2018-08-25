using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.Networking.Sockets;

namespace XMPP_API.Classes.Network
{
    public class ConnectionInformation
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _tlsConnected;
        public bool tlsConnected
        {
            get
            {
                return _tlsConnected;
            }
            set
            {
                if (value != _tlsConnected)
                {
                    _tlsConnected = value;
                    onPropertyChanged(nameof(tlsConnected));
                }
            }
        }

        private StreamSocketInformation _socketInfo;
        public StreamSocketInformation socketInfo
        {
            get
            {
                return _socketInfo;
            }
            set
            {
                if (value != _socketInfo)
                {
                    _socketInfo = value;
                    onPropertyChanged(nameof(socketInfo));
                }
            }
        }

        private MessageCarbonsState _msgCarbonsState;
        public MessageCarbonsState msgCarbonsState
        {
            get
            {
                return _msgCarbonsState;
            }
            set
            {
                if (value != _msgCarbonsState)
                {
                    _msgCarbonsState = value;
                    onPropertyChanged(nameof(msgCarbonsState));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 13/04/2018 Created [Fabian Sauter]
        /// </history>
        public ConnectionInformation()
        {
            this.msgCarbonsState = MessageCarbonsState.DISABLED;
            this.tlsConnected = false;
            this.socketInfo = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Gets detailed certificate information.
        /// Based on: https://github.com/Microsoft/Windows-universal-samples/blob/master/Samples/StreamSocket/cs/Scenario5_Certificates.xaml.cs
        /// </summary>
        /// <returns>A string containing certificate details.</returns>
        public string getCertificateInformation()
        {
            if (socketInfo != null && socketInfo.ServerCertificate != null)
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine("\tFriendly Name: " + socketInfo.ServerCertificate.FriendlyName);
                stringBuilder.AppendLine("\tSubject: " + socketInfo.ServerCertificate.Subject);
                stringBuilder.AppendLine("\tIssuer: " + socketInfo.ServerCertificate.Issuer);
                stringBuilder.AppendLine("\tValidity: " + socketInfo.ServerCertificate.ValidFrom + " - " + socketInfo.ServerCertificate.ValidTo);

                // Enumerate the entire certificate chain.
                if (socketInfo.ServerIntermediateCertificates.Count > 0)
                {
                    stringBuilder.AppendLine("\tCertificate chain: ");
                    foreach (var cert in socketInfo.ServerIntermediateCertificates)
                    {
                        stringBuilder.AppendLine("\t\tIntermediate Certificate Subject: " + cert.Subject);
                    }
                }
                else
                {
                    stringBuilder.AppendLine("\tNo certificates within the intermediate chain.");
                }

                return stringBuilder.ToString();
            }
            return null;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void onPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
