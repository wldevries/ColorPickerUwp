using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using static ColorPickerShared.ColorHelper;

namespace ColorPickerShared;

public static class HueWheel
{
    public static Task CreateHueCircle(WriteableBitmap bmp3, float lightness)
    {
        return FillBitmap(bmp3, (x, y) =>
        {
            return CalcWheelColor(x, y, lightness);
        });
    }

    public static async Task FillBitmap(WriteableBitmap bmp, Func<float, float, Color> fillPixel)
    {
        var stream = bmp.PixelBuffer.AsStream();
        int width = bmp.PixelWidth;
        int height = bmp.PixelHeight;
        await Task.Run(() =>
        {
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    var color = fillPixel((float)x / width, (float)y / height);
                    stream.WriteBGRA(color);
                }
            }
        });
        bmp.Invalidate();
    }

    public static Color CalcWheelColor(float x, float y, float lightness)
    {
        x -= 0.5f;
        y = 1 - y - 0.5f;
        float saturation = 2 * (float)Math.Sqrt(x * x + y * y);
        float hueAngle = y < 0 ?
            (float)Math.Atan2(-y, -x) + (float)Math.PI :
            (float)Math.Atan2(y, x);
        if (saturation > 1)
        {
            saturation = 1;
        }

        var hsl = new Vector4(hueAngle / ((float)Math.PI * 2), saturation, lightness, 1);
        return FromHSL(hsl);
    }
}
