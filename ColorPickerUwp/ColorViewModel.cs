using System.Collections.Generic;
using Windows.UI;

namespace ColorPickerUwp
{
    public class ColorViewModel
    {
        public string Name { get; set; }
        public string Line { get; set; }
        public Color Color { get; set; }
    }
    
    public class ColorGroup
    {
        public string Name { get; set; }
        public List<ColorViewModel> Colors { get; set; }
    }
}
