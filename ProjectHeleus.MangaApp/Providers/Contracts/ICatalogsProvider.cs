using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaApp.Models;

namespace ProjectHeleus.MangaApp.Providers.Contracts
{
    public interface ICatalogsProvider
    {
        string Url { get; set; }

        Task<IEnumerable<Catalog>> GetAllCatalogs();
        Task<IEnumerable<Manga>> GetCatalogContent(Catalog catalog);
        Task<IEnumerable<Manga>> GetCatalogContent(Catalog catalog, int page);
    }
}