using System.Numerics;
using Windows.UI;

namespace ColorPickerShared;

// http://stackoverflow.com/a/19338652/62857
public static partial class ColorHelper
{
    /// <summary>
    /// Converts an HSL color value to RGB.
    /// </summary>
    /// <param name="hsl">Normalized HSL vector</param>
    public static Color FromHSL(Vector4 hsl)
    {
        float r, g, b;

        if (hsl.Y == 0.0f)
        {
            r = g = b = hsl.Z;
        }
        else
        {
            var q = hsl.Z < 0.5f ? hsl.Z * (1.0f + hsl.Y) : hsl.Z + hsl.Y - hsl.Z * hsl.Y;
            var p = 2.0f * hsl.Z - q;
            r = hueToRgb(p, q, hsl.X + 1.0f / 3.0f);
            g = hueToRgb(p, q, hsl.X);
            b = hueToRgb(p, q, hsl.X - 1.0f / 3.0f);
        }

        return Color.FromArgb((byte)(hsl.W * 255), (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        
        float hueToRgb(float p, float q, float t)
        {
            if (t < 0.0f) t += 1.0f;
            if (t > 1.0f) t -= 1.0f;
            if (t < 1.0f / 6.0f) return p + (q - p) * 6.0f * t;
            if (t < 1.0f / 2.0f) return q;
            if (t < 2.0f / 3.0f) return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
            return p;
        }
    }

    /// <summary>
    /// Converts an RGB color value to a normalized HSL vector.
    /// </summary>
    /// <returns>
    /// Normalized vectors with values (hue, saturation, lightness, alpha)
    /// </returns>
    public static Vector4 ToHSL(Color rgba)
    {
        ToHSL(rgba, out float h, out float s, out float l);
        return new Vector4(h, s, l, rgba.A / 255.0f);
    }

    /// <summary>
    /// Converts an RGB color value to normalized HSL values.
    /// </summary>
    public static void ToHSL(Color rgba, out float h, out float s, out float l)
    {
        float r = rgba.R / 255.0f;
        float g = rgba.G / 255.0f;
        float b = rgba.B / 255.0f;

        float max = (r > g && r > b) ? r : (g > b) ? g : b;
        float min = (r < g && r < b) ? r : (g < b) ? g : b;

        l = (max + min) / 2.0f;

        if (l == 0)
        {
            h = s = 0.0f;
        }
        else
        {
            float delta = max - min;
            s = (l > 0.5f) ? delta / (2.0f - max - min) : delta / (max + min);

            if (r > g && r > b)
            {
                h = (g - b) / delta;
            }
            else if (g > b)
            {
                h = (b - r) / delta + 2.0f;
            }
            else
            {
                h = (r - g) / delta + 4.0f;
            }

            h /= 6.0f;
        }

        if (h < 0)
        {
            h += 1f;
        }
    }
}
