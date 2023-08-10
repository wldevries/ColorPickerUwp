using ColorPicker.Shared;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ColorPickerUwp
{
    public sealed partial class ColorCard : UserControl
    {
        public ColorCard()
        {
            this.InitializeComponent();
            this.UpdateUI();
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorCard), new PropertyMetadata(new Color(), UpdateUI));

        public string ColorName
        {
            get { return (string)GetValue(ColorNameProperty); }
            set { SetValue(ColorNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorNameProperty =
            DependencyProperty.Register("ColorName", typeof(string), typeof(ColorCard), new PropertyMetadata("", UpdateUI));

        public bool ShowHex
        {
            get { return (bool)GetValue(ShowHexProperty); }
            set { SetValue(ShowHexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowHex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowHexProperty =
            DependencyProperty.Register("ShowHex", typeof(bool), typeof(ColorCard), new PropertyMetadata(false, UpdateUI));

        private static void UpdateUI(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorCard c)
            {
                c.UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (this.border != null && this.nameTextBlock != null)
            {
                if (this.Color is Color color)
                {
                    this.border.Background = new SolidColorBrush(color);
                    var hsl = ColorPicker.Shared.ColorHelper.ToHSL(color);
                    if (hsl.Z > 0.5)
                    {
                        this.nameTextBlock.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    else
                    {
                        this.nameTextBlock.Foreground = new SolidColorBrush(Colors.White);
                    }
                }
                else
                {
                    this.border.Background = null;
                }

                if (this.ShowHex)
                {
                    if (this.Color is Color c)
                    {
                        this.nameTextBlock.Text = c.ToHex();
                    }
                    else
                    {
                        this.nameTextBlock.Text = null;
                    }
                }
                else
                {
                    if (this.ColorName is string name)
                    {
                        this.nameTextBlock.Text = name;
                    }
                    else
                    {
                        this.nameTextBlock.Text = null;
                    }
                }
            }
        }
    }
}
