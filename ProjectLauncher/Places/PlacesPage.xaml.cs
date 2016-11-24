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
            if (location == null)
                return;

            if (PlacesPage.IsCtrlDown())
                Process.Start(location.Path);
            else
                Utilities.NavigateFile(location.Path);
        }

        private static bool IsCtrlDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
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
            if (PlacesPage.IsCtrlDown())
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
                this.SearchTextBox.Text = string.Empty;
                e.Handled = true;
            }
        }

        private void ShowAllSearchResultLink_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.ShowAllSearchResult();
        }
    }
}
