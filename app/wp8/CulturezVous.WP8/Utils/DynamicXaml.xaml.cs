using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CulturezVous.WP8.Utils
{
    public partial class DynamicXaml : UserControl
    {
        public DynamicXaml()
        {
            InitializeComponent();
        }

        public string Xaml
        {
            get { return GetXaml(this); }
            set { SetXaml(this, value); }
        }

        public static DependencyProperty XamlProperty =
            DependencyProperty.Register("Xaml",
                                        typeof(string),
                                        typeof(DynamicXaml),
                                        new PropertyMetadata(null, _SetXaml));

        private static void _SetXaml(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DynamicXaml)d).LayoutRoot.Children.Clear();
            var uiElement = XamlReader.Load(e.NewValue.ToString()) as UIElement;
            if (uiElement == null) return;
            uiElement.SetValue(NameProperty, Guid.NewGuid().ToString().Replace("-", string.Empty));
            ((DynamicXaml)d).LayoutRoot.Children.Add(uiElement);
        }

        public static string GetXaml(DependencyObject obj)
        {
            return obj.GetValue(XamlProperty).ToString();
        }

        public static void SetXaml(DependencyObject obj, string value)
        {
            obj.SetValue(XamlProperty, value);
        }
    }
}
