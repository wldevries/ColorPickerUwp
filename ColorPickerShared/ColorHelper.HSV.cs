using System;
using System.Numerics;
using Windows.UI;

namespace ColorPickerShared;

// from http://stackoverflow.com/a/1626175/62857
public static partial class ColorHelper
{
    public static Vector4 ToHSV(Color color)
    {
        ToHSV(color, out float hue, out float saturation, out float value);
        return new Vector4(hue, saturation, value, color.A);
    }

    public static void ToHSV(Color rgba, out float hue, out float saturation, out float value)
    {
        int max = Math.Max(rgba.R, Math.Max(rgba.G, rgba.B));
        int min = Math.Min(rgba.R, Math.Min(rgba.G, rgba.B));

        var hsl = ToHSL(rgba);
        hue = hsl.X;

        saturation = (max == 0) ? 0 : 1f - (1f * min / max);
        value = max / 255f;
    }

    /// <summary>
    /// Create a color from a normalized HSV vector.
    /// </summary>
    public static Color FromHSV(Vector4 hsv)
    {
        var c = FromHSV(hsv.X, hsv.Y, hsv.Z);
        c.A = (byte)(hsv.W * 255);
        return c;
    }

    /// <summary>
    /// Create a color from normalized HSV values.
    /// </summary>
    public static Color FromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value *= 255;
        byte v = (byte)(value);
        byte p = (byte)(value * (1 - saturation));
        byte q = (byte)(value * (1 - f * saturation));
        byte t = (byte)(value * (1 - (1 - f) * saturation));

        if (hi == 0)
            return Color.FromArgb(255, v, t, p);
        else if (hi == 1)
            return Color.FromArgb(255, q, v, p);
        else if (hi == 2)
            return Color.FromArgb(255, p, v, t);
        else if (hi == 3)
            return Color.FromArgb(255, p, q, v);
        else if (hi == 4)
            return Color.FromArgb(255, t, p, v);
        else
            return Color.FromArgb(255, v, p, q);
    }
}
