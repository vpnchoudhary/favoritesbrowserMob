﻿#pragma checksum "C:\POC\phone\Favorites Browser\Favorites Browser\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A2C06840CE43CA6E895517B4F95771E7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Favorites_Browser.View;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Favorites_Browser {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.DataTemplate FolderListView;
        
        internal System.Windows.DataTemplate FileListView;
        
        internal System.Windows.DataTemplate myListViewTileTemplateSelector;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.AutoCompleteBox SearchBox;
        
        internal System.Windows.Controls.ListBox FoldersList;
        
        internal System.Windows.Controls.ProgressBar ProgressBar;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.LongListSelector Bookmarks;
        
        internal System.Windows.Controls.Primitives.Popup CreateFolderPopUP;
        
        internal System.Windows.Controls.Grid popGrid;
        
        internal Favorites_Browser.View.ClearableTextBox CreateFolderName;
        
        internal System.Windows.Controls.Button CreateFolderButton;
        
        internal System.Windows.Controls.Button UpdateFolderButton;
        
        internal System.Windows.Controls.Button CreateFolderCancel;
        
        internal System.Windows.Controls.Primitives.Popup AddBookmarkPopup;
        
        internal System.Windows.Controls.Border AddBookMarkBorder;
        
        internal Favorites_Browser.View.ClearableTextBox AddBookmarkName;
        
        internal Favorites_Browser.View.ClearableTextBox AddBookmarkURL;
        
        internal System.Windows.Controls.Button UpdateBookmarkButton;
        
        internal System.Windows.Controls.Primitives.Popup SelectDestinationPopup;
        
        internal Favorites_Browser.View.SelectFolderUserControl SelectFolderUserCntl;
        
        internal System.Windows.Controls.StackPanel DisableMask;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton AppBarButton_Add;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton AppBarButton_Select;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton AppBarButton_Refresh;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton AppBarButton_Search;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Favorites%20Browser;component/MainPage.xaml", System.UriKind.Relative));
            this.FolderListView = ((System.Windows.DataTemplate)(this.FindName("FolderListView")));
            this.FileListView = ((System.Windows.DataTemplate)(this.FindName("FileListView")));
            this.myListViewTileTemplateSelector = ((System.Windows.DataTemplate)(this.FindName("myListViewTileTemplateSelector")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.SearchBox = ((Microsoft.Phone.Controls.AutoCompleteBox)(this.FindName("SearchBox")));
            this.FoldersList = ((System.Windows.Controls.ListBox)(this.FindName("FoldersList")));
            this.ProgressBar = ((System.Windows.Controls.ProgressBar)(this.FindName("ProgressBar")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.Bookmarks = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("Bookmarks")));
            this.CreateFolderPopUP = ((System.Windows.Controls.Primitives.Popup)(this.FindName("CreateFolderPopUP")));
            this.popGrid = ((System.Windows.Controls.Grid)(this.FindName("popGrid")));
            this.CreateFolderName = ((Favorites_Browser.View.ClearableTextBox)(this.FindName("CreateFolderName")));
            this.CreateFolderButton = ((System.Windows.Controls.Button)(this.FindName("CreateFolderButton")));
            this.UpdateFolderButton = ((System.Windows.Controls.Button)(this.FindName("UpdateFolderButton")));
            this.CreateFolderCancel = ((System.Windows.Controls.Button)(this.FindName("CreateFolderCancel")));
            this.AddBookmarkPopup = ((System.Windows.Controls.Primitives.Popup)(this.FindName("AddBookmarkPopup")));
            this.AddBookMarkBorder = ((System.Windows.Controls.Border)(this.FindName("AddBookMarkBorder")));
            this.AddBookmarkName = ((Favorites_Browser.View.ClearableTextBox)(this.FindName("AddBookmarkName")));
            this.AddBookmarkURL = ((Favorites_Browser.View.ClearableTextBox)(this.FindName("AddBookmarkURL")));
            this.UpdateBookmarkButton = ((System.Windows.Controls.Button)(this.FindName("UpdateBookmarkButton")));
            this.SelectDestinationPopup = ((System.Windows.Controls.Primitives.Popup)(this.FindName("SelectDestinationPopup")));
            this.SelectFolderUserCntl = ((Favorites_Browser.View.SelectFolderUserControl)(this.FindName("SelectFolderUserCntl")));
            this.DisableMask = ((System.Windows.Controls.StackPanel)(this.FindName("DisableMask")));
            this.AppBarButton_Add = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("AppBarButton_Add")));
            this.AppBarButton_Select = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("AppBarButton_Select")));
            this.AppBarButton_Refresh = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("AppBarButton_Refresh")));
            this.AppBarButton_Search = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("AppBarButton_Search")));
        }
    }
}

