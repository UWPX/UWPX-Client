using Data_Manager2.Classes.DBTables;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class SpeechBubbleContentControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatMessageTable ChatMessage
        {
            get { return (ChatMessageTable)GetValue(ChatMessageProperty); }
            set
            {
                SetValue(ChatMessageProperty, value);
                showChatMessage();
            }
        }

        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register("ChatMessage", typeof(ChatMessageTable), typeof(SpeechBubbleContentControl), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 14/12/2017 Created [Fabian Sauter]
        /// </history>
        public SpeechBubbleContentControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Updates all controls with the proper content.
        /// </summary>
        private void showChatMessage()
        {
            if (ChatMessage != null)
            {
                if (ChatMessage.isImage)
                {
                    message_tbx.Visibility = Visibility.Collapsed;
                    image_img.Source = ChatMessage.message ?? "Error!";
                    image_img.Visibility = Visibility.Visible;
                }
                else
                {
                    image_img.Visibility = Visibility.Collapsed;
                    message_tbx.Text = ChatMessage.message ?? "";
                    message_tbx.Visibility = Visibility.Visible;
                }
                DateTime localDateTime = ChatMessage.date.ToLocalTime();
                if (localDateTime.Date.CompareTo(DateTime.Now.Date) == 0)
                {
                    date_tbx.Text = localDateTime.ToString("HH:mm");
                }
                else
                {
                    date_tbx.Text = localDateTime.ToString("dd.MM.yyyy HH:mm");
                }
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
