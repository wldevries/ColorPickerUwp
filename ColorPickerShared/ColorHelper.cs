﻿using System;
using System.Globalization;
using System.Numerics;
using Windows.UI;

namespace ColorPicker.Shared
{
    public static class ColorHelper
    {
        /// <summary>
        /// Try to parse a string as a color hex value
        /// </summary>
        public static Color? ParseHex(string input)
        {
            input = input.TrimStart('#');
            string hex = null;
            switch (input.Length)
            {
                case 3:
                    hex = $"{input[0]}{input[0]}{input[1]}{input[1]}{input[2]}{input[2]}";
                    break;

                case 6:
                    hex = input;
                    break;

                case 8:
                    hex = input.Substring(2);
                    break;
            };
            byte a = 255;
            if (hex == null ||
                !byte.TryParse(hex.Substring(0, 2), NumberStyles.HexNumber, null, out byte r) ||
                !byte.TryParse(hex.Substring(2, 2), NumberStyles.HexNumber, null, out byte g) ||
                !byte.TryParse(hex.Substring(4, 2), NumberStyles.HexNumber, null, out byte b) ||
                (hex.Length == 8 && !byte.TryParse(hex.Substring(6, 2), NumberStyles.HexNumber, null, out a)))
            {
                return null;
            }
            return Color.FromArgb(a, r, g, b);
        }

        public static string ToHex(this Color c)
        {
            return c.A == 255
                ? $"#{c.R:X2}{c.G:X2}{c.B:X2}"
                : $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        #region HSL

        // http://stackoverflow.com/a/19338652/62857

        /// <summary>
        /// Converts an HSL color value to RGB.
        /// Input: Vector4 ( X: [0.0, 1.0], Y: [0.0, 1.0], Z: [0.0, 1.0], W: [0.0, 1.0] )
        /// Output: Color ( R: [0, 255], G: [0, 255], B: [0, 255], A: [0, 255] )
        /// </summary>
        /// <param name="hsl">Vector4 defining X = h, Y = s, Z = l, W = a. Ranges [0, 1.0]</param>
        /// <returns>RGBA Color. Ranges [0, 255]</returns>
        public static Color FromHSL(Vector4 hsl)
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
        public static Vector4 ToHSL(Color rgba)
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
                float delta = max - min;
                s = (l > 0.5f) ? delta / (2.0f - max - min) : delta / (max + min);

                if (r > g && r > b)
                    h = (g - b) / delta;

                else if (g > b)
                    h = (b - r) / delta + 2.0f;

                else
                    h = (r - g) / delta + 4.0f;

                h /= 6.0f;
            }
            if (h < 0) h += 1f;

            return new Vector4(h, s, l, rgba.A / 255.0f);
        }

        #endregion

        #region HSV

        // from http://stackoverflow.com/a/1626175/62857

        public static Vector4 ToHSV(Color color)
        {
            double hue;
            double saturation;
            double value;
            ToHSV(color, out hue, out saturation, out value);
            return new Vector4((byte)(hue * 255), (byte)(saturation * 255), (byte)(value * 255), color.A / 255.0f);
        }

        public static void ToHSV(Color rgba, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(rgba.R, Math.Max(rgba.G, rgba.B));
            int min = Math.Min(rgba.R, Math.Min(rgba.G, rgba.B));

            //hue = color.GetHue();
            var hsl = ToHSL(rgba);
            hue = hsl.X;

            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color FromHSV(Vector4 hsv)
        {
            var c = FromHSV(hsv.X, hsv.Y, hsv.Z);
            c.A = (byte)(hsv.W * 255);
            return c;
        }

        public static Color FromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
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
        
        #endregion
    }
}
