using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Favorites_Browser.Data;

namespace Favorites_Browser.Common
{
    public abstract class DataTemplateSelector : ContentControl
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }
    public class ListViewTileTemplateSelector : DataTemplateSelector
    {
        //These are public properties that will be used in the Resources section of the XAML.
        public DataTemplate FileItemTemplate { get; set; }
        public DataTemplate FolderItemTemplate { get; set; }
        public DataTemplate AdItemTemplate { get; set; }
        //public DataTemplate AdItemTemplate2 { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {

            if (item != null)
            {
                switch (((SkyDriveDataCommon)item).Type)
                {
                    case SkyDriveItemType.Folder: return FolderItemTemplate;
                    case SkyDriveItemType.File: return FileItemTemplate;
                    case SkyDriveItemType.AdBig: return AdItemTemplate;
                    //case SkyDriveItemType.AdSmall: return AdItemTemplate2;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
