using ColorPickerShared.Services;
using ColorPickerShared.ViewModels;
using ColorPickerUwp.Views;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var vms = await SessionSaver.LoadAsync();
            foreach (var vm in vms)
            {
                this.AddGroup(vm);
            }
        }

        private void AddGroup(object sender, RoutedEventArgs e)
        {
            AddGroup(new ColorGroupViewModel() { Name = "new" });
        }

        private void AddColors(object sender, RoutedEventArgs e)
        {
            var cgvm = ColorGroupViewModel.FromText(this.inputText.Text);

            if (cgvm != null)
            {
                AddGroup(cgvm);
            }
        }

        private void AddSystem(object sender, RoutedEventArgs e)
        {
            AddGroup(ColorGroupViewModel.CreateSystem());
        }

        private void AddGroup(ColorGroupViewModel cgvm)
        {
            var colorGroup = new ColorGroupView
            {
                DataContext = cgvm
            };
            this.colorGroupPanel.Children.Add(colorGroup);
        }

        internal async Task Save()
        {
            var vms = this.colorGroupPanel.Children
                .OfType<FrameworkElement>()
                .Select(cgv => cgv.DataContext as ColorGroupViewModel)
                .ToList();
            await SessionSaver.SaveAsync(vms);
        }
    }
}
