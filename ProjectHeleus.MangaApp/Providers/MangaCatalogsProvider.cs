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
        public async Task<IEnumerable<Catalog>> GetAllCatalogs()
        {
            using (var client = new HttpClient())
            {
                var mangas = await client.GetStringAsync(new Uri("http://localhost:5486/api/catalogs"));
                return JsonConvert.DeserializeObject<IEnumerable<Catalog>>(mangas);
            }
        }
    }

    public static class MangaCatalogsProviderExtensions
    {
        public static async Task<IEnumerable<Manga>> GetCatalogContent(this Catalog catalog)
        {
            using (var client = new HttpClient())
            {
                var mangas = await client.GetStringAsync(new Uri($"http://localhost:5486/api/catalogs/{catalog.Id}/"));
                return JsonConvert.DeserializeObject<IEnumerable<Manga>>(mangas);
            }
        }
    }
}