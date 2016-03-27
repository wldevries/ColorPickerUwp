using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static ColorPickerUwp.ColorHelper;

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
            var selectedColor = CalcWheelColor((float)x, 1 - (float)y, (float)this.LightnessSlider.Value);

            if (selectedColor.A > 0)
            {
                this.Color = selectedColor;
                this.LightnessStart.Color = Colors.White;
                this.LightnessMid.Color = CalcWheelColor((float)x, 1 - (float)y, 0.5f);
                this.LightnessEnd.Color = Colors.Black;
            }
        }

        private void MeshCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            bmp3 = new WriteableBitmap(1000, 1000);
            CreateHueCircle(0.5f);
            this.image3.Source = bmp3;
        }

        private void CreateHueCircle(float lightness)
        {
            FillBitmap(bmp3, (x, y) =>
            {
                return CalcWheelColor(x, y, lightness);
            });
        }

        public static Color CalcWheelColor(float x, float y, float lightness)
        {
            x = x - 0.5f;
            y = (1 - y) - 0.5f;
            float saturation = 2 * (float)Math.Sqrt(x * x + y * y);
            float hue = y < 0 ?
                (float)Math.Atan2(-y, -x) + (float)Math.PI :
                (float)Math.Atan2(y, x);
            if (saturation > 1)
                return new Color();
            else
                return FromHSL(new Vector4(hue / ((float)Math.PI * 2), saturation, lightness, 1));
            //  return FromHSV(new Vector4(hue / (float)Math.PI * 180, saturation, lightness, 1));
        }

        private void lightnessChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //this.CreateHueCircle((float)e.NewValue);
            this.UpdateColor();
        }

        private static void FillBitmap(WriteableBitmap bmp, Func<float, float, Color> fillPixel)
        {
            int width = bmp.PixelWidth;
            int height = bmp.PixelHeight;
            //await Task.Run(() =>
            {
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < height; x++)
                    {
                        Color color = fillPixel((float)x / width, (float)y / height);
                        bmp.Pixels[x + y * width] = WriteableBitmapExtensions.ConvertColor(color);
                    }
                }
            }
            //);
            bmp.Invalidate();
        }
    }
}
