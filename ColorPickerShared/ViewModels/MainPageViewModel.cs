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
        cgvm.MoveUpAction = () => this.MoveUp(cgvm);
        cgvm.MoveDownAction = () => this.MoveDown(cgvm);
        cgvm.RemoveGroupAction = () => this.RemoveGroup(cgvm);
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

    public void RemoveGroup(ColorGroupViewModel vm)
    {
        this.Groups.Remove(vm);
    }

    public void MoveUp(ColorGroupViewModel vm)
    {
        var index = this.Groups.IndexOf(vm);
        if (index > 0)
        {
            this.Groups.Move(index, index - 1); ;
        }
    }

    public void MoveDown(ColorGroupViewModel vm)
    {
        var index = this.Groups.IndexOf(vm);
        if (index < this.Groups.Count - 1)
        {
            this.Groups.Move(index, index + 1); ;
        }
    }
}
