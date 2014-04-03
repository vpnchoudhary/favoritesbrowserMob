using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
//using Windows.Data.Xml.Dom;
using System.Threading.Tasks;
using Favorites_Browser.Common;
using System.Runtime.Serialization;
//using TCD.Serialization.Xml;


namespace Favorites_Browser.Data
{
    class DataSyncManager
    {
        private static SkyDriveFolder root;
        private static DataSyncManager instance = null;
        public static DataSyncManager GetInstance()
        {
            if(instance == null)
            {
                instance = new DataSyncManager();
            }
            return instance;
        }
        private DataSyncManager()
        {
             
        }

        public async Task GetFavorites()
        {
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            root = await source.GetFolder(source.Root);
            await FetchData(root);
        }

        public async Task SaveFavoriteXML()
        {
            var file = await Windows.Storage.ApplicationData.Current.RoamingFolder.CreateFileAsync(constants.FavoriteXML,
                                             CreationCollisionOption.ReplaceExisting);
            try
            {
                // serial data object to XML file specified in "DATA_FILE"
                //using(IRandomAccessStream  randomAccessStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                
                //{
                    //XmlDeSerializer.SerializeToStream(await file.OpenStreamForWriteAsync(), root);
                    //TCD.Serialization.Xml.XmlDeSerializer.
                    //IOutputStream sessionOutputStream = randomAccessStream.GetOutputStreamAt(0);
                    //var xns = new XmlSerializerNamespaces();
                    //xns.Add(string.Empty, string.Empty);
                    //XmlSerializer serializer = new XmlSerializer(root.GetType());
                    //serializer.Serialize(sessionOutputStream.AsStreamForWrite(), root, xns);
               // }
               

               // System.IO.Stream stream = randomAccessStream.WriteAsync()
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                // handle any kind of exceptions
            }

        }

        //recursively populate data from source
        private async Task FetchData(SkyDriveFolder folder)
        {
            //at this point we don't have folder contents. We need to make calls to get folder contents:
            SkyDriveDataSource source = await SkyDriveDataSource.GetInstance();
            folder = await source.GetGroups(folder.UniqueId, false, true);

            foreach (SkyDriveFolder subFolder in folder.Subalbums)
            {
                await FetchData(subFolder);
                return;
            }
        }


    }
}
