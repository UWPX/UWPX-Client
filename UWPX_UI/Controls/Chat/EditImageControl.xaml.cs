using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UWPX_UI_Context.Classes.DataContext.Controls.Chat;
using Windows.Graphics.Imaging;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class EditImageControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly EditImageControlContext VIEW_MODEL = new EditImageControlContext();
        private readonly Stack<InkStroke> STROKE_STACK = new Stack<InkStroke>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public EditImageControl()
        {
            InitializeComponent();
            inputModeToggleBtn.IsChecked = UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void SetImageAsync(SoftwareBitmap img)
        {

        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnInputModeCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (inputModeToggleBtn.IsChecked == true)
            {
                inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;
            }
            else
            {
                inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen;
            }
        }

        private void OnResetImageClicked(object sender, RoutedEventArgs e)
        {
            STROKE_STACK.Clear();
            inkCanvas.InkPresenter.StrokeContainer.Clear();
        }

        private void OnUndoClicked(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<InkStroke> strokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
            if (strokes.Count > 0)
            {
                strokes.Last().Selected = true;
                InkStroke stroke = strokes.Last();
                stroke.Selected = true;
                STROKE_STACK.Push(stroke);
                inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
            }
        }

        private void OnRedoClicked(object sender, RoutedEventArgs e)
        {
            if (STROKE_STACK.TryPop(out InkStroke stroke))
            {
                InkStrokeBuilder strokeBuilder = new InkStrokeBuilder();
                strokeBuilder.SetDefaultDrawingAttributes(stroke.DrawingAttributes);
                Matrix3x2 matr = stroke.PointTransform;
                IReadOnlyList<InkPoint> inkPoints = stroke.GetInkPoints();
                inkCanvas.InkPresenter.StrokeContainer.AddStroke(strokeBuilder.CreateStrokeFromInkPoints(inkPoints, matr));
            }
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void OnSendClicked(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        #endregion
    }
}
