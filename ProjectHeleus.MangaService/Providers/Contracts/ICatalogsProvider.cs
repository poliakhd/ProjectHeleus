using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Models.Contracts;

namespace ProjectHeleus.MangaService.Providers.Contracts
{
    public interface ICatalogsProvider
    {
        Task<IEnumerable<ICatalog>> GetCatalogsAsync();

        Task<IEnumerable<IManga>> GetNewCatalogContentAsync(CatalogType catalog, int page);
        Task<IEnumerable<IManga>> GetUpdateCatalogContentAsync(CatalogType catalog, int page);
        Task<IEnumerable<IManga>> GetRatingCatalogContentAsync(CatalogType catalog, int page);
        Task<IEnumerable<IManga>> GetPopularCatalogContentAsync(CatalogType catalog, int page);
    }
}