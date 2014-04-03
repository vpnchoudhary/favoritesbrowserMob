using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Live;
using Newtonsoft.Json.Linq;
using Windows.Storage;

namespace Favorites_Browser.Data
{
    class BookmarksData
    {
        private BookmarksData() { }

        private static BookmarksData bookmarksData = null;

        public static JArray jsonBookmarksData = null;

        private bool isBookmarksDataAvailable = false;

        public bool IsBookmarksDataAvailable
        {
            get { return isBookmarksDataAvailable; }
        }

        /// <summary>
        /// GetBookmarksDataInstance
        /// </summary>
        /// <returns></returns>
        public static BookmarksData GetBookmarksDataInstance()
        {
            if (null == bookmarksData)
            {
                bookmarksData = new BookmarksData();
                jsonBookmarksData = new JArray();
            }

            return bookmarksData;
        }

        /// <summary>
        /// PopulateBookmarksDataAsync
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<bool> PopulateBookmarksDataAsync(string path, bool bUseCache = false)
        {
            try
            {
             await PopulateBookmarksData(path);
             isBookmarksDataAvailable = true;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                isBookmarksDataAvailable = false;
            }

            return true;
        }

        /// <summary>
        /// PopulateBookmarksData
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private async Task<bool> PopulateBookmarksData(string path)
        {
            try
            {
                LiveConnectClient liveConnectClient = new LiveConnectClient(App.Session);
                LiveOperationResult liveOperationResult = await liveConnectClient.GetAsync(path);

                JObject jsonObject = JObject.Parse(liveOperationResult.RawResult);
                JArray jsonBookmarksArray = (JArray)jsonObject["data"];

                if (jsonBookmarksArray.Count > 0)
                {
                    foreach (JObject value in jsonBookmarksArray)
                    {
                        if ((string)value["type"] == "folder")
                        {
                            string folderPath = (string)value["id"] + "/files";
                            await PopulateBookmarksData(folderPath);
                        }
                        jsonBookmarksData.Add(value);
                    }
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }

            return true;
        }
    }
}
