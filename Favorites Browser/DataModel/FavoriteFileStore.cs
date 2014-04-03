using Favorites_Browser.Common;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
//using Windows.Data.Xml.Dom;
using Windows.Storage;


namespace Favorites_Browser.Data
{

    //optimized synchronization object to use with using keyword.
    //todo, should be moved to utility class
    public sealed class AsyncLock
    {
        private readonly SemaphoreSlim m_semaphore = new SemaphoreSlim(1, 1);
        private readonly Task<IDisposable> m_releaser;

        public AsyncLock()
        {
            m_releaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        public Task<IDisposable> LockAsync()
        {
            var wait = m_semaphore.WaitAsync();
            return wait.IsCompleted ?
                        m_releaser :
                        wait.ContinueWith((_, state) => (IDisposable)state,
                            m_releaser.Result, CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        private sealed class Releaser : IDisposable
        {
            private readonly AsyncLock m_toRelease;
            internal Releaser(AsyncLock toRelease) { m_toRelease = toRelease; }
            public void Dispose() { m_toRelease.m_semaphore.Release(); }
        }
    }

    //This class caches the file object. the IsFileExists() should be used before geting details of a shortcut file (.url):
    // this IsFileExists(ref file) returns true if file exists in cache and returns file through ref file argument. This function also update its cache if ref file object contains url. 

    public class FavoriteFileStore
    {

        private XDocument document;
        private StorageFile storageFile;
        private static FavoriteFileStore instance = null;
        private FavoriteFileStore() { }
        public static async Task<FavoriteFileStore> GetInstance()
        {
            var mutex = new AsyncLock();
            using (await mutex.LockAsync())
            {
                if (instance == null)
                {
                    instance = new FavoriteFileStore();
                    await instance.loadXML();
                }
            }
            return instance;
        }

        private async Task loadXML()
        {
            storageFile = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("FavoriteFileStore.xml", CreationCollisionOption.OpenIfExists);
            //Windows.Data.Xml.Dom.XmlLoadSettings loadSettings = new Windows.Data.Xml.Dom.XmlLoadSettings();
            //loadSettings.ProhibitDtd = false; // sample
            //loadSettings.ResolveExternals = false; // sample
            
            try
            {
                document = XDocument.Load(await storageFile.OpenStreamForWriteAsync());
            }
            catch (Exception ex)
            {
                document = new XDocument(new XElement("Favorites"));
                ex.ToString();
            }
            //string content = await FileIO.ReadTextAsync(storageFile);
            if (document.Root.Descendants() == null)
            {
               document.Add(new XElement("Favorites"));
            }
        }

        public void AddFile()
        {

        }

        public void DeleteFile()
        {

        }

        public void ModifyFile()
        {

        }
        // update skydrive object if there is an entry in the xml otherwise add the file entry in xml
        public bool IsFileExists(ref SkyDriveFile file)
        {
            bool bFileExists = false;
            XElement node = null;
            try
            {
                string fileid = file.UniqueId;
                node = document.Root.Descendants("File").Where(x => (string)x.Attribute("id") == fileid).FirstOrDefault();
                                                   
                      //(@"Favorites/File[@id= """ + file.UniqueId + @"""]");
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            if (node != null)
            {
                //
                if (!string.IsNullOrEmpty(file.URL))
                {
                    node.Attribute("source").Value = file.Source;
                    node.Attribute("url").Value = file.URL;
                    node.Attribute("Updated_time").Value = file.Updated_time;
                    node.Attribute("iconid").Value = file.ImagePath;
                    node.Attribute("iconTileColor").Value = file.ImageDominantColor.ToString();
                    bFileExists = false;
                }
                else
                {
                    // file.Source = node.Attributes[3].NodeValue.ToString();
                    file.URL = node.Attribute("url").Value;
                    file.ImagePath = node.Attribute("iconid").Value;
                    file.ImageDominantColor = Utility.ColorFromString(node.Attribute("iconTileColor").Value);
                    bFileExists = true;
                    // file.Updated_time = node.Attributes[2].NodeValue.ToString();
                }
            }
            else
            {
                try
                {
                    document.Root.Add(new XElement("File",
                        new XAttribute("name",  file.Title),
                        new XAttribute("id",file.UniqueId),
                        new XAttribute("Updated_time", file.Updated_time),
                        new XAttribute("source", file.Source),
                        new XAttribute("url", file.URL),
                        new XAttribute("description", file.Description),
                        new XAttribute("size", Convert.ToString(file.Size)),
                        new XAttribute("iconid", file.ImagePath),
                        new XAttribute("iconTileColor", file.ImageDominantColor.ToString())));
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            return bFileExists;
        }

        //save the entire xml dom to file
        public async Task SaveXML()
        {
            document.Save(await storageFile.OpenStreamForWriteAsync());
        }
    }
}
