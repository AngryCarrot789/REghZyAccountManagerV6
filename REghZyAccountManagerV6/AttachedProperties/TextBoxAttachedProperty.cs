using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace REghZyAccountManagerV6.AttachedProperties {
    public static class TextBoxAttachedProperty {
        public static readonly DependencyProperty PreviewTextProperty =
            DependencyProperty.RegisterAttached(
                "PreviewText",
                typeof(string),
                typeof(TextBoxAttachedProperty),
                new FrameworkPropertyMetadata("Enter text here..."));

        public static readonly DependencyProperty PreviewTextForegroundProperty =
            DependencyProperty.RegisterAttached(
                "PreviewTextForeground",
                typeof(Brush),
                typeof(TextBoxAttachedProperty),
                new FrameworkPropertyMetadata(
                    Brushes.DimGray, 
                    FrameworkPropertyMetadataOptions.AffectsRender | 
                    FrameworkPropertyMetadataOptions.Inherits | 
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public static void SetPreviewText(DependencyObject obj, string value) {
            obj.SetValue(PreviewTextProperty, value);
        }

        public static string GetPreviewText(DependencyObject obj) {
            return (string) obj.GetValue(PreviewTextProperty);
        }

        public static void SetPreviewTextForeground(DependencyObject obj, string value) {
            obj.SetValue(PreviewTextForegroundProperty, value);
        }

        public static string GetPreviewTextForeground(DependencyObject obj) {
            return (string) obj.GetValue(PreviewTextForegroundProperty);
        }
    }
}
