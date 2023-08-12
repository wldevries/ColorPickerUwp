using ColorPickerShared.ViewModels;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ColorPickerUwp.Views
{
    public sealed partial class ColorGroupView : UserControl
    {
        public ColorGroupView()
        {
            this.InitializeComponent();
            this.DataContext = new ColorGroupViewModel();
        }

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

        /// <summary>
        /// Calculates the absolute distance between two points
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns>The absolute distance between the points</returns>
        private double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        // Remove this usercontrol from it's parent
        private void RemoveGroup(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as Panel;
            parent.Children.Remove(this);
        }

        private void MoveUp(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as Panel;
            var index = parent.Children.IndexOf(this);
            if (index > 0)
            {
                parent.Children.Move((uint)index, (uint)index - 1); ;
            }
        }

        private void MoveDown(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as Panel;
            var index = parent.Children.IndexOf(this);
            if (index < parent.Children.Count - 1)
            {
                parent.Children.Move((uint)index, (uint)index + 1); ;
            }
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
                this.showHex.Focus(FocusState.Keyboard);
                this.nameEdit.Visibility = Visibility.Collapsed;
                this.nameView.Visibility = Visibility.Visible;
            }
        }
    }
}
