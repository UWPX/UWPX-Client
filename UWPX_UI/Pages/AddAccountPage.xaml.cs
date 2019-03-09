using UWPX_UI_Context.Classes.DataContext.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Pages
{
    [TemplateVisualState(Name = STATE_1_VIEW_STATE, GroupName = STATES_GROUP_NAME)]
    public sealed partial class AddAccountPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string STATES_GROUP_NAME = "States";
        private const string STATE_1_VIEW_STATE = "State_1";
        private const string STATE_2_VIEW_STATE = "State_2";

        public readonly AddAccountPageContext VIEW_MODEL = new AddAccountPageContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AddAccountPage()
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
        private void UpdateViewState(string state)
        {
            VisualStateManager.GoToState(this, state, true);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Next_ipbtn_Click(Controls.IconProgressButtonControl sender, RoutedEventArgs args)
        {
            UpdateViewState(STATE_2_VIEW_STATE);
        }

        private void Save_ipbtn_Click(Controls.IconProgressButtonControl sender, RoutedEventArgs args)
        {
            UpdateViewState(STATE_1_VIEW_STATE);
        }

        #endregion
    }
}
