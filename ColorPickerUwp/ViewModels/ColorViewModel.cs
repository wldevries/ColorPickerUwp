﻿using System;
using System.Linq;
using Windows.UI;
using static ColorPickerShared.ColorHelper;

namespace ColorPickerUwp.ViewModels
{
    public class ColorViewModel
    {
        public string Name { get; set; }
        public string Line { get; set; }
        public Color Color { get; set; }

        public static ColorViewModel ParseLine(string line)
        {
            var words = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            // try to parse first word as a color hex value
            if (words.Length > 0)
            {
                if (ParseHex(words[0]) is Color color)
                {
                    return new ColorViewModel
                    {
                        Color = color,
                        Name = determineName(),
                        Line = line,
                    };
                }
                else if (FromName(words[0]) is Color c2)
                {
                    return new ColorViewModel
                    {
                        Color = c2,
                        Name = determineName(),
                        Line = line,
                    };
                }
            }
            return null;

            string determineName()
            {
                var name = string.Join(' ', words.Skip(1)).Trim();
                return (name == "" || name == "-") ? words.FirstOrDefault() ?? string.Empty : name;
            }
        }
    }
}