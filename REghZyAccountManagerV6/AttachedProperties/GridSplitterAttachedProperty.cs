using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace REghZyAccountManagerV6.AttachedProperties {
    public static class GridSplitterAttachedProperty {
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetActiveWindow();

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT {
            public int X;
            public int Y;
        };
        public static Point GetCursorPosition() {
            POINT pos = new POINT();
            GetCursorPos(ref pos);
            return new Point(pos.X, pos.Y);
        }

        private static bool IS_MOVING_MOUSE;

        public static readonly DependencyProperty UseAutoCloseProperty =
            DependencyProperty.RegisterAttached(
                "UseAutoClose",
                typeof(bool),
                typeof(GridSplitterAttachedProperty),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        public static readonly DependencyProperty MinSizeForAutoCloseProperty =
            DependencyProperty.RegisterAttached(
                "MinSizeForAutoClose",
                typeof(double),
                typeof(GridSplitterAttachedProperty),
                new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty InvisibleDragSizeForActionThresholdProperty =
            DependencyProperty.RegisterAttached(
                "InvisibleDragSizeForActionThreshold",
                typeof(double),
                typeof(GridSplitterAttachedProperty),
                new FrameworkPropertyMetadata(50.0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ReopenCompareValueProperty =
            DependencyProperty.RegisterAttached(
                "ReopenCompareValue",
                typeof(int),
                typeof(GridSplitterAttachedProperty),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty TargetDefinitionProperty =
            DependencyProperty.RegisterAttached(
                "TargetDefinition",
                typeof(DefinitionBase),
                typeof(GridSplitterAttachedProperty),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private static readonly Dictionary<GridSplitter, SplitterInfo> DRAG_AMOUNTS = new Dictionary<GridSplitter, SplitterInfo>();
        private class SplitterInfo {
            public double value;

            public double startDragIncrement;

            public void Add(double value) {
                this.value += value;
            }

            public void Sub(double value) {
                this.value -= value;
            }
        }

        private static SplitterInfo GetDragWrapper(GridSplitter splitter) {
            SplitterInfo wrapper;
            if (!DRAG_AMOUNTS.TryGetValue(splitter, out wrapper)) {
                DRAG_AMOUNTS[splitter] = wrapper = new SplitterInfo();
            }

            return wrapper;
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is GridSplitter splitter) {
                splitter.DragDelta += SplitterOnDragDelta;
            }
        }

        private static void SplitterOnDragDelta(object sender, DragDeltaEventArgs e) {
            if (IS_MOVING_MOUSE) {
                return;
            }

            if (sender is GridSplitter splitter) {
                double size = GetMinSizeForAutoClose(splitter);
                if (double.IsNaN(size) || double.IsPositiveInfinity(size)) {
                    return;
                }

                DefinitionBase definition = GetTargetDefinition(splitter);
                if (definition == null) {
                    return;
                }

                if (definition is ColumnDefinition column) {
                    if (column.ActualWidth == 0.0d) {
                        GetDragWrapper(splitter).Add(e.HorizontalChange);
                        if (GetDragWrapper(splitter).value > GetInvisibleDragSizeForActionThreshold(splitter)) {
                            column.Width = new GridLength(size);
                            OnSplitterMoved(splitter, e, true);
                        }
                        // if (e.HorizontalChange.CompareTo(GetDragWrapper(splitter).value) == GetReopenCompareValue(splitter)) {
                        //     column.Width = new GridLength(size);
                        //     OnSplitterMoved(splitter, e, true);
                        // }
                    }
                    else if (column.ActualWidth < Math.Abs(size - GetInvisibleDragSizeForActionThreshold(splitter))) {
                        column.Width = new GridLength(0.0d);
                        OnSplitterMoved(splitter, e, true);
                    }
                }
                else if (definition is RowDefinition row) {
                    if (row.ActualHeight == 0.0d) {
                        GetDragWrapper(splitter).Add(e.VerticalChange);
                        if (GetDragWrapper(splitter).value > GetInvisibleDragSizeForActionThreshold(splitter)) {
                            row.Height = new GridLength(size);
                            OnSplitterMoved(splitter, e, false);
                        }
                        // if (e.VerticalChange.CompareTo(0) == GetReopenCompareValue(splitter)) {
                        //     row.Height = new GridLength(size);
                        //     OnSplitterMoved(splitter, e, false);
                        // }
                    }
                    else if (row.ActualHeight < Math.Abs(size - GetInvisibleDragSizeForActionThreshold(splitter))) {
                        row.Height = new GridLength(0.0d);
                        OnSplitterMoved(splitter, e, false);
                    }
                }
            }
        }

        public static void OnSplitterMoved(GridSplitter splitter, DragDeltaEventArgs e, bool isHorizontal) {
            e.Handled = true;
            // splitter.ReleaseAllTouchCaptures();
            // splitter.ReleaseMouseCapture();
            // if (TryGetAbsolutePlacement(splitter, out Point absolute)) {
            //     Point mPos = GetCursorPosition();
            //     IS_MOVING_MOUSE = true;
            //     if (isHorizontal) {
            //         absolute.Y = mPos.Y;
            //     }
            //     else {
            //         absolute.X = mPos.X;
            //     }
            //     SetCursorPos((int) absolute.X, (int) absolute.Y);
            //     IS_MOVING_MOUSE = false;
            // }
        }

        public static bool TryGetAbsolutePlacement(this FrameworkElement element, out Point point, bool relativeToScreen = false) {
            Point absolutePos = element.PointToScreen(new Point(0, 0));
            if (relativeToScreen) {
                point = absolutePos;
                return true;
            }

            IntPtr handle = GetActiveWindow();
            HwndSource hwndSource = HwndSource.FromHwnd(handle);
            if (hwndSource?.RootVisual is Window window) {
                Point windowPos = window.PointToScreen(new Point(0, 0));
                point = new Point(absolutePos.X - windowPos.X, absolutePos.Y - windowPos.Y);
                return true;
            }

            point = default;
            return false;
        }

        public static void MoveMouseTo(GridSplitter splitter) {
            if (TryGetAbsolutePlacement(splitter, out Point absolute)) {
                IS_MOVING_MOUSE = true;
                SetCursorPos((int) absolute.X, (int) absolute.Y);
                IS_MOVING_MOUSE = false;
            }
        }

        public static double GetMinSizeForAutoClose(DependencyObject o) => (double) o.GetValue(MinSizeForAutoCloseProperty);
        public static void SetMinSizeForAutoClose(DependencyObject o, double v) => o.SetValue(MinSizeForAutoCloseProperty, v);

        public static bool GetUseAutoClose(DependencyObject o) => (bool) o.GetValue(UseAutoCloseProperty);
        public static void SetUseAutoClose(DependencyObject o, bool v) => o.SetValue(UseAutoCloseProperty, v);

        public static int GetReopenCompareValue(DependencyObject o) => (int) o.GetValue(ReopenCompareValueProperty);
        public static void SetReopenCompareValue(DependencyObject o, int v) => o.SetValue(ReopenCompareValueProperty, v);

        public static double GetInvisibleDragSizeForActionThreshold(DependencyObject o) => (double) o.GetValue(InvisibleDragSizeForActionThresholdProperty);
        public static void SetInvisibleDragSizeForActionThreshold(DependencyObject o, double v) => o.SetValue(InvisibleDragSizeForActionThresholdProperty, v);

        public static DefinitionBase GetTargetDefinition(DependencyObject o) => (DefinitionBase) o.GetValue(TargetDefinitionProperty);
        public static void SetTargetDefinition(DependencyObject o, DefinitionBase v) => o.SetValue(TargetDefinitionProperty, v);
    }
}