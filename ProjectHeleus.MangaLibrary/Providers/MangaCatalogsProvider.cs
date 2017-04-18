namespace ProjectHeleus.MangaLibrary.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.Web.Http;
    using Interfaces;
    using Models;
    using Newtonsoft.Json;

    public class MangaCatalogsProvider
        : ICatalogsProvider
    {
        #region Private Members

        private readonly HttpClient _httpClient = new HttpClient();

        #endregion

        public string Url { get; set; }

        public async Task<IEnumerable<CatalogModel>> GetAllCatalogs()
        {
            return JsonConvert.DeserializeObject<IEnumerable<CatalogModel>>(
                    await _httpClient.GetStringAsync(
                        new Uri("http://tenmanga.westeurope.cloudapp.azure.com/api/catalogs")
                    )
                );
        }

        public async Task<IEnumerable<MangaShortModel>> GetCatalogContent(CatalogModel catalog)
        {
            return await GetCatalogContent(catalog, 0);
        }
        public async Task<IEnumerable<MangaShortModel>> GetCatalogContent(CatalogModel catalog, int page)
        {
            return JsonConvert.DeserializeObject<IEnumerable<MangaShortModel>>(
                    await _httpClient.GetStringAsync(
                        new Uri($"http://tenmanga.westeurope.cloudapp.azure.com/api/catalogs/{catalog.Id}/{page}")
                    )
                );
        }
    }
}