using Windows.UI;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ColorPickerUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ColorEditDialog : Page
    {
        public ColorEditDialog()
        {
            this.InitializeComponent();
        }

        public string ColorName
        {
            get => this.colorName.Text;
            set => this.colorName.Text = value;
        }

        public Color Color
        {
            get => this.colorPicker.Color;
            set => this.colorPicker.Color = value;
        }
    }
}
