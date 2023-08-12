using ColorPickerShared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ColorPickerShared.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    public MainPageViewModel()
    {
        this.Groups = new ObservableCollection<ColorGroupViewModel>();
    }

    public ObservableCollection<ColorGroupViewModel> Groups { get; }

    [RelayCommand]
    public void AddGroup()
    {
        this.AddGroup(new ColorGroupViewModel() { Name = "new" });
    }

    [RelayCommand]
    public void AddSystem()
    {
        this.AddGroup(ColorGroupViewModel.CreateSystem());
    }

    public void AddGroup(ColorGroupViewModel cgvm)
    {
        this.Groups.Add(cgvm);
    }

    public async Task RestoreAsync()
    {
        var vms = await SessionSaver.LoadAsync();
        foreach (var vm in vms)
        {
            this.AddGroup(vm);
        }
    }

    public async Task SuspendAsync()
    {
        await SessionSaver.SaveAsync(this.Groups);
    }
}
