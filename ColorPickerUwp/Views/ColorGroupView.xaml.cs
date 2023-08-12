using ColorPickerShared.ViewModels;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using static ColorPickerShared.PointHelper;

namespace ColorPickerUwp.Views
{
    public sealed partial class ColorGroupView : UserControl
    {
        public ColorGroupView()
        {
            this.InitializeComponent();
        }

        public ColorGroupViewModel ViewModel => this.DataContext as ColorGroupViewModel;

        public bool ShowHex
        {
            get { return (bool)GetValue(ShowHexProperty); }
            set { SetValue(ShowHexProperty, value); }
        }

        public static readonly DependencyProperty ShowHexProperty =
            DependencyProperty.Register("ShowHex", typeof(bool), typeof(ColorGroupView), new PropertyMetadata(false));

        private void TargetListView_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = e.Data.Properties.ContainsKey("sourceItem")
                ? DataPackageOperation.Move : DataPackageOperation.None;
        }

        private void Colors_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            // The ListView is declared with selection mode set to Single.
            // But we want the code to be robust
            if (e.Items.Count == 1)
            {
                var dataItem = e.Items[0] as ColorViewModel;
                e.Data.Properties.Add("sourceItem", dataItem);
                e.Data.Properties.Add("sourceGrid", sender as GridView);

                e.Data.RequestedOperation = DataPackageOperation.Move;
            }
        }

        private void TargetListView_Drop(object sender, DragEventArgs e)
        {
            var sourceItem = e.Data.Properties.TryGetValue("sourceItem", value: out object value) ? value as ColorViewModel : null;
            if (sourceItem is null)
            {
                return;
            }

            var sourceGrid = e.Data.Properties.TryGetValue("sourceGrid", value: out value) ? value as GridView : null;
            if (sourceItem is null)
            {
                return;
            }

            // remove from source
            var sourceList = sourceGrid.ItemsSource as ICollection<ColorViewModel>;
            sourceList.Remove(sourceItem);

            // add to target
            var viewModel = this.DataContext as ColorGroupViewModel;
            var position = e.GetPosition(this.Colors);

            var elementIndex = FindClosestContainer(position, this.Colors.ItemsPanelRoot);

            if (elementIndex < 0)
            {
                viewModel.Colors.Add(sourceItem);
            }
            else
            {
                viewModel.Colors.Insert(elementIndex, sourceItem);
            }
        }

        private int FindClosestContainer(Point position, Panel itemsPanelRoot)
        {
            var children = itemsPanelRoot.Children;
            var closestContainerIndex = -1;
            var shortestDistance = double.PositiveInfinity;

            for (var i = 0; i < children.Count; i++)
            {
                var container = children[i];
                var containerPosition = container.TransformToVisual(itemsPanelRoot).TransformPoint(new Point(0, 0));
                var containerCenter = new Point(containerPosition.X + container.ActualSize.X / 2.0, containerPosition.Y + container.ActualSize.Y / 2.0);
                var distance = Distance(position, containerCenter);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestContainerIndex = i;
                }
            }

            return closestContainerIndex;
        }

        private void NamePressed(object sender, PointerRoutedEventArgs e)
        {
            this.nameEdit.Visibility = Visibility.Visible;
            this.nameView.Visibility = Visibility.Collapsed;
            this.nameEdit.Focus(FocusState.Keyboard);
            this.nameEdit.SelectAll();
        }

        private void NameEditEnded(object sender, RoutedEventArgs args)
        {
            this.nameEdit.Visibility = Visibility.Collapsed;
            this.nameView.Visibility = Visibility.Visible;
        }

        private void NameEditKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                // we do not want to highlight the remove button
                this.moveUpButton.Focus(FocusState.Keyboard);
                this.nameEdit.Visibility = Visibility.Collapsed;
                this.nameView.Visibility = Visibility.Visible;
            }
        }

        private async void EditColor_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)sender).DataContext as ColorViewModel;
            await EditColor(item);
        }

        private void RemoveColor_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)sender).DataContext as ColorViewModel;
            this.ViewModel.Colors.Remove(item);
        }

        private async void AddColor(object sender, RoutedEventArgs e)
        {
            var item = new ColorViewModel()
            {
                Name = "New color",
                Color = Windows.UI.Colors.CornflowerBlue,
            };
            if (await EditColor(item, "Add color", "Add"))
            {
                this.ViewModel.Colors.Add(item);
            }
        }

        private static async Task<bool> EditColor(ColorViewModel item, string title = "Edit color", string primaryAction = "Update")
        {
            var editvm = new ColorEditViewModel()
            {
                Color = item.Color,
                ColorName = item.Name ?? "",
            };

            var dialog = new ContentDialog();
            dialog.Title = title;
            dialog.PrimaryButtonText = primaryAction;
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new ColorEditDialog()
            {
                DataContext = editvm,
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                item.Color = editvm.Color;
                item.Name = editvm.ColorName;
                return true;
            }

            return false;
        }

        private void ColorClicked(object sender, ItemClickEventArgs e)
        {
            var gridview = (GridView)sender;
            var container = gridview.ContainerFromIndex(gridview.Items.IndexOf(e.ClickedItem));
            var control = container.FindDescendant<Border>();

            FlyoutBase.ShowAttachedFlyout(control);
        }
    }
}
