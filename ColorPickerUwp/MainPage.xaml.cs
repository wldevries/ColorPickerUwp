using ColorPickerUwp.ViewModels;
using ColorPickerUwp.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        }

        private void AddGroup(object sender, RoutedEventArgs e)
        {
            this.colorGroupPanel.Children.Add(new ColorGroupView());
        }

        private void AddColors(object sender, RoutedEventArgs e)
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
                var colorGroup = new ColorGroupView();
                    colorGroup.DataContext = new ColorGroupViewModel()
                    {
                        Name = "Custom",
                        Colors = new ObservableCollection<ColorViewModel>(colors),
                    };
                this.colorGroupPanel.Children.Add(colorGroup);
            }
        }
    }
}
