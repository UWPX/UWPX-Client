using Data_Manager2.Classes.DBTables;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class SpeechBubbleInfoControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatMessageTable ChatMessage
        {
            get { return (ChatMessageTable)GetValue(ChatMessageProperty); }
            set
            {
                SetValue(ChatMessageProperty, value);
                showDate();
            }
        }
        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register("ChatMessage", typeof(ChatMessageTable), typeof(SpeechBubbleInfoControl), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 12/03/2018 Created [Fabian Sauter]
        /// </history>
        public SpeechBubbleInfoControl()
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
        private void showDate()
        {
            if(ChatMessage != null)
            {
                if (ChatMessage.date.Date.CompareTo(DateTime.Now.Date) == 0)
                {
                    date_tbx.Text = ChatMessage.date.ToString("HH:mm");
                }
                else
                {
                    date_tbx.Text = ChatMessage.date.ToString("dd.MM.yyyy HH:mm");
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
