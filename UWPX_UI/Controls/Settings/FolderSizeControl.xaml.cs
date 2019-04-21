using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataContext.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Settings
{
    public sealed partial class FolderSizeControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string FolderPath
        {
            get => (string)GetValue(FolderPathProperty);
            set => SetValue(FolderPathProperty, value);
        }
        public static readonly DependencyProperty FolderPathProperty = DependencyProperty.Register(nameof(FolderPath), typeof(string), typeof(FolderSizeControl), new PropertyMetadata(null, OnFolderPathChanged));

        private readonly FolderSizeControlContext VIEW_MODEL = new FolderSizeControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public FolderSizeControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task RecalculateFolderSizeAsync()
        {
            await VIEW_MODEL.RecalculateFolderSizeAsync(FolderPath);
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task UpdateViewAsync(DependencyPropertyChangedEventArgs e)
        {
            await VIEW_MODEL.UpdateViewAsync(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static async void OnFolderPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FolderSizeControl folderSizeControl)
            {
                await folderSizeControl.UpdateViewAsync(e);
            }
        }

        #endregion
    }
}
