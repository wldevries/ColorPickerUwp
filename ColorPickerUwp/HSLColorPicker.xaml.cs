using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using static ColorPickerShared.ColorHelper;

namespace ColorPickerUwp
{
    public sealed partial class HSLColorPicker : UserControl
    {
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(double), typeof(HSLColorPicker), new PropertyMetadata(0.0));

        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register("Saturation", typeof(double), typeof(HSLColorPicker), new PropertyMetadata(0.5));

        public static readonly DependencyProperty LightnessProperty =
            DependencyProperty.Register("Lightness", typeof(double), typeof(HSLColorPicker), new PropertyMetadata(0.5));

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(HSLColorPicker), new PropertyMetadata(new Color()));

        private readonly LinearGradientBrush HueGradient;

        private readonly LinearGradientBrush SaturationGradient;
        private readonly GradientStop SaturationStart;
        private readonly GradientStop SaturationEnd;

        private readonly LinearGradientBrush LightnessGradient;
        private readonly GradientStop LightnessStart;
        private readonly GradientStop LightnessEnd;

        public HSLColorPicker()
        {
            this.InitializeComponent();

            this.HueGradient = new LinearGradientBrush();
            HueGradient.StartPoint = new Point(0, 0);
            HueGradient.EndPoint = new Point(0, 1);
            for (int a = 0; a <= 360; a+= 1)
            {
                float weight = a / 360f;
                var c = FromHSL(new Vector4(weight, 1, 0.5f, 1));
                var stop = new GradientStop() { Color = c, Offset = weight };
                HueGradient.GradientStops.Add(stop);
            }

            this.SaturationGradient = new LinearGradientBrush();
            SaturationGradient.StartPoint = new Point(0, 1);
            SaturationGradient.EndPoint = new Point(0, 0);
            SaturationStart = new GradientStop();
            SaturationEnd = new GradientStop() { Offset = 1 };
            SaturationGradient.GradientStops = new GradientStopCollection()
            {
                SaturationStart, SaturationEnd,
            };

            this.LightnessGradient = new LinearGradientBrush();
            LightnessGradient.StartPoint = new Point(0, 1);
            LightnessGradient.EndPoint = new Point(0, 0);
            LightnessStart = new GradientStop();
            LightnessEnd = new GradientStop() { Offset = 1 };
            LightnessGradient.GradientStops = new GradientStopCollection()
            {
                LightnessStart, LightnessEnd,
            };

            this.UpdateColor();

            this.Loaded += HSLColorPicker_Loaded;
        }

        private void HSLColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            this.HueBand.Fill = HueGradient;
            this.SaturationBand.Fill = this.SaturationGradient;
            this.LightnessBand.Fill = this.LightnessGradient;

        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        public double Lightness
        {
            get { return (double)GetValue(LightnessProperty); }
            set { SetValue(LightnessProperty, value); }
        }

        private void HueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            UpdateColor();
        }

        private void LightnessChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            UpdateColor();
        }

        private void SaturationChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            UpdateColor();
        }

        private void UpdateColor()
        {
            UpdateSaturation();
            UpdateLightness();
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            var previewHSLA = new Vector4((float)(this.Hue / 360), (float)this.Saturation, (float)this.Lightness, 1);
            this.Color = FromHSL(previewHSLA);
        }

        private void UpdateSaturation()
        {
            if (this.SaturationGradient == null) return;
            var hsla = new Vector4((float)(this.Hue / 360), 0, (float)this.Lightness, 1);
            this.SaturationGradient.GradientStops[0].Color = FromHSL(hsla);
            hsla.Y = 1;
            this.SaturationGradient.GradientStops[1].Color = FromHSL(hsla);
        }

        private void UpdateLightness()
        {
            if (this.LightnessGradient == null) return;
            var hsla = new Vector4((float)(this.Hue / 360), (float)this.Saturation, 0, 1);
            this.LightnessGradient.GradientStops[0].Color = FromHSL(hsla);
            hsla.Z = 1;
            this.LightnessGradient.GradientStops[1].Color = FromHSL(hsla);
        }
    }
}
