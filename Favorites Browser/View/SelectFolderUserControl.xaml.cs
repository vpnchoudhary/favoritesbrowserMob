using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Phone.Controls;
using System.Windows.Controls;
using System.ComponentModel;
using System.Threading.Tasks;
using Favorites_Browser.Data;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Favorites_Browser.View
{
      
    public sealed partial class SelectFolderUserControl : UserControl
    {
        private SkyDriveFolder itemSelected = null;
        List<SkyDriveFolder> FolderStack = new List<SkyDriveFolder>();
        public event EventHandler<SkyDriveFolder> FolderSelectedEvent;
        public event EventHandler<SkyDriveFolder> CancelEvent;
       

        private bool sharing = false;
        public bool Sharing
        {
            get { return sharing; }
            set
            {
                sharing = value;
                if (sharing == true)
                {
                    this.progressRing.Visibility = System.Windows.Visibility.Visible;
                   // this.buttonSelect.IsEnabled = true;
                }
                else
                {
                    this.progressRing.Visibility = System.Windows.Visibility.Collapsed;
                    //this.buttonSelect.IsEnabled = false;
                }
            }
        }

        void OnFolderSelectedEvent(SkyDriveFolder e)
        {
            FolderSelectedEvent(this, e);
        }

        void OnCancelEvent(SkyDriveFolder e)
        {
            CancelEvent(this, e);
        }

        public SkyDriveFolder ItemSelected
        {
            get { return itemSelected;}
        }

        public SelectFolderUserControl()
        {
            this.InitializeComponent();
        }

 
        public async Task LoadFolder()
        {
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            var sampleDataGroups = await source.GetGroups(source.Root, true);
            FolderStack.Add(sampleDataGroups);
            itemSelected = sampleDataGroups;
            this.FolderName.Text = itemSelected.Title;
            SemListView.ItemsSource = sampleDataGroups.AllItems;
            Sharing = false;   
        }

        private async void ListItemSelected(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Sharing = true;
            try
            {
                this.FolderName.Text = ((SkyDriveFolder)((ListBox)sender).SelectedItem).Title;
            }
            catch { return; };
            //SemListView.ItemsSource = null;
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            itemSelected = await source.GetGroups(((SkyDriveFolder)((ListBox)sender).SelectedItem).UniqueId, true);
            SemListView.ItemsSource = itemSelected.AllItems;
            FolderStack.Add(itemSelected);
            Sharing = false;
        }

        private void SelectButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnFolderSelectedEvent(itemSelected);
        }

        private async void FolderUp(object sender, System.Windows.RoutedEventArgs e)
        {
            SemListView.ItemsSource = null;
            //this.mainProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            // Use the navigation frame to return to the previous page
            if (FolderStack.Count > 1)
            {
                //Sharing = true;
                Sharing = true;
                SemListView.ItemsSource = FolderStack[FolderStack.Count - 2].AllItems;
                itemSelected = FolderStack[FolderStack.Count - 2];
                FolderStack.RemoveAt(FolderStack.Count - 1);
                this.FolderName.Text = itemSelected.Title;
                Sharing = false;
            }
            if (FolderStack.Count == 1)
            {
                SemListView.ItemsSource = FolderStack[0].AllItems;
                itemSelected = FolderStack[0];
                this.FolderName.Text = itemSelected.Title;
            }

        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnCancelEvent(null);
        }
    }
}
