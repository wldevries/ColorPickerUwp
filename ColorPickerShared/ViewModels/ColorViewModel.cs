using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using Windows.UI;
using static ColorPickerShared.ColorHelper;

namespace ColorPickerShared.ViewModels;

public partial class ColorViewModel : ObservableObject
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string line;

    [ObservableProperty]
    private Color color;

    public static ColorViewModel FromLine(string line)
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
            var name = string.Join(" ", words.Skip(1)).Trim();
            return (name == "" || name == "-") ? words.FirstOrDefault() ?? string.Empty : name;
        }
    }
}
