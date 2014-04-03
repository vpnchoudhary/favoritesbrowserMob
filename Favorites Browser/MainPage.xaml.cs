using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Favorites_Browser.Resources;
using Favorites_Browser;
using Favorites_Browser.Data;
using Favorites_Browser.Common;

namespace Favorites_Browser
{
    
    public partial class MainPage : PhoneApplicationPage
    {
        private string selectedSearchEngine;
        private Microsoft.Phone.Shell.ApplicationBarIconButton AppBarButton_Edit;
        private Microsoft.Phone.Shell.ApplicationBarIconButton AppBarButton_delete;
        private Microsoft.Phone.Shell.ApplicationBarIconButton AppBarButton_Move;
        List<SkyDriveDataCommon> selectedItems = new List<SkyDriveDataCommon>();
        ApplicationBar actionAppbar = null;
        ApplicationBar defaultAppbar = null;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            defaultAppbar = (ApplicationBar)ApplicationBar;
            ActionAppBarButtonInit();
            this.Loaded += MainPage_Loaded;
            SkyDriveDataCommon.ItemSelectionEvent += new EventHandler<ItemSelectionEventArguments>(ItemSelectionEventHander);
            selectedSearchEngine = constants.SearchEngineBing;
            
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void ActionAppBarButtonInit()
        {
            actionAppbar = new Microsoft.Phone.Shell.ApplicationBar();
            actionAppbar.BackgroundColor = defaultAppbar.BackgroundColor;
            AppBarButton_Edit = new ApplicationBarIconButton();
            AppBarButton_Edit.IconUri = new Uri("/Assets/edit.png", UriKind.Relative);
            AppBarButton_Edit.Text = "Rename";
            actionAppbar.Buttons.Add(AppBarButton_Edit);
            AppBarButton_Edit.Click += new EventHandler(AppBarButton_Edit_Click);

            AppBarButton_delete = new ApplicationBarIconButton();
            AppBarButton_delete.IconUri = new Uri("/Assets/delete.png", UriKind.Relative);
            AppBarButton_delete.Text = "Delete";
            actionAppbar.Buttons.Add(AppBarButton_delete);
            AppBarButton_delete.Click += new EventHandler(AppBarButton_Delete_Click);

            AppBarButton_Move = new ApplicationBarIconButton();
            AppBarButton_Move.IconUri = new Uri("/Assets/move.png", UriKind.Relative);
            AppBarButton_Move.Text = "Move";
            actionAppbar.Buttons.Add(AppBarButton_Move);
            AppBarButton_Move.Click += new EventHandler(AppBarButton_Move_Click);

        }

        //ObservableCollection<FolderList> listForFolder = new ObservableCollection<FolderList>();

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.ProgressBar.Visibility = System.Windows.Visibility.Visible;
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            if (App.FolderStack.Count == 0)
            {
                var sampleDataGroups = await source.GetGroups("Root");
                try
                {
                    
                    Bookmarks.ItemsSource = sampleDataGroups.GetGroupedList();// sampleDataGroups.AllItems.ToList();
                    source.CurrentFolder = sampleDataGroups;
                    App.FolderStack.Add(sampleDataGroups);
                    //this.listForFolder.Add(new FolderList(sampleDataGroups));
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            else
            {
                await this.RevertEditState();
                //this.Bookmarks.ItemsSource = App.FolderStack[App.FolderStack.Count - 1].AllItems.ToList();
                this.Bookmarks.ItemsSource = App.FolderStack[App.FolderStack.Count - 1].GetGroupedList();
            }
            this.ProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            
            this.FoldersList.ItemsSource = App.FolderStack;
        }

        protected override async void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            this.SearchBox.Visibility = System.Windows.Visibility.Collapsed;
            this.FoldersList.Visibility = System.Windows.Visibility.Visible;
            await this.RevertEditState();
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            // Use the navigation frame to return to the previous page
            if (App.FolderStack.Count > 1)
            {
                //this.Bookmarks.ItemsSource = App.FolderStack[App.FolderStack.Count - 2].AllItems.ToList();
                this.Bookmarks.ItemsSource = App.FolderStack[App.FolderStack.Count - 2].GetGroupedList();
                source.CurrentFolder = App.FolderStack[App.FolderStack.Count - 2];
                App.FolderStack.RemoveAt(App.FolderStack.Count - 1);
                //this.listForFolder.RemoveAt(this.listForFolder.Count - 1);
                //this.glyph.Visibility = System.Windows.Visibility.Visible;
                //this.DefaultViewModel["SelectedFolderName"] = source.CurrentFolder.Title;
            }
            if (App.FolderStack.Count == 1)
            {
                //this.Bookmarks.ItemsSource = App.FolderStack[0].AllItems.ToList();
                this.Bookmarks.ItemsSource = App.FolderStack[0].GetGroupedList();
                //this.glyph.Visibility = System.Windows.Visibility.Collapsed;
                //this.DefaultViewModel["SelectedFolderName"] = App.FolderStack[0].Title;
                source.CurrentFolder = App.FolderStack[0];
            }

            this.FoldersList.ItemsSource = App.FolderStack;
            e.Cancel = true;
            base.OnBackKeyPress(e);

        }

        private async void OnSearchTextChange(object sender, RoutedEventArgs e)
        {
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            var result = source.SearchSuggestions(this.SearchBox.Text);
            this.SearchBox.ItemsSource = result;

        }

        private async void SearchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string selecteditem = (string)e.AddedItems[0];
                this.SearchBox.Text = selecteditem;
                await this.NavigateToSelectedText(this.SearchBox.Text);
            }
        }

       
        private async void SearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                await this.NavigateToSelectedText(this.SearchBox.Text);
            }
        }

        private async Task NavigateToSelectedText(string searchText)
        {
            //reset titlebar state before navigate
            this.SearchBox.Visibility = System.Windows.Visibility.Collapsed;
            this.FoldersList.Visibility = System.Windows.Visibility.Visible;
            if (string.IsNullOrEmpty(searchText.Trim()))
            {
                return;
            }
            string url = searchText;
            Uri urlToBrowse = Utility.IsInternetURL(url,true);
            if (urlToBrowse == null)
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
                        else
                        {
                            url = fileCollection.URL;
                        }
                        //fileCollection.URL = System.Net.WebUtility.UrlEncode(fileCollection.URL);
                    }
                    
                }
                catch { url = "http://www.bing.com"; };
            }
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri(url, UriKind.Absolute);
            webBrowserTask.Show();
           

            //this.NavigationService.Navigate(new Uri("/View/ContentViewerWithAppBar.xaml?msg=" + searchText, UriKind.Relative));
        }

        private void Search_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.SearchBox.Visibility == System.Windows.Visibility.Collapsed)
            {
                this.SearchBox.Visibility = System.Windows.Visibility.Visible;
                this.FoldersList.Visibility = System.Windows.Visibility.Collapsed;
                this.SearchBox.Focus();
            }
            else if (string.IsNullOrEmpty(this.SearchBox.Text.Trim()))
            {
                this.SearchBox.Visibility = System.Windows.Visibility.Collapsed;
                this.FoldersList.Visibility = System.Windows.Visibility.Visible;
                this.Bookmarks.Focus();
            }
            else
            {
                this.Bookmarks.Focus();
                this.NavigateToSelectedText(this.SearchBox.Text);
            }
           
        }

        private void DetermineTitlePanelVisibility()
        {
           

        }

        private void AppBarButton_Add_Click(object sender, EventArgs e)
        {
            this.CreateFolderButton.Visibility = System.Windows.Visibility.Visible;
            this.UpdateFolderButton.Visibility = System.Windows.Visibility.Collapsed;
            this.CreateFolderName.Text = "";
            this.CreateFolderPopUP.HorizontalOffset = (Application.Current.Host.Content.ActualWidth - 400)/2;
            this.CreateFolderPopUP.VerticalOffset = (Application.Current.Host.Content.ActualHeight - 180) / 2;
            this.CreateFolderPopUP.IsOpen = true;
        }

        private async void AppBarButton_Refresh_Click(object sender, EventArgs e)
        {
            this.ProgressBar.Visibility = System.Windows.Visibility.Visible;
            await UIRefresh();
            this.ProgressBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private async Task UIRefresh()
        {
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            source.CurrentFolder = await source.Refresh(source.CurrentFolder.UniqueId);
            App.FolderStack.RemoveAt(App.FolderStack.Count - 1);
            App.FolderStack.Add(source.CurrentFolder);
            Bookmarks.ItemsSource = source.CurrentFolder.GetGroupedList();
            await this.RevertEditState();
        }

        private async void AppBarButton_Delete_Click(object sender, EventArgs e)
        {
            this.ProgressBar.Visibility = System.Windows.Visibility.Visible;
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            if (selectedItems.Count > 0)
            {
                foreach (SkyDriveDataCommon item in selectedItems)
                {
                    await source.DeleteFile(item.UniqueId);
                    switch (item.Type)
                    {
                        case SkyDriveItemType.File: source.CurrentFolder.Items.Remove((SkyDriveFile)item);
                            break;
                        case SkyDriveItemType.Folder: source.CurrentFolder.Subalbums.Remove((SkyDriveFolder)item);
                            break;
                    }
                }
                await UIRefreshWithoutFetch();
            }
            selectedItems.Clear();
            DetermineBottomAppBarVisibility();
            this.ProgressBar.Visibility = System.Windows.Visibility.Collapsed;

        }

        private async Task UIRefreshWithoutFetch()
        {
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            Bookmarks.ItemsSource = source.CurrentFolder.GetGroupedList();
            await this.RevertEditState();
        }

        private async void AppBarButton_Edit_Click(object sender, EventArgs e)
        {
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            if (selectedItems.Count > 0)
            {
                
                //edit the first selected item
                if (selectedItems[0].Type == SkyDriveItemType.File)
                {
                    //Modify Add file popup
                    this.UpdateBookmarkButton.Visibility = System.Windows.Visibility.Visible;
                    this.AddBookmarkName.Text = ((SkyDriveFile)this.selectedItems[0]).Title;
                    this.AddBookmarkURL.Text = ((SkyDriveFile)this.selectedItems[0]).URL;
                    this.AddBookmarkURL.IsReadOnly = "true";
                    // open Add File popup
                    this.AddBookmarkPopup.HorizontalOffset = (Application.Current.Host.Content.ActualWidth - 400) / 2;
                    this.AddBookmarkPopup.VerticalOffset = (Application.Current.Host.Content.ActualHeight - 230) / 2;
                    this.AddBookmarkPopup.IsOpen = true;
                }
                else
                {
                    //Modify New Folder popup
                    this.CreateFolderName.Text = ((SkyDriveFolder)this.selectedItems[0]).Title;
                    this.CreateFolderButton.Visibility = System.Windows.Visibility.Collapsed;
                    this.UpdateFolderButton.Visibility = System.Windows.Visibility.Visible;
                    //open create folder pop
                    this.CreateFolderPopUP.HorizontalOffset = (Application.Current.Host.Content.ActualWidth - 400) / 2;
                    this.CreateFolderPopUP.VerticalOffset = (Application.Current.Host.Content.ActualHeight - 180) / 2;
                    this.CreateFolderPopUP.IsOpen = true;
                }
            }
        }

        private async void AppBarButton_Move_Click(object sender, EventArgs e)
        {
            this.SelectDestinationPopup.HorizontalOffset = (Application.Current.Host.Content.ActualWidth - 365)/2;
            this.SelectDestinationPopup.VerticalOffset = (Application.Current.Host.Content.ActualHeight - 450) / 2;
            await this.SelectFolderUserCntl.LoadFolder();
            this.SelectDestinationPopup.IsOpen = true;
        }

        private async void AppBarButton_Select_Click(object sender, EventArgs e)
        {
            var source = await SkyDriveDataSource.GetInstance();
            // toggle checkbox visibility
                foreach (var item in source.CurrentFolder.AllItems)
                {
                    item.IsChecked = false;
                    item.ShowCheckBox = !item.ShowCheckBox;
                }
        }

        private async Task RevertEditState()
        {
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            // we want to revert back all items to non-editable state
            foreach (var item in source.CurrentFolder.AllItems)
            {
                item.IsChecked = false;
                item.ShowCheckBox = false;
            }
        }

              
        private bool IsCheckBox(DependencyObject obj)
        {
            while (obj != null)
            {
                if (obj is CheckBox)
                {
                    
                    return true;
                }

                obj = System.Windows.Media.VisualTreeHelper.GetParent(obj);
            }
            return false;
        }

        private async void Bookmarks_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.SearchBox.Visibility = System.Windows.Visibility.Collapsed;
            this.FoldersList.Visibility = System.Windows.Visibility.Visible;
            var obj = e.OriginalSource as DependencyObject;
            if (IsCheckBox(obj))
            {
                ((LongListSelector)sender).SelectedItem = null;
                return;
            }
            if (e.OriginalSource is Border)
            {
                ((LongListSelector)sender).SelectedItem = null;
                return;
            }
            //after this time we only want to process SkyDriveDataCommon type of objects
            if (sender is LongListSelector && ((LongListSelector)sender).SelectedItem != null)
            {
                SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
                if (((SkyDriveDataCommon)(((LongListSelector)sender).SelectedItem)).Type == SkyDriveItemType.AdBig)
                {
                    return;
                }
                if (((SkyDriveDataCommon)(((LongListSelector)sender).SelectedItem)).Type == SkyDriveItemType.Folder)
                {
                    await this.RevertEditState();
                    this.ProgressBar.Visibility = System.Windows.Visibility.Visible;
                    var matches = await source.GetGroups(((SkyDriveFolder)(((LongListSelector)sender).SelectedItem)).UniqueId);
                    //this.Bookmarks.ItemsSource = matches.AllItems.ToList();
                    this.Bookmarks.ItemsSource = matches.GetGroupedList();
                    App.FolderStack.Add(matches);
                    source.CurrentFolder = matches;
                    //this.listForFolder.Add(new FolderList(matches));
                    this.FoldersList.ItemsSource = App.FolderStack;
                    this.ProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    //this.glyph.Visibility = System.Windows.Visibility.Visible;
                    //this.DefaultViewModel["SelectedFolderName"] = source.CurrentFolder.Title;
                }
                else if (((SkyDriveFile)(((LongListSelector)sender).SelectedItem)).FileExtenstion.ToLower() == ".url")
                {
                    string url = ((SkyDriveFile)(((LongListSelector)sender).SelectedItem)).URL;
                    if (!url.Equals(string.Empty))
                    {
                        this.RevertEditState();
                        this.NavigateToSelectedText(url);
                        //this.NavigationService.Navigate(new Uri("/View/ContentViewerWithAppBar.xaml?msg=" + url, UriKind.Relative));
                    }
                }
                ((LongListSelector)sender).SelectedItem = null;
            }

        }

        private void ItemSelectionEventHander(object sender, ItemSelectionEventArguments e)
        {
            if (e.SelectedItem.IsChecked)
            {
                if (!this.selectedItems.Any(skItem => skItem == e.SelectedItem))
                {
                    this.selectedItems.Add(e.SelectedItem);
                }
            }
            else
            {
                if (this.selectedItems.Any(skItem => skItem == e.SelectedItem))
                {
                    this.selectedItems.Remove(e.SelectedItem);
                }
            }
            this.DetermineBottomAppBarVisibility();
        }

        private void DetermineBottomAppBarVisibility()
        {
            if (this.selectedItems.Count > 0)
            {
                ApplicationBar = actionAppbar;
                if (this.selectedItems.Count > 1)
                {
                    this.AppBarButton_Edit.IsEnabled = false;
                }
                else
                {
                    this.AppBarButton_Edit.IsEnabled = true;
                }
            }
            else
            {
                ApplicationBar = defaultAppbar;
            }
        }

        private void AppBarButton_Search_Click(object sender, EventArgs e)
        {
            if (this.SearchBox.Visibility == System.Windows.Visibility.Collapsed)
            {
                this.SearchBox.Visibility = System.Windows.Visibility.Visible;
                this.FoldersList.Visibility = System.Windows.Visibility.Collapsed;
                this.SearchBox.Focus();
            }
            else if (string.IsNullOrEmpty(this.SearchBox.Text.Trim()))
            {
                this.SearchBox.Visibility = System.Windows.Visibility.Collapsed;
                this.FoldersList.Visibility = System.Windows.Visibility.Visible;
                this.Bookmarks.Focus();
            }
            else
            {
                this.Bookmarks.Focus();
                this.NavigateToSelectedText(this.SearchBox.Text);
            }
        }

        private async void PopupCreateFolder_Click(object sender, RoutedEventArgs e)
        {
            this.CreateFolderPopUP.IsOpen = false;
            //this.BottomAppBar.IsOpen = false;
            this.ProgressBar.Visibility = System.Windows.Visibility.Visible;
            var source = await SkyDriveDataSource.GetInstance();
            if (this.CreateFolderName.Text.Trim() != string.Empty)
            {
                var skFolder = source.CurrentFolder.Subalbums.FirstOrDefault(sf => sf.Title.ToLower() == this.CreateFolderName.Text.Trim().ToLower());
                if (object.Equals(skFolder, null))
                {
                    SkyDriveFolder folder = new SkyDriveFolder(this.CreateFolderName.Text.Trim());
                    folder.Parent_id = source.CurrentFolder.UniqueId;
                    folder.UniqueId = await source.CreateFolder(folder);
                    source.CurrentFolder.Subalbums.Add(folder);
                }
                this.CreateFolderName.Text = string.Empty;
            }
            await UIRefreshWithoutFetch();
            this.ProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            //mySearchBox.FocusOnKeyboardInput = true;

        }

        private async void PopupUpdateFolder_Click(object sender, RoutedEventArgs e)
        {
            this.CreateFolderPopUP.IsOpen = false;
            //this.BottomAppBar.IsOpen = false;
            this.ProgressBar.Visibility = System.Windows.Visibility.Visible;
            var source = await SkyDriveDataSource.GetInstance();

            if (this.CreateFolderName.Text.Trim() != string.Empty && selectedItems[0].Type == SkyDriveItemType.Folder)
            {
                var skFolder = source.CurrentFolder.Subalbums.FirstOrDefault(sf => sf.Title.ToLower() == this.CreateFolderName.Text.Trim().ToLower());
                if (skFolder == null)
                {
                    selectedItems[0].UniqueId = await source.UpdateFolder(selectedItems[0].UniqueId, this.CreateFolderName.Text.Trim());
                    selectedItems[0].Title = this.CreateFolderName.Text.Trim();
                }
                this.CreateFolderName.Text = string.Empty;
            }
            await UIRefreshWithoutFetch();
            this.ProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            //mySearchBox.FocusOnKeyboardInput = true;
        }

        private async void PopupUpdateBookmark_Click(object sender, RoutedEventArgs e)
        {
            this.ProgressBar.Visibility = System.Windows.Visibility.Visible;
            this.AddBookmarkPopup.IsOpen = false;
            //this.BottomAppBar.IsOpen = false;
            var source = await SkyDriveDataSource.GetInstance();
            if (this.AddBookmarkName.Text.Trim() != string.Empty)
            {
                this.selectedItems[0].Title = AddBookmarkName.Text.Trim();
                bool result = await source.UpdateFile((SkyDriveFile)this.selectedItems[0]);
                this.AddBookmarkName.Text = string.Empty;
                this.AddBookmarkURL.Text = string.Empty;
            }
            await UIRefreshWithoutFetch();
            this.ProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            //mySearchBox.FocusOnKeyboardInput = true;
        }

        private void SelectDestinationPopup_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void FolderSelected_Click(object sender, SkyDriveFolder e)
        {
            SkyDriveFolder destination = null;
            if (e != null)
            {
                //move items 
                destination = e;
                this.SelectDestinationPopup.IsOpen = false;
                this.ProgressBar.Visibility = System.Windows.Visibility.Visible;
                var source = await SkyDriveDataSource.GetInstance();
                foreach (SkyDriveDataCommon item in selectedItems)
                {
                    try
                    {
                        await source.MoveItem(item.UniqueId, destination.UniqueId);
                        // remove moved items from present view
                        // and add them to destination. this addition will not work until we append all results/data to root skydrivefolder
                        switch (item.Type)
                        {
                            case SkyDriveItemType.Folder: source.CurrentFolder.Subalbums.Remove((SkyDriveFolder)item);
                                destination.Subalbums.Add((SkyDriveFolder)item);
                                break;
                            case SkyDriveItemType.File: source.CurrentFolder.Items.Remove((SkyDriveFile)item);
                                destination.Items.Add((SkyDriveFile)item);
                                break;
                        }

                        //fallback until we do item 2 mentioned in above comment
                        // check if destination is in our back list. if it exists then add moved items to destination
                        foreach (SkyDriveFolder folder in App.FolderStack)
                        {
                            if (folder.UniqueId == destination.UniqueId)
                            {
                                switch (item.Type)
                                {
                                    case SkyDriveItemType.Folder: folder.Subalbums.Add((SkyDriveFolder)item);
                                        break;
                                    case SkyDriveItemType.File: folder.Items.Add((SkyDriveFile)item);
                                        break;
                                }
                                break;
                            }
                            foreach (SkyDriveFolder skItem in folder.Subalbums)
                            {
                                if (skItem.UniqueId == destination.UniqueId)
                                {
                                    switch (item.Type)
                                    {
                                        case SkyDriveItemType.Folder: skItem.Subalbums.Add((SkyDriveFolder)item);
                                            break;
                                        case SkyDriveItemType.File: skItem.Items.Add((SkyDriveFile)item);
                                            break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignore any exception becuase of wrong destination folder selection by user
                        //throw ex;
                    }
                }
                await UIRefreshWithoutFetch();
                this.ProgressBar.Visibility = System.Windows.Visibility.Collapsed;

            }

        }

        private void CreateFolderCancel_Click(object sender, RoutedEventArgs e)
        {
            this.CreateFolderPopUP.IsOpen = false;
        }

        private void UpdateBookmarkCancel_Click(object sender, RoutedEventArgs e)
        {
            this.AddBookmarkPopup.IsOpen = false;
        }

        private void SelectFolderUserCntl_CancelEvent(object sender, SkyDriveFolder e)
        {
            this.SelectDestinationPopup.IsOpen = false;
        }
        

        


        

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }

   
}