﻿using ColorPickerShared;
using ColorPickerShared.Services;
using ColorPickerShared.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
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

        public MainPageViewModel ViewModel => this.DataContext as MainPageViewModel;

        public async Task Suspend()
        {
            await this.ViewModel.SuspendAsync();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            this.DataContext = new MainPageViewModel();

            if (e.Parameter is FileActivatedEventArgs fae && fae.Files.FirstOrDefault() is IStorageFile file)
            {
                await this.LoadFromFile(file);
            }
            else
            {
                await this.ViewModel.RestoreAsync();
            }
        }

        private async void ImportColors(object sender, RoutedEventArgs e)
        {
            var content = new ImportColorsDialog();
            var dialog = new ContentDialog();
            dialog.Title = "Import colors";
            dialog.PrimaryButtonText = "Import";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = content;

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var cgvm = ColorGroupViewModel.FromText(content.Text);

                if (cgvm != null)
                {
                    this.ViewModel.AddGroup(cgvm);
                }
            }
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Palette", new List<string>() { Constants.FileExtension });
            savePicker.SuggestedFileName = "New palette";

            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);

                // write to file
                await FileIO.WriteTextAsync(file, SessionSaver.Serialize(this.ViewModel.Groups));

                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status != FileUpdateStatus.Complete)
                {
                    this.infoBar.IsOpen = true;
                    this.infoBar.Title = "Save palette";
                    this.infoBar.Message = "File " + file.Name + " couldn't be saved.";
                    this.infoBar.Severity = InfoBarSeverity.Warning;
                }
            }
        }

        private async void Load(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(Constants.FileExtension);

            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                await LoadFromFile(file);
            }
        }

        private async Task LoadFromFile(IStorageFile file)
        {
            var currentGroups = this.ViewModel.Groups.ToList();
            try
            {
                var json = await FileIO.ReadTextAsync(file);
                var vms = SessionSaver.Deserialize(json);

                foreach (var vm in vms)
                {
                    this.ViewModel.AddGroup(vm);
                }

                // Remove current groups after loading succeeded
                foreach (var cgvm in currentGroups)
                {
                    this.ViewModel.Groups.Remove(cgvm);
                }
            }
            catch (Exception)
            {
                this.infoBar.IsOpen = true;
                this.infoBar.Title = "Open palette";
                this.infoBar.Message = "File " + file.Name + " couldn't be opened.";
                this.infoBar.Severity = InfoBarSeverity.Error;
            }
        }

        private async void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem selectedItem)
            {
                string format = selectedItem.Tag.ToString();
                if (format == "text")
                {
                    setClipboardText(this.ViewModel.GetClipboardText());
                }
                else if (format == "xaml")
                {
                    setClipboardText(this.ViewModel.GetClipboardXaml());
                }
                else if (format == "css")
                {
                    setClipboardText(this.ViewModel.GetClipboardCss());
                }
                else if (format == "html")
                {
                    setClipboardText(await this.ViewModel.GetClipboardHtml());
                }
            }

            void setClipboardText(string text)
            {
                var package = new DataPackage
                {
                    RequestedOperation = DataPackageOperation.Copy,
                };
                package.SetText(text);
                Clipboard.SetContent(package);
            }
        }
    }
}
