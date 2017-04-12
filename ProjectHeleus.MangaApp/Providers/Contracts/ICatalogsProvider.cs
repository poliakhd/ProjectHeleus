using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaApp.Models;

namespace ProjectHeleus.MangaApp.Providers.Contracts
{
    public interface ICatalogsProvider
    {
        string Url { get; set; }

        Task<IEnumerable<CatalogModel>> GetAllCatalogs();
        Task<IEnumerable<MangaShortModel>> GetCatalogContent(CatalogModel catalog);
        Task<IEnumerable<MangaShortModel>> GetCatalogContent(CatalogModel catalog, int page);
    }
}