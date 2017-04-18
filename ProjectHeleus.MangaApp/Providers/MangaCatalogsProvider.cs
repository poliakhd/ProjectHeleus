using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json;
using ProjectHeleus.MangaApp.Models;
using ProjectHeleus.MangaApp.Providers.Contracts;

namespace ProjectHeleus.MangaApp.Providers
{
    public class MangaCatalogsProvider
        : ICatalogsProvider
    {
        public string Url { get; set; }

        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<IEnumerable<CatalogModel>> GetAllCatalogs()
        {
            return
                JsonConvert.DeserializeObject<IEnumerable<CatalogModel>>(
                    await _httpClient.GetStringAsync(new Uri("http://localhost:5486/api/catalogs")));
        }

        public async Task<IEnumerable<MangaShortModel>> GetCatalogContent(CatalogModel catalog)
        {
            return await GetCatalogContent(catalog, 0);
        }

        public async Task<IEnumerable<MangaShortModel>> GetCatalogContent(CatalogModel catalog, int page)
        {
            return
                JsonConvert.DeserializeObject<IEnumerable<MangaShortModel>>(
                    await _httpClient.GetStringAsync(new Uri($"http://localhost:5486/api/catalogs/{catalog.Id}/{page}")));
        }
    }
}