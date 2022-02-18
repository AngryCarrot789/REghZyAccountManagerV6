using System;
using System.Collections;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace REghZyAccountManagerV6.Controls {
    public class AutoCloseGridSplitter : Thumb {
        public static readonly DependencyProperty ResizeDirectionProperty =
            DependencyProperty.Register(
                nameof(ResizeDirection),
                typeof(GridResizeDirection),
                typeof(AutoCloseGridSplitter),
                new FrameworkPropertyMetadata(GridResizeDirection.Auto, UpdateCursor), IsValidResizeDirection);

        public static readonly DependencyProperty ResizeBehaviorProperty =
            DependencyProperty.Register(
                nameof(ResizeBehavior),
                typeof(GridResizeBehavior),
                typeof(AutoCloseGridSplitter),
                new FrameworkPropertyMetadata(GridResizeBehavior.BasedOnAlignment), IsValidResizeBehavior);

        public static readonly DependencyProperty ShowsPreviewProperty =
            DependencyProperty.Register(
                nameof(ShowsPreview),
                typeof(bool),
                typeof(AutoCloseGridSplitter),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty PreviewStyleProperty =
            DependencyProperty.Register(
                nameof(PreviewStyle),
                typeof(Style),
                typeof(AutoCloseGridSplitter),
                new FrameworkPropertyMetadata((object) null));

        public static readonly DependencyProperty KeyboardIncrementProperty =
            DependencyProperty.Register(
                nameof(KeyboardIncrement),
                typeof(double),
                typeof(AutoCloseGridSplitter),
                new FrameworkPropertyMetadata(10.0), IsValidDelta);

        public static readonly DependencyProperty DragIncrementProperty =
            DependencyProperty.Register(
            nameof(DragIncrement),
            typeof(double),
            typeof(AutoCloseGridSplitter),
            new FrameworkPropertyMetadata(1.0), IsValidDelta);

        private ResizeData resizeData;

        static AutoCloseGridSplitter() {
            EventManager.RegisterClassHandler(typeof(AutoCloseGridSplitter), DragStartedEvent, new DragStartedEventHandler(OnDragStarted));
            EventManager.RegisterClassHandler(typeof(AutoCloseGridSplitter), DragDeltaEvent, new DragDeltaEventHandler(OnDragDelta));
            EventManager.RegisterClassHandler(typeof(AutoCloseGridSplitter), DragCompletedEvent, new DragCompletedEventHandler(OnDragCompleted));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCloseGridSplitter), new FrameworkPropertyMetadata(typeof(AutoCloseGridSplitter)));
            FocusableProperty.OverrideMetadata(typeof(AutoCloseGridSplitter), new FrameworkPropertyMetadata(true));
            HorizontalAlignmentProperty.OverrideMetadata(typeof(AutoCloseGridSplitter), new FrameworkPropertyMetadata(HorizontalAlignment.Right));
            CursorProperty.OverrideMetadata(typeof(AutoCloseGridSplitter), new FrameworkPropertyMetadata(null, CoerceCursor));
        }

        private static void UpdateCursor(DependencyObject o, DependencyPropertyChangedEventArgs e) => o.CoerceValue(CursorProperty);

        private static object CoerceCursor(DependencyObject obj, object value) {
            AutoCloseGridSplitter splitter = (AutoCloseGridSplitter) obj;
            if (value == null) {
                switch (splitter.GetEffectiveResizeDirection()) {
                    case GridResizeDirection.Columns: return Cursors.SizeWE;
                    case GridResizeDirection.Rows: return Cursors.SizeNS;
                }
            }

            return value;
        }

        public GridResizeDirection ResizeDirection {
            get => (GridResizeDirection) this.GetValue(ResizeDirectionProperty);
            set => this.SetValue(ResizeDirectionProperty, value);
        }

        private static bool IsValidResizeDirection(object o) {
            GridResizeDirection direction = (GridResizeDirection) o;
            switch (direction) {
                case GridResizeDirection.Auto:
                case GridResizeDirection.Columns:
                    return true;
                default: return direction == GridResizeDirection.Rows;
            }
        }

        public GridResizeBehavior ResizeBehavior {
            get => (GridResizeBehavior) this.GetValue(ResizeBehaviorProperty);
            set => this.SetValue(ResizeBehaviorProperty, value);
        }

        private static bool IsValidResizeBehavior(object o) {
            GridResizeBehavior gridResizeBehavior = (GridResizeBehavior) o;
            switch (gridResizeBehavior) {
                case GridResizeBehavior.BasedOnAlignment:
                case GridResizeBehavior.CurrentAndNext:
                case GridResizeBehavior.PreviousAndCurrent:
                    return true;
                default: return gridResizeBehavior == GridResizeBehavior.PreviousAndNext;
            }
        }

        public bool ShowsPreview {
            get => (bool) this.GetValue(ShowsPreviewProperty);
            set => this.SetValue(ShowsPreviewProperty, value);
        }

        public Style PreviewStyle {
            get => (Style) this.GetValue(PreviewStyleProperty);
            set => this.SetValue(PreviewStyleProperty, value);
        }

        public double KeyboardIncrement {
            get => (double) this.GetValue(KeyboardIncrementProperty);
            set => this.SetValue(KeyboardIncrementProperty, value);
        }

        private static bool IsValidDelta(object o) {
            double d = (double) o;
            return d > 0.0 && !double.IsPositiveInfinity(d);
        }

        public double DragIncrement {
            get => (double) this.GetValue(DragIncrementProperty);
            set => this.SetValue(DragIncrementProperty, value);
        }

        public class AutoCloseGridSplitterAutomationPeer : ThumbAutomationPeer {
            public AutoCloseGridSplitterAutomationPeer(Thumb owner) : base(owner) {

            }

            protected override string GetClassNameCore() => "GridSplitter";

            public override object GetPattern(PatternInterface patternInterface) {
                if (patternInterface == PatternInterface.Transform) {
                    return this;
                }
                else {
                    return base.GetPattern(patternInterface);
                }
            }

            void Resize(double width, double height) => throw new InvalidOperationException("Cannot resize with width and height");

            void Rotate(double degrees) => throw new InvalidOperationException("Cannot resize with degrees");
        }

        protected override AutomationPeer OnCreateAutomationPeer() => new AutoCloseGridSplitterAutomationPeer(this);

        private GridResizeDirection GetEffectiveResizeDirection() {
            GridResizeDirection gridResizeDirection = this.ResizeDirection;
            if (gridResizeDirection == GridResizeDirection.Auto)
                gridResizeDirection = this.HorizontalAlignment == HorizontalAlignment.Stretch ? (this.VerticalAlignment == VerticalAlignment.Stretch ? (this.ActualWidth > this.ActualHeight ? GridResizeDirection.Rows : GridResizeDirection.Columns) : GridResizeDirection.Rows) : GridResizeDirection.Columns;
            return gridResizeDirection;
        }

        private GridResizeBehavior GetEffectiveResizeBehavior(GridResizeDirection direction) {
            GridResizeBehavior gridResizeBehavior = this.ResizeBehavior;
            if (gridResizeBehavior == GridResizeBehavior.BasedOnAlignment) {
                if (direction == GridResizeDirection.Columns) {
                    switch (this.HorizontalAlignment) {
                        case HorizontalAlignment.Left:
                            gridResizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                            break;
                        case HorizontalAlignment.Right:
                            gridResizeBehavior = GridResizeBehavior.CurrentAndNext;
                            break;
                        default:
                            gridResizeBehavior = GridResizeBehavior.PreviousAndNext;
                            break;
                    }
                }
                else {
                    switch (this.VerticalAlignment) {
                        case VerticalAlignment.Top:
                            gridResizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                            break;
                        case VerticalAlignment.Bottom:
                            gridResizeBehavior = GridResizeBehavior.CurrentAndNext;
                            break;
                        default:
                            gridResizeBehavior = GridResizeBehavior.PreviousAndNext;
                            break;
                    }
                }
            }

            return gridResizeBehavior;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);
            this.CoerceValue(CursorProperty);
        }

        private void RemovePreviewAdorner() {
            if (this.resizeData.Adorner == null)
                return;
            (VisualTreeHelper.GetParent(this.resizeData.Adorner) as AdornerLayer).Remove(this.resizeData.Adorner);
        }

        private void InitializeData(bool ShowsPreview) {
            if (!(this.Parent is Grid parent))
                return;
            this.resizeData = new ResizeData();
            this.resizeData.Grid = parent;
            this.resizeData.ShowsPreview = ShowsPreview;
            this.resizeData.ResizeDirection = this.GetEffectiveResizeDirection();
            this.resizeData.ResizeBehavior = this.GetEffectiveResizeBehavior(this.resizeData.ResizeDirection);
            this.resizeData.SplitterLength = Math.Min(this.ActualWidth, this.ActualHeight);
            if (!this.SetupDefinitionsToResize())
                this.resizeData = null;
            else
                this.SetupPreview();
        }

        private bool SetupDefinitionsToResize() {
            if ((int) this.GetValue(this.resizeData.ResizeDirection == GridResizeDirection.Columns ? Grid.ColumnSpanProperty : Grid.RowSpanProperty) == 1) {
                int num1 = (int) this.GetValue(this.resizeData.ResizeDirection == GridResizeDirection.Columns ? Grid.ColumnProperty : Grid.RowProperty);
                int index1;
                int index2;
                switch (this.resizeData.ResizeBehavior) {
                    case GridResizeBehavior.CurrentAndNext:
                        index1 = num1;
                        index2 = num1 + 1;
                        break;
                    case GridResizeBehavior.PreviousAndCurrent:
                        index1 = num1 - 1;
                        index2 = num1;
                        break;
                    default:
                        index1 = num1 - 1;
                        index2 = num1 + 1;
                        break;
                }

                int num2 = this.resizeData.ResizeDirection == GridResizeDirection.Columns ? this.resizeData.Grid.ColumnDefinitions.Count : this.resizeData.Grid.RowDefinitions.Count;
                if (index1 >= 0 && index2 < num2) {
                    this.resizeData.SplitterIndex = num1;
                    this.resizeData.Definition1Index = index1;
                    this.resizeData.Definition1 = GetGridDefinition(this.resizeData.Grid, index1, this.resizeData.ResizeDirection);
                    this.resizeData.OriginalDefinition1Length = (this.resizeData.Definition1 is ColumnDefinition col1 ? col1.Width : ((RowDefinition) this.resizeData.Definition1).Height);
                    this.resizeData.OriginalDefinition1ActualLength = this.GetActualLength(this.resizeData.Definition1);
                    this.resizeData.Definition2Index = index2;
                    this.resizeData.Definition2 = GetGridDefinition(this.resizeData.Grid, index2, this.resizeData.ResizeDirection);
                    this.resizeData.OriginalDefinition2Length = (this.resizeData.Definition2 is ColumnDefinition col2 ? col2.Width : ((RowDefinition) this.resizeData.Definition2).Height);
                    this.resizeData.OriginalDefinition2ActualLength = this.GetActualLength(this.resizeData.Definition2);
                    bool flag1 = IsStar(this.resizeData.Definition1);
                    bool flag2 = IsStar(this.resizeData.Definition2);
                    this.resizeData.SplitBehavior = !(flag1 & flag2) ? (!flag1 ? SplitBehavior.Resize1 : SplitBehavior.Resize2) : SplitBehavior.Split;
                    return true;
                }
            }

            return false;
        }

        private void SetupPreview() {
            if (!this.resizeData.ShowsPreview)
                return;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.resizeData.Grid);
            if (adornerLayer == null)
                return;
            this.resizeData.Adorner = new PreviewAdorner(this, this.PreviewStyle);
            adornerLayer.Add(this.resizeData.Adorner);
            this.GetDeltaConstraints(out this.resizeData.MinChange, out this.resizeData.MaxChange);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            base.OnLostKeyboardFocus(e);
            if (this.resizeData == null)
                return;
            this.CancelResize();
        }

        private static void OnDragStarted(object sender, DragStartedEventArgs e) => (sender as AutoCloseGridSplitter).OnDragStarted(e);

        private void OnDragStarted(DragStartedEventArgs e) => this.InitializeData(this.ShowsPreview);

        private static void OnDragDelta(object sender, DragDeltaEventArgs e) => (sender as AutoCloseGridSplitter).OnDragDelta(e);

        private void OnDragDelta(DragDeltaEventArgs e) {
            if (this.resizeData == null)
                return;
            double horizontalChange = e.HorizontalChange;
            double verticalChange = e.VerticalChange;
            double dragIncrement = this.DragIncrement;
            double num1 = Math.Round(horizontalChange / dragIncrement) * dragIncrement;
            double num2 = Math.Round(verticalChange / dragIncrement) * dragIncrement;
            if (this.resizeData.ShowsPreview) {
                if (this.resizeData.ResizeDirection == GridResizeDirection.Columns)
                    this.resizeData.Adorner.OffsetX = Math.Min(Math.Max(num1, this.resizeData.MinChange), this.resizeData.MaxChange);
                else
                    this.resizeData.Adorner.OffsetY = Math.Min(Math.Max(num2, this.resizeData.MinChange), this.resizeData.MaxChange);
            }
            else
                this.MoveSplitter(num1, num2);
        }

        private static void OnDragCompleted(object sender, DragCompletedEventArgs e) => (sender as AutoCloseGridSplitter).OnDragCompleted(e);

        private void OnDragCompleted(DragCompletedEventArgs e) {
            if (this.resizeData == null)
                return;
            if (this.resizeData.ShowsPreview) {
                this.MoveSplitter(this.resizeData.Adorner.OffsetX, this.resizeData.Adorner.OffsetY);
                this.RemovePreviewAdorner();
            }

            this.resizeData = null;
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            switch (e.Key) {
                case Key.Escape:
                    if (this.resizeData == null)
                        break;
                    this.CancelResize();
                    e.Handled = true;
                    break;
                case Key.Left:
                    e.Handled = this.KeyboardMoveSplitter(-this.KeyboardIncrement, 0.0);
                    break;
                case Key.Up:
                    e.Handled = this.KeyboardMoveSplitter(0.0, -this.KeyboardIncrement);
                    break;
                case Key.Right:
                    e.Handled = this.KeyboardMoveSplitter(this.KeyboardIncrement, 0.0);
                    break;
                case Key.Down:
                    e.Handled = this.KeyboardMoveSplitter(0.0, this.KeyboardIncrement);
                    break;
            }
        }

        private void CancelResize() {
            Grid parent = this.Parent as Grid;
            if (this.resizeData.ShowsPreview) {
                this.RemovePreviewAdorner();
            }
            else {
                SetDefinitionLength(this.resizeData.Definition1, this.resizeData.OriginalDefinition1Length);
                SetDefinitionLength(this.resizeData.Definition2, this.resizeData.OriginalDefinition2Length);
            }

            this.resizeData = null;
        }

        private static bool IsStar(DefinitionBase definition) {
            return definition is ColumnDefinition ? ((ColumnDefinition) definition).Width.IsStar : ((RowDefinition) definition).Height.IsStar;
        }

        private static DefinitionBase GetGridDefinition(
            Grid grid,
            int index,
            GridResizeDirection direction) {
            return direction != GridResizeDirection.Columns ? grid.RowDefinitions[index] : (DefinitionBase) grid.ColumnDefinitions[index];
        }

        private double GetActualLength(DefinitionBase definition) => definition is ColumnDefinition columnDefinition ? columnDefinition.ActualWidth : ((RowDefinition) definition).ActualHeight;

        private static void SetDefinitionLength(DefinitionBase definition, GridLength length) => definition.SetValue(definition is ColumnDefinition ? ColumnDefinition.WidthProperty : RowDefinition.HeightProperty, length);

        private void GetDeltaConstraints(out double minDelta, out double maxDelta) {
            double actualLength1 = this.GetActualLength(this.resizeData.Definition1);
            double val1_1 = this.resizeData.Definition1 is ColumnDefinition ? ((ColumnDefinition) this.resizeData.Definition1).MinWidth : ((RowDefinition) this.resizeData.Definition1).MinHeight;
            double maxSizeValueCache1 = this.resizeData.Definition1 is ColumnDefinition ? ((ColumnDefinition) this.resizeData.Definition1).MaxWidth : ((RowDefinition) this.resizeData.Definition1).MaxHeight;
            double actualLength2 = this.GetActualLength(this.resizeData.Definition2);
            double val1_2 = this.resizeData.Definition2 is ColumnDefinition ? ((ColumnDefinition) this.resizeData.Definition2).MinWidth : ((RowDefinition) this.resizeData.Definition2).MinHeight;
            double maxSizeValueCache2 = this.resizeData.Definition2 is ColumnDefinition ? ((ColumnDefinition) this.resizeData.Definition2).MaxWidth : ((RowDefinition) this.resizeData.Definition2).MaxHeight;
            if (this.resizeData.SplitterIndex == this.resizeData.Definition1Index)
                val1_1 = Math.Max(val1_1, this.resizeData.SplitterLength);
            else if (this.resizeData.SplitterIndex == this.resizeData.Definition2Index)
                val1_2 = Math.Max(val1_2, this.resizeData.SplitterLength);
            if (this.resizeData.SplitBehavior == SplitBehavior.Split) {
                minDelta = -Math.Min(actualLength1 - val1_1, maxSizeValueCache2 - actualLength2);
                maxDelta = Math.Min(maxSizeValueCache1 - actualLength1, actualLength2 - val1_2);
            }
            else if (this.resizeData.SplitBehavior == SplitBehavior.Resize1) {
                minDelta = val1_1 - actualLength1;
                maxDelta = maxSizeValueCache1 - actualLength1;
            }
            else {
                minDelta = actualLength2 - maxSizeValueCache2;
                maxDelta = actualLength2 - val1_2;
            }
        }

        private void SetLengths(double definition1Pixels, double definition2Pixels) {
            if (this.resizeData.SplitBehavior == SplitBehavior.Split) {
                IEnumerable enumerable = this.resizeData.ResizeDirection == GridResizeDirection.Columns ? (IEnumerable) this.resizeData.Grid.ColumnDefinitions : (IEnumerable) this.resizeData.Grid.RowDefinitions;
                int num = 0;
                foreach (DefinitionBase definition in enumerable) {
                    if (num == this.resizeData.Definition1Index)
                        SetDefinitionLength(definition, new GridLength(definition1Pixels, GridUnitType.Star));
                    else if (num == this.resizeData.Definition2Index)
                        SetDefinitionLength(definition, new GridLength(definition2Pixels, GridUnitType.Star));
                    else if (IsStar(definition))
                        SetDefinitionLength(definition, new GridLength(this.GetActualLength(definition), GridUnitType.Star));
                    ++num;
                }
            }
            else if (this.resizeData.SplitBehavior == SplitBehavior.Resize1)
                SetDefinitionLength(this.resizeData.Definition1, new GridLength(definition1Pixels));
            else
                SetDefinitionLength(this.resizeData.Definition2, new GridLength(definition2Pixels));
        }

        private bool AreClose(double a, double b) {
            if (a == b) {
                return true;
            }

            double difference = a - b;
            return difference < 1.53E-06 && difference > -1.53E-06;
        }

        private void MoveSplitter(double horizontalChange, double verticalChange) {
            double move;
            if (this.resizeData.ResizeDirection == GridResizeDirection.Columns) {
                move = horizontalChange;
            }
            else {
                move = verticalChange;
            }

            DefinitionBase definition1 = this.resizeData.Definition1;
            DefinitionBase definition2 = this.resizeData.Definition2;
            if (definition1 == null || definition2 == null)
                return;
            double actualLength1 = this.GetActualLength(definition1);
            double actualLength2 = this.GetActualLength(definition2);
            if (this.resizeData.SplitBehavior == SplitBehavior.Split && !AreClose(actualLength1 + actualLength2, this.resizeData.OriginalDefinition1ActualLength + this.resizeData.OriginalDefinition2ActualLength)) {
                this.CancelResize();
            }
            else {
                double minDelta;
                double maxDelta;
                this.GetDeltaConstraints(out minDelta, out maxDelta);
                if (this.FlowDirection != this.resizeData.Grid.FlowDirection)
                    move = -move;
                double num = Math.Min(Math.Max(move, minDelta), maxDelta);
                double definition1Pixels = actualLength1 + num;
                double definition2Pixels = actualLength1 + actualLength2 - definition1Pixels;
                this.SetLengths(definition1Pixels, definition2Pixels);
            }
        }

        internal bool KeyboardMoveSplitter(double horizontalChange, double verticalChange) {
            if (this.resizeData != null)
                return false;
            this.InitializeData(false);
            if (this.resizeData == null)
                return false;
            if (this.FlowDirection == FlowDirection.RightToLeft)
                horizontalChange = -horizontalChange;
            this.MoveSplitter(horizontalChange, verticalChange);
            this.resizeData = null;
            return true;
        }

        private sealed class PreviewAdorner : Adorner {
            private TranslateTransform Translation;
            private Decorator decorator;

            public PreviewAdorner(AutoCloseGridSplitter AutoCloseGridSplitter, Style previewStyle)
                : base(AutoCloseGridSplitter) {
                Control control = new Control();
                control.Style = previewStyle;
                control.IsEnabled = false;
                this.Translation = new TranslateTransform();
                this.decorator = new Decorator();
                this.decorator.Child = control;
                this.decorator.RenderTransform = this.Translation;
                this.AddVisualChild(this.decorator);
            }

            protected override Visual GetVisualChild(int index) {
                if (index != 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index out of range: " + index);
                return this.decorator;
            }

            protected override int VisualChildrenCount => 1;

            protected override Size ArrangeOverride(Size finalSize) {
                this.decorator.Arrange(new Rect(new Point(), finalSize));
                return finalSize;
            }

            public double OffsetX {
                get => this.Translation.X;
                set => this.Translation.X = value;
            }

            public double OffsetY {
                get => this.Translation.Y;
                set => this.Translation.Y = value;
            }
        }

        private enum SplitBehavior {
            Split,
            Resize1,
            Resize2,
        }

        private class ResizeData {
            public bool ShowsPreview;
            public PreviewAdorner Adorner;
            public double MinChange;
            public double MaxChange;
            public Grid Grid;
            public GridResizeDirection ResizeDirection;
            public GridResizeBehavior ResizeBehavior;
            public DefinitionBase Definition1;
            public DefinitionBase Definition2;
            public SplitBehavior SplitBehavior;
            public int SplitterIndex;
            public int Definition1Index;
            public int Definition2Index;
            public GridLength OriginalDefinition1Length;
            public GridLength OriginalDefinition2Length;
            public double OriginalDefinition1ActualLength;
            public double OriginalDefinition2ActualLength;
            public double SplitterLength;
        }
    }
}