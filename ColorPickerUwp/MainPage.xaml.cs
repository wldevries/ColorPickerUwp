using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI;
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

                var words = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                // try to parse first word as a color hex value
                if (words.Length > 0)
                {
                    var color = ColorPicker.Shared.ColorHelper.ParseHex(words[0]);
                    if (color != null && !colors.Any(c => c.Color == color))
                    {
                        var name = string.Join(' ', words.Skip(1));
                        colors.Add(new ColorViewModel { Color = color.Value, Name = name });
                    }
                }
            }
            while (line != null);

            this.info.SetColors(colors);
        }
    }
}
