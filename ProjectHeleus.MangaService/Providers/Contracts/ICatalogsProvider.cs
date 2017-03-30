using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Models;

namespace ProjectHeleus.MangaService.Providers.Contracts
{
    public interface ICatalogsProvider
    {
        Task<IEnumerable<Catalog>> GetCatalogsAsync();

        Task<IEnumerable<Manga>> GetNewestCatalogContentAsync(CatalogType catalog, int page);
        Task<IEnumerable<Manga>> GetLatestCatalogContentAsync(CatalogType catalog, int page);
    }
}