using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColorPicker.Shared;
using static ColorPicker.Shared.ColorHelper;

namespace ColorPickerUwp
{
    public sealed partial class ColorMap : UserControl
    {
        private Point lastPoint;
        private WriteableBitmap bmp3;

        private readonly LinearGradientBrush LightnessGradient;
        private readonly GradientStop LightnessStart;
        private readonly GradientStop LightnessMid;
        private readonly GradientStop LightnessEnd;

        public ColorMap()
        {
            this.InitializeComponent();

            this.Loaded += MeshCanvas_Loaded;

            this.image3.MouseMove += Image3_PointerMoved;
            this.image3.MouseLeftButtonDown += Image3_PointerPressed;
            this.image3.MouseLeftButtonUp += Image3_PointerReleased;

            this.LightnessGradient = new LinearGradientBrush();
            LightnessGradient.StartPoint = new Point(0, 0);
            LightnessGradient.EndPoint = new Point(0, 1);
            LightnessStart = new GradientStop();
            LightnessMid = new GradientStop() { Offset = 0.5 };
            LightnessEnd = new GradientStop() { Offset = 1 };
            LightnessGradient.GradientStops = new GradientStopCollection()
            {
                LightnessStart, LightnessMid, LightnessEnd,
            };
            this.LightnessBackground.Fill = this.LightnessGradient;
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorMap), new PropertyMetadata(new Color()));
        private bool captured;

        private void Image3_PointerPressed(object sender, MouseEventArgs e)
        {
            this.captured = true;
            image3.CaptureMouse();
        }

        private void Image3_PointerReleased(object sender, MouseEventArgs e)
        {
            this.captured = false;
            image3.ReleaseMouseCapture();
        }

        private void Image3_PointerMoved(object sender, MouseEventArgs e)
        {
            if (this.captured)
            {
                this.lastPoint = e.GetPosition(this.image3);
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            if (lastPoint == null || image3 == null) return;
            var x = lastPoint.X / image3.ActualWidth;
            var y = 1 - lastPoint.Y / image3.ActualHeight;
            var selectedColor = HueWheel.CalcWheelColor((float)x, 1 - (float)y, (float)this.LightnessSlider.Value);

            if (selectedColor.A > 0)
            {
                this.Color = selectedColor;
                this.LightnessStart.Color = Colors.White;
                this.LightnessMid.Color = HueWheel.CalcWheelColor((float)x, 1 - (float)y, 0.5f);
                this.LightnessEnd.Color = Colors.Black;
            }
        }

        private void MeshCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            bmp3 = new WriteableBitmap(1000, 1000);
            HueWheel.CreateHueCircle(bmp3, 0.5f);
            this.image3.Source = bmp3;
        }
       
        private void lightnessChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //this.CreateHueCircle((float)e.NewValue);
            this.UpdateColor();
        }
    }
}
