using ColorPickerShared.ViewModels;
using ColorPickerUwp.Views;
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
            var cgvm = ColorGroupViewModel.FromText(this.inputText.Text);

            if (cgvm != null)
            {
                var colorGroup = new ColorGroupView
                {
                    DataContext = cgvm
                };
                this.colorGroupPanel.Children.Add(colorGroup);
            }
        }
    }
}
