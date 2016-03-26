using System.Numerics;
using Windows.UI;

namespace ColorPickerUwp
{
    public static class ColorHelper
    {
        /// <summary>
        /// Converts an HSL color value to RGB.
        /// Input: Vector4 ( X: [0.0, 1.0], Y: [0.0, 1.0], Z: [0.0, 1.0], W: [0.0, 1.0] )
        /// Output: Color ( R: [0, 255], G: [0, 255], B: [0, 255], A: [0, 255] )
        /// </summary>
        /// <param name="hsl">Vector4 defining X = h, Y = s, Z = l, W = a. Ranges [0, 1.0]</param>
        /// <returns>RGBA Color. Ranges [0, 255]</returns>
        public static Color HslToRgba(Vector4 hsl)
        {
            float r, g, b;

            if (hsl.Y == 0.0f)
                r = g = b = hsl.Z;

            else
            {
                var q = hsl.Z < 0.5f ? hsl.Z * (1.0f + hsl.Y) : hsl.Z + hsl.Y - hsl.Z * hsl.Y;
                var p = 2.0f * hsl.Z - q;
                r = HueToRgb(p, q, hsl.X + 1.0f / 3.0f);
                g = HueToRgb(p, q, hsl.X);
                b = HueToRgb(p, q, hsl.X - 1.0f / 3.0f);
            }

            return Color.FromArgb((byte)(hsl.W * 255), (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        // Helper for HslToRgba
        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0.0f) t += 1.0f;
            if (t > 1.0f) t -= 1.0f;
            if (t < 1.0f / 6.0f) return p + (q - p) * 6.0f * t;
            if (t < 1.0f / 2.0f) return q;
            if (t < 2.0f / 3.0f) return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
            return p;
        }

        /// <summary>
        /// Converts an RGB color value to HSL.
        /// Input: Color ( R: [0, 255], G: [0, 255], B: [0, 255], A: [0, 255] )
        /// Output: Vector4 ( X: [0.0, 1.0], Y: [0.0, 1.0], Z: [0.0, 1.0], W: [0.0, 1.0] )
        /// </summary>
        /// <param name="rgba"></param>
        /// <returns></returns>
        public static Vector4 RgbaToHsl(Color rgba)
        {
            float r = rgba.R / 255.0f;
            float g = rgba.G / 255.0f;
            float b = rgba.B / 255.0f;

            float max = (r > g && r > b) ? r : (g > b) ? g : b;
            float min = (r < g && r < b) ? r : (g < b) ? g : b;

            float h, s, l;
            h = s = l = (max + min) / 2.0f;

            if (max == min)
                h = s = 0.0f;

            else
            {
                float d = max - min;
                s = (l > 0.5f) ? d / (2.0f - max - min) : d / (max + min);

                if (r > g && r > b)
                    h = (g - b) / d + (g < b ? 6.0f : 0.0f);

                else if (g > b)
                    h = (b - r) / d + 2.0f;

                else
                    h = (r - g) / d + 4.0f;

                h /= 6.0f;
            }

            return new Vector4(h, s, l, rgba.A / 255.0f);
        }
    }
}
