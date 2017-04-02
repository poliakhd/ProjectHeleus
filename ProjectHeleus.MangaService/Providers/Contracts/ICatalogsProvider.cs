using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Models.Mangas;

namespace ProjectHeleus.MangaService.Providers.Contracts
{
    public interface ICatalogsProvider
    {
        Task<IEnumerable<Catalog>> GetCatalogsAsync();

        Task<IEnumerable<ListManga>> GetNewCatalogContentAsync(CatalogType catalog, int page);
        Task<IEnumerable<ListManga>> GetUpdateCatalogContentAsync(CatalogType catalog, int page);
        Task<IEnumerable<ListManga>> GetRatingCatalogContentAsync(CatalogType catalog, int page);

    }
}