using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Windows.UI;

namespace ColorPickerShared;

public static partial class ColorHelper
{
    public static Color? FromName(string name)
    {
        var color_props = typeof(Colors).GetProperties();
        foreach (var c in color_props)
        {
            if (name.Equals(c.Name, StringComparison.OrdinalIgnoreCase))
            {
                return (Color)c.GetValue(null, null);
            }
        }

        return null;
    }

    public static IEnumerable<(Color color, string name)> GetColors()
    {
        var props = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public);
        foreach (var prop in props)
        {
            var v = prop.GetValue(null, null);
            if (v is Color c)
            {
                yield return (c, prop.Name);
            }
        }
    }

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

    public static string ToHexWeb(this Color c)
    {
        return c.A == 255
            ? $"#{c.R:X2}{c.G:X2}{c.B:X2}"
            : $"#{c.R:X2}{c.G:X2}{c.B:X2}{c.A:X2}";
    }
}
