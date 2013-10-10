

using System;
namespace SeriesManager.API
{
    public static class API
    {        
        public static readonly Uri BannerPath = new Uri("http://thetvdb.com/banners/");
        public static readonly Uri SeriesPath = new Uri("http://thetvdb.com/api/");
        public static readonly string Key = "B8489AFD55EF0375";

        //public static async Task<XDocument> Parse(StorageFolder folder, string shortLanguage)
        //{
        //    if (folder == null)
        //        return null;

        //    XDocument doc = null;
        //    string[] names = { shortLanguage, "Actors", "Banners" };

        //    foreach (string name in names)
        //    {
        //        try
        //        {
        //            var file = await folder.GetFileAsync(name.ToLower() + ".xml");
        //            var content = await FileIO.ReadTextAsync(file);
        //            if (doc == null)
        //            {
        //                doc = XDocument.Parse(content);
        //                continue;
        //            }

        //            var tmp = XDocument.Parse(content);
        //            doc.Element("Data").Add(tmp.Element(name));
        //        }
        //        catch { return null; }
        //    }

        //    return doc;
        //}

        //public async Task Load()
        //{
        //    string shortLanguage = this.language.ToShort();

        //    if (!await Local.Exists(this.ID, shortLanguage + ".xml"))
        //    {
        //        string url = API.SeriesPath + API.Key + "/series/" + this.ID + "/all/" + shortLanguage + ".zip";
        //        Stream stream = null;

        //        try
        //        {
        //            HttpClient httpClient = new HttpClient();
        //            HttpResponseMessage response = await httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            stream = await response.Content.ReadAsStreamAsync();
        //        }
        //        catch { return; }

        //        var seriesFolder = await Local.Put(this.ID);
        //        using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
        //        {
        //            foreach (var entry in archive.Entries)
        //            {
        //                StorageFile file = null;

        //                if (await seriesFolder.TryGetItemAsync(entry.Name) != null) continue;

        //                file = await seriesFolder.CreateFileAsync(entry.Name, CreationCollisionOption.FailIfExists);
        //                using (var reader = new StreamReader(entry.Open()))
        //                {
        //                    string xmlText = reader.ReadToEnd();
        //                    await FileIO.WriteTextAsync(file, xmlText);
        //                }
        //            }
        //        }
        //    }

        //    var folder = await Local.Get(this.ID);
        //    var newContent = await API.Parse(folder, this.language);
        //    ApplicationData.Current.LocalSettings.Values[this.ID.ToString()] = DateTime.Now.ToUnixTime();
        //    this.Update(newContent);
        //}
    }
}
