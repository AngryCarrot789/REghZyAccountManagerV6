using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace REghZyAccountManagerV6.Controls {
    public class AutoClosingGridSplitter : GridSplitter {
        public static readonly DependencyProperty UseAutoCloseProperty =
            DependencyProperty.Register(
                nameof(UseAutoClose),
                typeof(bool),
                typeof(AutoClosingGridSplitter),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MinSizeForAutoCloseProperty =
            DependencyProperty.Register(
                nameof(MinSizeForAutoClose),
                typeof(double),
                typeof(AutoClosingGridSplitter),
                new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ReopenCompareValueProperty =
            DependencyProperty.Register(
                nameof(ReopenCompareValue),
                typeof(int),
                typeof(AutoClosingGridSplitter),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty TargetDefinitionProperty =
            DependencyProperty.Register(
                nameof(TargetDefinition),
                typeof(DefinitionBase),
                typeof(AutoClosingGridSplitter),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
                nameof(IsOpen),
                typeof(bool),
                typeof(AutoClosingGridSplitter),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        private bool isSettingOpenState = false;

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is AutoClosingGridSplitter splitter) {
                if (splitter.isSettingOpenState) {
                    return;
                }

                if (e.NewValue == e.OldValue) {
                    return;
                }

                DefinitionBase definition = splitter.TargetDefinition;
                if (definition == null) {
                    return;
                }

                if (definition is ColumnDefinition column) {

                    if ((bool) e.NewValue) {
                        column.Width = new GridLength(splitter.MinSizeForAutoClose + 1);
                    }
                    else {
                        column.Width = new GridLength(0.0d);
                    }
                }
            }
        }

        public bool UseAutoClose {
            get => (bool) GetValue(UseAutoCloseProperty);
            set => SetValue(UseAutoCloseProperty, value);
        }

        public double MinSizeForAutoClose {
            get => (double) GetValue(MinSizeForAutoCloseProperty);
            set => SetValue(MinSizeForAutoCloseProperty, value);
        }

        public int ReopenCompareValue {
            get => (int) GetValue(ReopenCompareValueProperty);
            set => SetValue(ReopenCompareValueProperty, value);
        }

        public DefinitionBase TargetDefinition {
            get => (DefinitionBase) GetValue(TargetDefinitionProperty);
            set => SetValue(TargetDefinitionProperty, value);
        }

        public bool IsOpen {
            get => (bool) GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        private bool isDragging;
        private bool isGhostResizing;
        private double prevIncrement;
        private bool isClosed;

        public AutoClosingGridSplitter() {
            DragDelta += OnDragDelta;
            this.DragStarted += OnDragStarted;
            this.DragCompleted += OnDragCompleted;
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e) {
            this.isDragging = true;
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e) {
            this.isDragging = false;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e) {
            base.OnMouseDoubleClick(e);
            this.IsOpen = !this.IsOpen;
        }

        private Point origin;
        private void OnDragDelta(object sender, DragDeltaEventArgs e) {
            DefinitionBase definition = this.TargetDefinition;
            if (definition == null) {
                return;
            }

            if (definition is ColumnDefinition column) {
                double sizeForAutoClose = this.MinSizeForAutoClose;
                if (column.ActualWidth >= sizeForAutoClose && this.isGhostResizing) {
                    this.isGhostResizing = false;
                    this.isClosed = false;
                    this.DragIncrement = this.prevIncrement;
                    this.skip = false;
                }

                if (this.resetDragIncrement) {
                    if (column.ActualWidth >= sizeForAutoClose) {
                        if (Math.Abs(this.origin.X - Mouse.GetPosition(null).X) >= (sizeForAutoClose / 2.0d)) {
                            this.DragIncrement = this.prevIncrement;
                            this.resetDragIncrement = false;
                        }
                    }
                }

                if (double.IsNaN(sizeForAutoClose) || double.IsPositiveInfinity(sizeForAutoClose)) {
                    return;
                }

                if (this.isGhostResizing) {
                    if (column.ActualWidth > 0.0d) {
                        return;
                    }

                    this.isGhostResizing = false;
                    this.isClosed = true;
                }
                else if (this.isClosed && column.ActualWidth >= sizeForAutoClose) {
                    this.origin = Mouse.GetPosition(null);
                    this.resetDragIncrement = true;
                    this.isClosed = false;
                    this.skip = true;
                }
                else if (column.ActualWidth == 0.0d) {
                    if (this.isGhostResizing) {
                        this.isGhostResizing = true;
                    }
                }
                else if (column.ActualWidth < sizeForAutoClose && !this.isGhostResizing && !this.isClosed) {
                    if (this.skip) {
                        return;
                    }

                    this.prevIncrement = this.DragIncrement;
                    this.DragIncrement = this.MinSizeForAutoClose;
                    this.isGhostResizing = true;
                    this.skip = true;
                }
                else if (column.ActualWidth > sizeForAutoClose) {
                    if (this.isGhostResizing) {
                        this.isGhostResizing = false;
                        this.isClosed = false;
                        this.DragIncrement = this.prevIncrement;
                    }

                    this.skip = false;
                }

                this.isSettingOpenState = true;
                if (column.ActualWidth != 0.0d) {
                    if (!this.IsOpen) {
                        this.IsOpen = true;
                    }
                }
                else if (this.IsOpen) {
                    this.IsOpen = false;
                }

                this.isSettingOpenState = false;
            }

            // if (definition is ColumnDefinition column) {
            //     if (this.isInvisibleDragging) {
            //         this.change.X += e.HorizontalChange;
            //         if (column.ActualWidth == 0.0d && this.change.X > sizeForAutoClose) {
            //             if (this.firstClose) {
            //                 this.isInvisibleDragging = false;
            //                 this.DragIncrement = this.prevIncrement;
            //                 column.Width = new GridLength(sizeForAutoClose + 1);
            //                 ReleaseMouseCapture();
            //                 ReleaseAllTouchCaptures();
            //                 this.firstClose = false;
            //             }
            //             else {
            //                 this.firstClose = true;
            //             }
            //         }
            //     }
            //     else if (column.ActualWidth == 0.0d) {
            //         if (Math.Abs(this.change.X - sizeForAutoClose) > this.InvisibleDragSizeForActionThreshold) {
            //             column.Width = new GridLength(sizeForAutoClose + 1);
            //             OnSplitterJumped(0.0d, sizeForAutoClose + 1);
            //             ReleaseMouseCapture();
            //             ReleaseAllTouchCaptures();
            //         }
            //     }
            //     else if (column.ActualWidth < sizeForAutoClose) {
            //         this.isInvisibleDragging = true;
            //         this.isDragging = true;
            //         this.prevIncrement = this.DragIncrement;
            //         this.DragIncrement = this.InvisibleDragSizeForActionThreshold;
            //         this.change = new Point(0, 0);
            //     }
            //     else if (column.ActualWidth < Math.Abs(sizeForAutoClose - this.InvisibleDragSizeForActionThreshold)) {
            //         // if (this.isInvisibleDragging) {
            //         //     column.Width = new GridLength(0.0d);
            //         //     OnSplitterJumped(0.0d, minSize + 1);
            //         //     this.isInvisibleDragging = false;
            //         // }
            //     }
            // }
        }

        private bool skip = false;
        private bool resetDragIncrement = false;

        private void OnSplitterJumped(ColumnDefinition column) {

        }

        private void OnSplitterMoved(DragDeltaEventArgs e, bool isHorizontal) {

        }
    }
}