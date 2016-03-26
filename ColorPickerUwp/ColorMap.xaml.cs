using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using static ColorPickerUwp.ColorHelper;

namespace ColorPickerUwp
{
    public sealed partial class ColorMap : UserControl
    {
        private PointerPoint lastPoint;
        private WriteableBitmap bmp1;
        private WriteableBitmap bmp2;
        private WriteableBitmap bmp3;

        private readonly LinearGradientBrush LightnessGradient;
        private readonly GradientStop LightnessStart;
        private readonly GradientStop LightnessMid;
        private readonly GradientStop LightnessEnd;

        public ColorMap()
        {
            this.InitializeComponent();

            this.Loaded += MeshCanvas_Loaded;

            this.image3.PointerMoved += Image3_PointerMoved;
            this.image3.PointerPressed += Image3_PointerPressed;
            this.image3.PointerReleased += Image3_PointerReleased;

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

        private void Image3_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            image3.CapturePointer(e.Pointer);
            e.Handled = true;
        }

        private void Image3_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            image3.ReleasePointerCapture(e.Pointer);
            e.Handled = true;
        }

        private void Image3_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (image3.PointerCaptures?.Any(p => p.PointerId == e.Pointer.PointerId) == true)
            {
                this.lastPoint = e.GetCurrentPoint(this.image3);
                UpdateColor();
                e.Handled = true;
            }
        }

        private void UpdateColor()
        {
            if (lastPoint == null) return;
            var x = lastPoint.Position.X / image3.ActualWidth;
            var y = 1 - lastPoint.Position.Y / image3.ActualHeight;
            var selectedColor = CalcWheelColor((float)x, 1 - (float)y, (float)this.LightnessSlider.Value);

            if (selectedColor.A > 0)
            {
                this.Color = selectedColor;
                this.LightnessStart.Color = Colors.White;
                this.LightnessMid.Color = CalcWheelColor((float)x, 1 - (float)y, 0.5f);
                this.LightnessEnd.Color = Colors.Black;
            }
        }

        private async void MeshCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            //bmp1 = new WriteableBitmap(1000, 1000);
            //await FillBitmap(bmp1, (x, y) => FromHSL(new Vector4(x, 1 - y, 0.5f, 1)));
            //this.image1.Source = bmp1;

            //bmp2 = new WriteableBitmap(1000, 1000);
            //await FillBitmap(bmp2, (x, y) => FromHSV(new Vector4(360 * x, 1 - y, 1, 1)));
            //this.image2.Source = bmp2;

            bmp3 = new WriteableBitmap(1000, 1000);
            await CreateHueCircle(0.5f);
            this.image3.Source = bmp3;
        }

        private Task CreateHueCircle(float lightness)
        {
            return FillBitmap(bmp3, (x, y) =>
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

        private void lightnessChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //this.CreateHueCircle((float)e.NewValue);
            this.UpdateColor();
        }

        private static async Task FillBitmap(WriteableBitmap bmp, Func<float, float, Color> fillPixel)
        {
            var stream = bmp.PixelBuffer.AsStream();
            int width = bmp.PixelWidth;
            int height = bmp.PixelHeight;
            await Task.Run(() =>
            {
                {
                    for (int y = 0; y < width; y++)
                    {
                        for (int x = 0; x < height; x++)
                        {
                            var color = fillPixel((float)x / width, (float)y / height);
                            WriteBGRA(stream, color);
                        }
                    }
                }
            });
            stream.Dispose();
            bmp.Invalidate();
        }

        private static void WriteBGRA(Stream stream, Color color)
        {
            stream.WriteByte(color.B);
            stream.WriteByte(color.G);
            stream.WriteByte(color.R);
            stream.WriteByte(color.A);
        }
    }
}
