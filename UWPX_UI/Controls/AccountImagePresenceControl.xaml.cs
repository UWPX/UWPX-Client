﻿using Storage.Classes.Models.Chat;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;

namespace UWPX_UI.Controls
{
    public sealed partial class AccountImagePresenceControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Presence PresenceProp
        {
            get => (Presence)GetValue(PresencePropProperty);
            set => SetValue(PresencePropProperty, value);
        }
        public static readonly DependencyProperty PresencePropProperty = DependencyProperty.Register(nameof(PresenceProp), typeof(Presence), typeof(AccountImagePresenceControl), new PropertyMetadata(Presence.Unavailable));

        public ImageSource Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(AccountImagePresenceControl), new PropertyMetadata(null));

        public string BareJid
        {
            get => (string)GetValue(BareJidProperty);
            set => SetValue(BareJidProperty, value);
        }
        public static readonly DependencyProperty BareJidProperty = DependencyProperty.Register(nameof(BareJid), typeof(string), typeof(AccountImagePresenceControl), new PropertyMetadata("", OnBareJidChanged));

        public ChatType ChatType
        {
            get => (ChatType)GetValue(ChatTypeProperty);
            set => SetValue(ChatTypeProperty, value);
        }
        public static readonly DependencyProperty ChatTypeProperty = DependencyProperty.Register(nameof(ChatType), typeof(ChatType), typeof(AccountImagePresenceControl), new PropertyMetadata(ChatType.CHAT, OnChatTypeChanged));

        public Visibility PresenceVisibility
        {
            get => (Visibility)GetValue(PresenceVisibilityProperty);
            set => SetValue(PresenceVisibilityProperty, value);
        }
        public static readonly DependencyProperty PresenceVisibilityProperty = DependencyProperty.Register(nameof(PresenceVisibility), typeof(Visibility), typeof(AccountImagePresenceControl), new PropertyMetadata(Visibility.Visible));

        public readonly AccountImagePresenceControlContext VIEW_MODEL = new AccountImagePresenceControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AccountImagePresenceControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView()
        {
            VIEW_MODEL.UpdateView(ChatType, BareJid);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnBareJidChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountImagePresenceControl control)
            {
                control.UpdateView();
            }
        }

        private static void OnChatTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountImagePresenceControl control)
            {
                control.UpdateView();
            }
        }

        #endregion
    }
}
