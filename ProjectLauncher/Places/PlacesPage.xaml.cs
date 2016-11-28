using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UE4Launcher.Places
{
    public partial class PlacesPage : UserControl
    {
        private PlacesViewModel ViewModel => this.DataContext as PlacesViewModel;

        public PlacesPage()
        {
            this.InitializeComponent();
        }

        private void LocationListItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var location = (sender as ListBoxItem)?.DataContext as LocationViewModelBase;
            location?.Navigate(Utilities.IsCtrlDown);
        }

        private void RemoveFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.RemoveSelectedFavorite();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.SaveFavorites();
        }

        private void LocationListItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                var favorite = (sender as ListBoxItem)?.DataContext as FavoriteLocationViewModel;
                if (favorite == null)
                    return;

                this.ViewModel.TogglePublicity(favorite);
            }
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.CancelSearch();
                e.Handled = true;
            }
        }

        private void ShowAllSearchResultLink_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.ShowAllSearchResult();
        }

        private void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    this.ViewModel.SelectPreviousLocation();
                    this.LocationList.ScrollIntoView(this.ViewModel.SelectedLocation);
                    break;
                case Key.Down:
                    this.ViewModel.SelectNextLocation();
                    this.LocationList.ScrollIntoView(this.ViewModel.SelectedLocation);
                    break;
                case Key.Enter:
                    this.ViewModel.NavigateToSelectedLocation(Utilities.IsCtrlDown);
                    break;
            }
        }

        private void LocationListItem_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var location = (sender as ListBoxItem)?.DataContext as LocationViewModelBase;
                location?.Navigate(Utilities.IsCtrlDown);
            }
        }

        private void AddToFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            var location = (sender as Button)?.DataContext as FileSearchResultViewModel;
            if (location != null)
                this.ViewModel.AddToFavorite(location);
        }

        private void LocationList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.CancelSearch();
                e.Handled = true;
            }
        }

        private void CancelSearch()
        {
            this.SearchTextBox.Text = string.Empty;
        }
    }
}
