using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Favorites_Browser.Data;
using Favorites_Browser.Common;

namespace Favorites_Browser.View
{
    public partial class ContentViewerWithAppBar : PhoneApplicationPage
    {
        private string selectedSearchEngine;

        public ContentViewerWithAppBar()
        {
            InitializeComponent();
            selectedSearchEngine = constants.SearchEngineBing;
            //BrowserControl.UnviewableContentIdentified += FavoriteBrowser_UnviewableContentIdentified;
            //BrowserControl.NavigationStarting += FavoriteBrowser_NavigationStarting;
            //BrowserControl.ContentLoading += FavoriteBrowser_ContentLoading;
            //BrowserControl.DOMContentLoaded += FavoriteBrowser_DOMContentLoaded;
            //BrowserControl.NavigationCompleted += FavoriteBrowser_NavigationCompleted;
            //BrowserControl.UnsafeContentWarningDisplaying += FavoriteBrowser_UnsafeContentWarningDisplaying;
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string url;
            NavigationContext.QueryString.TryGetValue("msg", out url);
            Uri urlToBrowse = Utility.IsInternetURL(url);
            if (urlToBrowse != null)
            {
                this.SearchBox.Text = url;
                this.BrowserControl.Navigate(new Uri(url));
            }
            else
            {
                SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
                SkyDriveFile fileCollection = SkyDriveDataSource.searchResultContainer.Items.FirstOrDefault(sf => sf.Title.ToLower().Contains(url.ToLower()));
                try
                {
                    if (object.Equals(fileCollection, null))
                    {
                        url = selectedSearchEngine + Uri.EscapeUriString(url);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(fileCollection.URL))
                        {
                            await source.LoadFile(fileCollection);
                            url = fileCollection.URL;
                        }
                        //fileCollection.URL = System.Net.WebUtility.UrlEncode(fileCollection.URL);
                    }
                    this.SearchBox.Text = url;
                    this.BrowserControl.Navigate(new Uri(url));
                }
                catch { };
            }
            base.OnNavigatedTo(e);
        }

        private async void OnSearchTextChange(object sender, RoutedEventArgs e)
        {
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            var result = source.SearchSuggestions(this.SearchBox.Text);
            this.SearchBox.ItemsSource = result;
        }

        private void SearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.NavigateToSelectedText(this.SearchBox.Text);
            }
        }

        private void SearchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string selecteditem = (string)e.AddedItems[0];
                this.SearchBox.Text = selecteditem;
                this.NavigateToSelectedText(this.SearchBox.Text);
            }
        }

        private void NavigateToSelectedText(string searchText)
        {
            this.NavigationService.Navigate(new Uri("/View/ContentViewerWithAppBar.xaml?msg=" + searchText, UriKind.Relative));
        }

    }
}