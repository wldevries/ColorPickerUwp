using System;
using System.Collections.Generic;
using System.Text;

namespace ColorPickerShared.DTO
{
    public class ColorDTO
    {
        public string Color { get; set; }
        public string Name { get; set; }
    }

    public class ColorGroupDTO
    {
        public string Name { get; set; }
        public List<ColorDTO> Colors { get; set; }
    }
}
