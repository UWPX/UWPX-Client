using UWPX_UI_Context.Classes.DataContext.Controls.Chat.MUC;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat.MUC
{
    public sealed partial class MucSuggestionsControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MucSuggestionsControlContext VIEW_MODEL = new MucSuggestionsControlContext();

        public string RoomBareJid
        {
            get => (string)GetValue(RoomBareJidProperty);
            set => SetValue(RoomBareJidProperty, value);
        }
        public static readonly DependencyProperty RoomBareJidProperty = DependencyProperty.Register(nameof(RoomBareJid), typeof(string), typeof(MucSuggestionsControl), new PropertyMetadata(""));

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }
        public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register(nameof(IsValid), typeof(bool), typeof(MucSuggestionsControl), new PropertyMetadata(false));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucSuggestionsControl()
        {
            InitializeComponent();
            // Required since the IsValid property does not update if initialized when the dialog is not being shown:
            bareJid_jipc.VIEW_MODEL.MODEL.PropertyChanged += MODEL_PropertyChanged;
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
        private void MODEL_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BareJidInputControlDataTemplate.IsValid):
                    IsValid = bareJid_jipc.VIEW_MODEL.MODEL.IsValid;
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}
