using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using static ColorPickerShared.ColorHelper;

namespace ColorPickerShared.ViewModels;

public partial class ColorGroupViewModel : ObservableObject
{
    public ColorGroupViewModel()
    {
        colors = new ObservableCollection<ColorViewModel>();
    }

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private ObservableCollection<ColorViewModel> colors;

    [RelayCommand]
    private void Sort()
    {
        var sortedColors = this.Colors
            .GroupBy(vm => vm.Color)
            .OrderBy(g => g.Key.A)
            .ThenBy(g => g.Key.ToHex())
            .Select(c => (color: c, hsl: ToHSL(c.Key)))
            .OrderBy(c => c.hsl.X)
            .ThenBy(c => c.hsl.Z)
            .ThenBy(c => c.hsl.Y)
            .Select(c => c.color)
            .Select(g => g.FirstOrDefault(v => v.Name != "-" && v.Name != "") ?? g.First())
            .ToList();

        this.Colors = new ObservableCollection<ColorViewModel>(sortedColors);
    }

    public static ColorGroupViewModel FromText(string input)
    {
        List<ColorViewModel> colors = new();
        StringReader reader = new(input);
        string line;
        do
        {
            line = reader.ReadLine();
            if (line is null)
            {
                break;
            }

            var cvm = ColorViewModel.FromLine(line);
            if (cvm != null)
            {
                var match = colors.FirstOrDefault(c =>
                    c.Color.A == cvm.Color.A &&
                    c.Color.R == cvm.Color.R &&
                    c.Color.G == cvm.Color.G &&
                    c.Color.B == cvm.Color.B);
                if (match == null)
                {
                    colors.Add(cvm);
                }
                else
                {
                    match.Name = string.IsNullOrWhiteSpace(match.Name) || match.Name.StartsWith("#")
                        ? cvm.Name : match.Name;
                }
            }
        }
        while (line != null);

        return colors.Any()
            ? new ColorGroupViewModel()
            {
                Name = "Custom",
                Colors = new ObservableCollection<ColorViewModel>(colors),
            }
            : null;
    }

    public static ColorGroupViewModel CreateSystem()
    {
        List<ColorViewModel> list = new();
        foreach (var (color, name) in GetColors())
        {
            ColorViewModel vm = new()
            {
                Color = color,
                Name = name.Humanize(LetterCasing.LowerCase),
            };
            list.Add(vm);
        }
        return new()
        {
            Name = "System",
            Colors = new ObservableCollection<ColorViewModel>(list),
        };
    }
}
