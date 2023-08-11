using Humanizer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Windows.UI;

namespace ColorPickerUwp.ViewModels
{
    public class ColorGroupViewModel
    {
        public ColorGroupViewModel()
        {
            Colors = new ObservableCollection<ColorViewModel>();                
        }

        public string Name { get; set; }
        public ObservableCollection<ColorViewModel> Colors { get; set; }

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
