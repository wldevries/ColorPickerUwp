using CommunityToolkit.Mvvm.ComponentModel;
using Windows.UI;

namespace ColorPickerShared.ViewModels;

public partial class ColorEditViewModel : ObservableObject
{
    [ObservableProperty]
    private string colorName;

    [ObservableProperty]
    private Color color;
}
