using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Xml.Linq;
using System.Collections.ObjectModel;
//using Windows.Data.Xml.Dom;
using System.Threading.Tasks;
using Favorites_Browser.Common;

namespace Favorites_Browser.Data
{
    [Windows.Foundation.Metadata.WebHostHidden]
    class FavoriteItem : BindableBase
    {
        string _title;
        public string Title
        {
            get { return _title; }
            set { this.SetProperty(ref this._title, value); }
        }
        string _url;
        public string URL
        {
            get { return _url; }
            set { this.SetProperty(ref this._url, value); }
        }
        string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set { this.SetProperty(ref this._imagePath, value); }
        }
        bool _isChecked = true;
        public bool IsChecked
        {
            get { return _isChecked; }
            set { this.SetProperty(ref this._isChecked, value); }
        }
        public FavoriteItem(string title, string url, string imagepath)
        {
            _title = title;
            _url = url;
            if (String.IsNullOrEmpty(imagepath))
            {
                if (Uri.IsWellFormedUriString(_url, UriKind.Absolute))
                {
                    _imagePath = @"http://getfavicon.appspot.com/" + System.Net.WebUtility.UrlEncode(_url);
                }
                else
                {
                    _imagePath = @"ms-appx:///Assets/favorite.png";
                }
            }
            _isChecked = true;
        }
    }
    class FavoriteSuggestionProvider
    {
        ObservableCollection<SkyDriveFile> favoriteList = new ObservableCollection<SkyDriveFile>();
        public FavoriteSuggestionProvider()
        {
            this.loadXML();
        }
        public ObservableCollection<SkyDriveFile> FavoriteList
        {
            get { return favoriteList; }
        }
        public async void loadXML()
        {
            Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("XML"); // you can get the specific folder from KnownFolders or other folders via FolderPicker as well
            var storageFile = await storageFolder.OpenStreamForReadAsync("FavoriteSuggestionList.xml");
            //Windows.Data.Xml.Dom.XmlLoadSettings loadSettings = new Windows.Data.Xml.Dom.XmlLoadSettings();
            //loadSettings.ProhibitDtd = false; // sample
            //loadSettings.ResolveExternals = false; // sample
            XDocument doc = XDocument.Load(storageFile);
           // XmlDocument doc = await XmlDocument.LoadFromFileAsync(storageFile, loadSettings);
           // XmlNodeList groups = doc.Elements;//.SelectNodes("//FavoriteList/FavoriteItem");
            
            foreach (var group in doc.Root.Elements("FavoriteItem"))
            {
                SkyDriveFile favoriteItem = new SkyDriveFile((string)group.Attribute("Title"), (string)group.Attribute("URL"), (string)group.Attribute("ImagePath"));
                favoriteItem.IsChecked = true;
                favoriteItem.ShowCheckBox = true;
                favoriteList.Add(favoriteItem);
            }
        }
    }

   
    
}
