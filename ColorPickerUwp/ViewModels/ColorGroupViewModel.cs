using ColorPicker.Shared;
using Humanizer;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reflection;
using Windows.UI;

namespace ColorPickerUwp.ViewModels
{
    public class ColorGroupViewModel : ReactiveObject
    {
        public ColorGroupViewModel()
        {
            Colors = new ObservableCollection<ColorViewModel>();

            this.SortCommand = ReactiveCommand.Create(() =>
            {
                var sortedColors = this.Colors
                    .GroupBy(vm => vm.Color)
                    .OrderBy(g => g.Key.A)
                    .ThenBy(g => g.Key.ToHex())
                    .Select(c => (color: c, hsl: ColorPicker.Shared.ColorHelper.ToHSL(c.Key)))
                    .OrderBy(c => c.hsl.X)
                    .ThenBy(c => c.hsl.Z)
                    .ThenBy(c => c.hsl.Y)
                    .Select(c => c.color)
                    .Select(g => g.FirstOrDefault(v => v.Name != "-" && v.Name != "") ?? g.First())
                    .ToList();

                this.Colors = new ObservableCollection<ColorViewModel>(sortedColors);
                this.RaisePropertyChanged(nameof(Colors));
            });
        }

        public string Name { get; set; }
        public ObservableCollection<ColorViewModel> Colors { get; set; }

        public ReactiveCommand<Unit, Unit> SortCommand { get; }

        public static ColorGroupViewModel CreateSystem()
        {
            List<ColorViewModel> list = new List<ColorViewModel>();
            var props = typeof(Colors)
                .GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (var prop in props)
            {
                var v = prop.GetValue(null, null);
                if (v is Color)
                {
                    var c = (Color)v;
                    var vm = new ColorViewModel()
                    {
                        Color = c,
                        Name = prop.Name.Humanize(LetterCasing.LowerCase),
                    };
                    list.Add(vm);
                }
            }
            return new ColorGroupViewModel()
            {
                Name = "System",
                Colors = new ObservableCollection<ColorViewModel>(list),
            };
        }
    }
}
