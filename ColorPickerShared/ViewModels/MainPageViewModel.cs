using ColorPickerShared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Gaming.Input;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Controls.Primitives;

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
            // ItemsControl becomes weirdly slow when using Move
            this.Groups.Remove(vm);
            this.Groups.Insert(index - 1, vm);
        }
    }

    public void MoveDown(ColorGroupViewModel vm)
    {
        var index = this.Groups.IndexOf(vm);
        if (index < this.Groups.Count - 1)
        {
            // ItemsControl becomes weirdly slow when using Move
            this.Groups.Remove(vm);
            this.Groups.Insert(index + 1, vm);
        }
    }

    public string GetClipboardText()
    {
        StringBuilder sb = new();
        foreach (var group in this.Groups)
        {
            sb.AppendLine("// " + group.Name);
            foreach (var color in group.Colors)
            {
                sb.AppendLine($"{color.Color.ToHex()}\t{color.Name}");
            }

            sb.AppendLine();
        }
        return sb.ToString();
    }

    public string GetClipboardXaml()
    {
        StringBuilder sb = new();
        sb.AppendLine("<ResourceDictionary");
        sb.AppendLine("    xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
        sb.AppendLine("    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");
        sb.AppendLine();

        foreach (var group in this.Groups)
        {
            sb.AppendLine($"\t<!-- {group.Name} -->");
            foreach (var color in group.Colors)
            {
                var name = canonicalize(color.Name, color.Color);
                sb.AppendLine($"\t<Color x:Key=\"{name}\">{color.Color.ToHex()}</Color>");
            }

            sb.AppendLine();
        }

        sb.Append("</ResourceDictionary>");

        return sb.ToString();

        static string canonicalize(string name, Color color)
        {
            if (name.Equals(color.ToHex(), StringComparison.OrdinalIgnoreCase))
            {
                name = string.Empty;
            }

            List<char> chars = name
                .Where(XmlConvert.IsXmlChar)
                .Where(c => c != ' ' && c != '#')
                .ToList();

            if (chars.Count == 0)
            {
                return "color" + color.ToHex().Substring(1);
            }
            else
            {
                return new string(chars.ToArray());
            }
        }
    }

    public string GetClipboardCss()
    {
        StringBuilder sb = new();
        sb.AppendLine(":root {");
        foreach (var group in this.Groups)
        {
            sb.AppendLine("    /* " + group.Name + "*/");
            foreach (var color in group.Colors)
            {
                var name = canonicalize(color.Name, color.Color);
                var colorHex = color.Color.ToHexWeb();
                sb.AppendLine($"    --{name}: {colorHex};");
            }

            sb.AppendLine();
        }
        sb.Append("}");

        return sb.ToString();

        string canonicalize(string name, Color color)
        {
            if (name.Equals(color.ToHex(), StringComparison.OrdinalIgnoreCase))
            {
                name = string.Empty;
            }

            List<char> chars = name
                .Where(c => char.IsLetterOrDigit(c) || c is '_' or '-')
                .ToList();

            if (chars.Count == 0)
            {
                return "color" + color.ToHex().Substring(1);
            }
            else
            {
                if (name.StartsWith("--") ||
                    (name[0] is '-' && char.IsDigit(name[1])) ||
                    char.IsDigit(name[0]))
                {
                    name = "color" + name;
                }

                name = new string(chars.ToArray());
                return name;
            }
        }
    }

    public async Task<string> GetClipboardHtml()
    {
        var css = await ReadExportCSS();

        StringBuilder sb = new();
        sb.Append("<!DOCTYPE html>");
        sb.AppendLine("<html><head><title>Color palette</title><style>");
        sb.AppendLine(css);
        sb.AppendLine("</style></head><body>");

        foreach (var group in this.Groups)
        {
            sb.AppendLine($"<div class=\"palette-header\">{group.Name}</div>");
            sb.AppendLine("<div class=\"palette-container\">");
            foreach (var color in group.Colors)
            {
                var name = color.Name.Equals(color.Color.ToHex(), StringComparison.OrdinalIgnoreCase) ? "" : " " + color.Name;
                var colorHex = color.Color.ToHexWeb();
                sb.AppendLine($"<div class=\"color-rect\" style=\"background: {colorHex};\"><div class=\"color-name\">{colorHex} {name}</div></div>");
            }
            sb.AppendLine("</div>");
        }

        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    private async Task<string> ReadExportCSS()
    {
        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///htmlexport.css"));
        using (var inputStream = await file.OpenReadAsync())
        using (var classicStream = inputStream.AsStreamForRead())
        using (var streamReader = new StreamReader(classicStream))
        {
            return await streamReader.ReadToEndAsync();
        }
    }
}
