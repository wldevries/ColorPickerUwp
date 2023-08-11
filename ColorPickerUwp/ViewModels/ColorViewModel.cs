using System;
using System.Linq;
using Windows.UI;

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
                if (ColorPicker.Shared.ColorHelper.ParseHex(words[0]) is Color color)
                {
                    var name = string.Join(' ', words.Skip(1));
                    return new ColorViewModel
                    {
                        Color = color,
                        Name = name,
                        Line = line,
                    };
                }
            }
            return null;
        }
    }
}
