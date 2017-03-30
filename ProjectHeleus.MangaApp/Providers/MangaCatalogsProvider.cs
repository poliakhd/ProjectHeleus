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

        public async Task<IEnumerable<Catalog>> GetAllCatalogs()
        {
            using (var client = new HttpClient())
            {
                return JsonConvert.DeserializeObject<IEnumerable<Catalog>>(await client.GetStringAsync(new Uri("http://localhost:5486/api/catalogs")));
            }
        }

        public async Task<IEnumerable<Manga>> GetCatalogContent(Catalog catalog)
        {
            return await GetCatalogContent(catalog, 0);
        }
        public async Task<IEnumerable<Manga>> GetCatalogContent(Catalog catalog, int page)
        {
            using (var client = new HttpClient())
            {
                return JsonConvert.DeserializeObject<IEnumerable<Manga>>(await client.GetStringAsync(new Uri($"http://localhost:5486/api/catalogs/{catalog.Id}/{page}")));
            }
        }
    }
}