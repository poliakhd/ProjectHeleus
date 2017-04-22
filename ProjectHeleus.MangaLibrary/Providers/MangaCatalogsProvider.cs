namespace ProjectHeleus.MangaLibrary.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.Web.Http;
    using Caliburn.Micro;
    using Interfaces;
    using Microsoft.Toolkit.Uwp;
    using Models;
    using Newtonsoft.Json;

    public class MangaCatalogsProvider
        : ICatalogsProvider
    {
        #region Private Members

        private readonly HttpClient _httpClient = new HttpClient();

        #endregion

        public string Url { get; set; }

        public async Task<BindableCollection<CatalogModel>> GetAllCatalogs()
        {
            using (var request = new HttpHelperRequest(new Uri("http://tenmanga.westeurope.cloudapp.azure.com/api/catalogs"), HttpMethod.Post))
            {
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    return JsonConvert.DeserializeObject<BindableCollection<CatalogModel>>(await response.GetTextResultAsync());
                }
            }
        }

        public async Task<BindableCollection<MangaShortModel>> GetCatalogContent(CatalogModel catalog)
        {
            return await GetCatalogContent(catalog, 0);
        }
        public async Task<BindableCollection<MangaShortModel>> GetCatalogContent(CatalogModel catalog, int page)
        {
            using (var request = new HttpHelperRequest(new Uri($"http://tenmanga.westeurope.cloudapp.azure.com/api/catalogs/{catalog.Id}/{page}"), HttpMethod.Get))
            {
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    return JsonConvert.DeserializeObject<BindableCollection<MangaShortModel>>(await response.GetTextResultAsync());
                }
            }
        }


        public async Task<BindableCollection<GenreModel>> GetCatalogGenres(CatalogModel catalog)
        {
            using (var request = new HttpHelperRequest(new Uri($"http://tenmanga.westeurope.cloudapp.azure.com/api/genres/{catalog.Id}/"), HttpMethod.Get))
            {
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    return JsonConvert.DeserializeObject<BindableCollection<GenreModel>>(await response.GetTextResultAsync());
                }
            }
        }

        public async Task<BindableCollection<SortModel>> GetCatalogSorts(CatalogModel catalog)
        {
            using (var request = new HttpHelperRequest(new Uri($"http://tenmanga.westeurope.cloudapp.azure.com/api/catalogs/{catalog.Id}/"), HttpMethod.Get))
            {
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    return JsonConvert.DeserializeObject<BindableCollection<SortModel>>(await response.GetTextResultAsync());
                }
            }
        }
        public async Task<BindableCollection<MangaShortModel>> GetCatalogContent(CatalogModel catalog, SortModel sort, int page)
        {
            using (var request = new HttpHelperRequest(new Uri($"http://tenmanga.westeurope.cloudapp.azure.com/api/catalogs/{catalog.Id}/{sort.Id}/{page}"), HttpMethod.Get))
            {
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    return JsonConvert.DeserializeObject<BindableCollection<MangaShortModel>>(await response.GetTextResultAsync());
                }
            }
        }
    }
}