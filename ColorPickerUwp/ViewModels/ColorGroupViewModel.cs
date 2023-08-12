using ColorPickerShared;
using Humanizer;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using static ColorPickerShared.ColorHelper;

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
                    .Select(c => (color: c, hsl: ToHSL(c.Key)))
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
            foreach (var (color, name) in GetColors())
            {
                var vm = new ColorViewModel()
                {
                    Color = color,
                    Name = name.Humanize(LetterCasing.LowerCase),
                };
                list.Add(vm);
            }
            return new ColorGroupViewModel()
            {
                Name = "System",
                Colors = new ObservableCollection<ColorViewModel>(list),
            };
        }
    }
}
