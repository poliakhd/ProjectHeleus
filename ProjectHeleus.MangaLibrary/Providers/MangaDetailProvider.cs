namespace ProjectHeleus.MangaLibrary.Providers
{
    using System;
    using System.Threading.Tasks;
    using Windows.Web.Http;
    using Caliburn.Micro;
    using Interfaces;
    using Microsoft.Toolkit.Uwp;
    using Newtonsoft.Json;
    using Shared.Models;

    public class MangaDetailProvider : IDetailProvider
    {
        #region Implementation of IDetailProvider

        public async Task<MangaModel> GetMangaContent(CatalogModel catalog, MangaPreviewModel mangaPreview)
        {
            using (var request = new HttpHelperRequest(new Uri($"http://tenmanga.eastus2.cloudapp.azure.com/api/manga/{catalog.Id}/{mangaPreview.Id}"), HttpMethod.Get))
            {
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    return JsonConvert.DeserializeObject<MangaModel>(await response.GetTextResultAsync(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                }
            }
        }
        public async Task<ChapterImagesModel> GetMangaChapterContent(CatalogModel catalog, MangaModel manga, ChapterModel chapter)
        {
            using (var request = new HttpHelperRequest(new Uri($"http://tenmanga.eastus2.cloudapp.azure.com/api/manga/{catalog.Id}/{chapter.Id}"), HttpMethod.Get))
            {
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    return JsonConvert.DeserializeObject<ChapterImagesModel>(await response.GetTextResultAsync(), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                }
            }
        }

        #endregion
    }
}