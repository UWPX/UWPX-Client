using System;
using System.Drawing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Toolkit.MasterDetailsView
{
    public partial class MasterDetailsView
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Brush MasterPaneBackground
        {
            get => (Brush)GetValue(MasterPaneBackgroundProperty);
            set => SetValue(MasterPaneBackgroundProperty, value);
        }
        public static readonly DependencyProperty MasterPaneBackgroundProperty = DependencyProperty.Register(nameof(MasterPaneBackground), typeof(Brush), typeof(MasterDetailsView), new PropertyMetadata(null));

        public Brush DetailsPaneBackground
        {
            get => (Brush)GetValue(DetailsPaneBackgroundProperty);
            set => SetValue(DetailsPaneBackgroundProperty, value);
        }
        public static readonly DependencyProperty DetailsPaneBackgroundProperty = DependencyProperty.Register(nameof(DetailsPaneBackground), typeof(Brush), typeof(MasterDetailsView), new PropertyMetadata(null));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(MasterDetailsView), new PropertyMetadata(null, OnSelectedItemChanged));

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(MasterDetailsView), new PropertyMetadata(-1, OnSelectedIndexChanged));

        public object NoItemsContent
        {
            get => GetValue(NoItemsContentProperty);
            set => SetValue(NoItemsContentProperty, value);
        }
        public static readonly DependencyProperty NoItemsContentProperty = DependencyProperty.Register(nameof(NoItemsContent), typeof(object), typeof(MasterDetailsView), new PropertyMetadata(null));

        public DataTemplate NoItemsContentTemplate
        {
            get => (DataTemplate)GetValue(NoItemsContentTemplateProperty);
            set => SetValue(NoItemsContentTemplateProperty, value);
        }
        public static readonly DependencyProperty NoItemsContentTemplateProperty = DependencyProperty.Register(nameof(NoItemsContentTemplate), typeof(DataTemplate), typeof(MasterDetailsView), new PropertyMetadata(null));

        public object MasterHeader
        {
            get => GetValue(MasterHeaderProperty);
            set => SetValue(MasterHeaderProperty, value);
        }
        public static readonly DependencyProperty MasterHeaderProperty = DependencyProperty.Register(nameof(MasterHeader), typeof(object), typeof(MasterDetailsView), new PropertyMetadata(null));

        public DataTemplate MasterHeaderTemplate
        {
            get => (DataTemplate)GetValue(MasterHeaderTemplateProperty);
            set => SetValue(MasterHeaderTemplateProperty, value);
        }
        public static readonly DependencyProperty MasterHeaderTemplateProperty = DependencyProperty.Register(nameof(MasterHeaderTemplate), typeof(DataTemplate), typeof(MasterDetailsView), new PropertyMetadata(null));

        public object DetailsHeader
        {
            get => GetValue(DetailsHeaderProperty);
            set => SetValue(DetailsHeaderProperty, value);
        }
        public static readonly DependencyProperty DetailsHeaderProperty = DependencyProperty.Register(nameof(DetailsHeader), typeof(object), typeof(MasterDetailsView), new PropertyMetadata(null));

        public DataTemplate DetailsHeaderTemplate
        {
            get => (DataTemplate)GetValue(DetailsHeaderTemplateProperty);
            set => SetValue(DetailsHeaderTemplateProperty, value);
        }
        public static readonly DependencyProperty DetailsHeaderTemplateProperty = DependencyProperty.Register(nameof(DetailsHeaderTemplate), typeof(DataTemplate), typeof(MasterDetailsView), new PropertyMetadata(null));

        public object NoSelectionContent
        {
            get => GetValue(NoSelectionContentProperty);
            set => SetValue(NoSelectionContentProperty, value);
        }
        public static readonly DependencyProperty NoSelectionContentProperty = DependencyProperty.Register(nameof(NoSelectionContent), typeof(object), typeof(MasterDetailsView), new PropertyMetadata(null));

        public DataTemplate NoSelectionContentTemplate
        {
            get => (DataTemplate)GetValue(NoSelectionContentTemplateProperty);
            set => SetValue(NoSelectionContentTemplateProperty, value);
        }
        public static readonly DependencyProperty NoSelectionContentTemplateProperty = DependencyProperty.Register(nameof(NoSelectionContentTemplate), typeof(DataTemplate), typeof(MasterDetailsView), new PropertyMetadata(null));

        public DataTemplateSelector DetailsContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(DetailsContentTemplateSelectorProperty);
            set => SetValue(DetailsContentTemplateSelectorProperty, value);
        }
        public static readonly DependencyProperty DetailsContentTemplateSelectorProperty = DependencyProperty.Register(nameof(DetailsContentTemplateSelector), typeof(DataTemplateSelector), typeof(MasterDetailsView), new PropertyMetadata(null));

        public DataTemplate DetailsTemplate
        {
            get => (DataTemplate)GetValue(DetailsTemplateProperty);
            set => SetValue(DetailsTemplateProperty, value);
        }
        public static readonly DependencyProperty DetailsTemplateProperty = DependencyProperty.Register(nameof(DetailsTemplate), typeof(DataTemplate), typeof(MasterDetailsView), new PropertyMetadata(null));

        public DataTemplateSelector MasterItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(MasterItemTemplateSelectorProperty);
            set => SetValue(MasterItemTemplateSelectorProperty, value);
        }
        public static readonly DependencyProperty MasterItemTemplateSelectorProperty = DependencyProperty.Register(nameof(MasterItemTemplateSelector), typeof(DataTemplateSelector), typeof(MasterDetailsView), new PropertyMetadata(null));

        public GridLength MasterPaneWidth
        {
            get => (GridLength)GetValue(MasterPaneWidthProperty);
            set => SetValue(MasterPaneWidthProperty, value);
        }
        public static readonly DependencyProperty MasterPaneWidthProperty = DependencyProperty.Register(nameof(MasterPaneWidth), typeof(GridLength), typeof(MasterDetailsView), new PropertyMetadata(new GridLength(320)));

        public double CompactModeThresholdWidth
        {
            get => (double)GetValue(CompactModeThresholdWidthProperty);
            set => SetValue(CompactModeThresholdWidthProperty, value);
        }
        public static readonly DependencyProperty CompactModeThresholdWidthProperty = DependencyProperty.Register(nameof(CompactModeThresholdWidth), typeof(double), typeof(MasterDetailsView), new PropertyMetadata(640d, OnCompactModeThresholdWidthChanged));

        public CommandBar MasterCommandBar
        {
            get => (CommandBar)GetValue(MasterCommandBarProperty);
            set => SetValue(MasterCommandBarProperty, value);
        }
        public static readonly DependencyProperty MasterCommandBarProperty = DependencyProperty.Register(nameof(MasterCommandBar), typeof(CommandBar), typeof(MasterDetailsView), new PropertyMetadata(null, OnMasterCommandBarChanged));

        public CommandBar DetailsCommandBar
        {
            get => (CommandBar)GetValue(DetailsCommandBarProperty);
            set => SetValue(DetailsCommandBarProperty, value);
        }
        public static readonly DependencyProperty DetailsCommandBarProperty = DependencyProperty.Register(nameof(DetailsCommandBar), typeof(CommandBar), typeof(MasterDetailsView), new PropertyMetadata(null, OnDetailsCommandBarChanged));

        public BackButtonBehavior BackButtonBehavior
        {
            get => (BackButtonBehavior)GetValue(BackButtonBehaviorProperty);
            set => SetValue(BackButtonBehaviorProperty, value);
        }
        public static readonly DependencyProperty BackButtonBehaviorProperty = DependencyProperty.Register(nameof(BackButtonBehavior), typeof(BackButtonBehavior), typeof(MasterDetailsView), new PropertyMetadata(null, OnBackButtonBehaviorChanged));

        public MasterDetailsViewState ViewState
        {
            get => (MasterDetailsViewState)GetValue(ViewStateProperty);
            set => SetValue(ViewStateProperty, value);
        }
        public static readonly DependencyProperty ViewStateProperty = DependencyProperty.Register(nameof(ViewState), typeof(MasterDetailsViewState), typeof(MasterDetailsView), new PropertyMetadata(default(MasterDetailsViewState)));

        /// <summary>
        /// Gets or sets a function for mapping the selected item to a different model.
        /// This new model will be the DataContext of the Details area.
        /// </summary>
        public Func<object, object> MapDetails { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


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
        private static void OnDetailsCommandBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MasterDetailsView)d).OnDetailsCommandBarChanged();
        }

        private static void OnMasterCommandBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MasterDetailsView)d).OnMasterCommandBarChanged();
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MasterDetailsView)d).OnSelectedItemChanged(e);
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MasterDetailsView)d).OnSelectedIndexChanged(e);
        }

        private static void OnCompactModeThresholdWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

        private static void OnBackButtonBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

        #endregion
    }
}
