using ColorPicker.Shared;
using ColorPickerUwp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ColorPickerUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.inputText.TextChanged += InputText_TextChanged;

            this.info.DataContext = ColorGroupViewModel.CreateSystem();
        }

        private void InputText_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<ColorViewModel> colors = new List<ColorViewModel>();
            StringReader reader = new StringReader(this.inputText.Text);
            string line;
            do
            {
                line = reader.ReadLine();
                if (line is null)
                {
                    break;
                }

                var cvm = ColorViewModel.ParseLine(line);
                if (cvm != null && !colors.Any(c => c.Color == cvm.Color))
                {
                    colors.Add(cvm);
                }
            }
            while (line != null);

            if (colors.Any())
            {
                this.info.DataContext = new ColorGroupViewModel()
                {
                    Name = "Custom",
                    Colors = new ObservableCollection<ColorViewModel>(colors),
                };
            }
        }

        private void SortColors(object sender, RoutedEventArgs e)
        {
            var lines  = this.inputText.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            lines = lines
                .Select(ColorViewModel.ParseLine)
                .OfType<ColorViewModel>()
                .GroupBy(vm => vm.Color)
                .OrderBy(g => g.Key.A)
                .ThenBy(g => g.Key.ToHex())
                .Select(c => (color: c, hsl: ColorPicker.Shared.ColorHelper.ToHSL(c.Key)))
                .OrderBy(c => c.hsl.X)
                .ThenBy(c => c.hsl.Z)
                .ThenBy(c => c.hsl.Y)
                .Select(c => c.color)
                .Select(g => g.FirstOrDefault(v => v.Name != "-" && v.Name != "")?.Line ?? g.First().Color.ToHex())
                .ToList();

            this.inputText.Text = string.Join(Environment.NewLine, lines);
        }
    }
}
