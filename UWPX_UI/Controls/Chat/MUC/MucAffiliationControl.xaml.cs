using UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace UWPX_UI.Controls.Chat.MUC
{
    public sealed partial class MucAffiliationControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucAffiliationControlContext VIEW_MODEL = new MucAffiliationControlContext();

        public MUCAffiliation Affiliation
        {
            get { return (MUCAffiliation)GetValue(AffiliationProperty); }
            set { SetValue(AffiliationProperty, value); }
        }
        public static readonly DependencyProperty AffiliationProperty = DependencyProperty.Register(nameof(Affiliation), typeof(MUCAffiliation), typeof(MucAffiliationControl), new PropertyMetadata(MUCAffiliation.NONE, OnAffiliationChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucAffiliationControl()
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
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            VIEW_MODEL.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnAffiliationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MucAffiliationControl control)
            {
                control.UpdateView(e);
            }
        }

        #endregion
    }
}
