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
    }
}
