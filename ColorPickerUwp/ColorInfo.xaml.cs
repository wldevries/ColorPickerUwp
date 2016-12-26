using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ColorPickerUwp
{
    public sealed partial class ColorInfo : UserControl
    {
        public ColorInfo()
        {
            this.InitializeComponent();
            this.Loaded += ColorInfo_Loaded;
        }

        private void ColorInfo_Loaded(object sender, RoutedEventArgs e)
        {
            List<ColorViewModel> list = new List<ColorViewModel>();
            var props = typeof(Windows.UI.Colors)
                .GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach(var prop in props)
            {
                var v = prop.GetValue(null, null);
                if (v is Color)
                {
                    var c = (Color)v;
                    var vm = new ColorViewModel()
                    {
                        Color = c,
                        Name = Humanizer.StringHumanizeExtensions.Humanize(
                            prop.Name, Humanizer.LetterCasing.LowerCase),
                    };
                    list.Add(vm);
                }
            }
            list = list.OrderBy(c => ColorPicker.Shared.ColorHelper.ToHSL(c.Color).X).ToList();
            this.Colors.ItemsSource = list;
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorInfo), new PropertyMetadata(new Color(), ColorChanged));

        private static void ColorChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            var i = dobj as ColorInfo;
            if (i != null)
            {
                if (e.NewValue is Color)
                {
                    var c = (Color)e.NewValue;
                    if (c == null)
                    {
                        i.Info.Text = "";
                    }
                    else
                    {
                        i.Info.Text = $"#{c.A:X}{c.R:X}{c.G:X}{c.B:X}";
                    }
                }
            }
        }

        private void SelectColor(object sender, ItemClickEventArgs e)
        {
            this.Color = (e.ClickedItem as ColorViewModel).Color;
        }
    }

    public class ColorViewModel
    {
        public string Name { get; set; }
        public Color Color { get; set; }
    }
}
