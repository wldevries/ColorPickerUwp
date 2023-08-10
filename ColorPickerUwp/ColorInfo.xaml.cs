using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorInfo), new PropertyMetadata(new Color()));

        private void ColorInfo_Loaded(object sender, RoutedEventArgs e)
        {
            List<ColorViewModel> list = new List<ColorViewModel>();
            var props = typeof(Colors)
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
            SetColors(list);
        }

        private void SelectColor(object sender, ItemClickEventArgs e)
        {
            this.Color = (e.ClickedItem as ColorViewModel).Color;
        }

        public void SetColors(IEnumerable<ColorViewModel> colors)
        {
            var oc = new ObservableCollection<ColorViewModel>(colors);
            this.Colors.ItemsSource = oc;
        }
    }
}
