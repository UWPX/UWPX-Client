using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;

namespace UWPX_UI.Controls.Settings
{
    public sealed partial class ServerProviderControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ServerProviderControlContext VIEW_MODEL = new ServerProviderControlContext();

        public Provider ServerProvider
        {
            get => (Provider)GetValue(ServerProviderProperty);
            set => SetValue(ServerProviderProperty, value);
        }
        public static readonly DependencyProperty ServerProviderProperty = DependencyProperty.Register(nameof(ServerProvider), typeof(Provider), typeof(ServerProviderControl), new PropertyMetadata(null));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ServerProviderControl()
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
