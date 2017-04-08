using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Models.Contracts;

namespace ProjectHeleus.MangaService.Providers.Contracts
{
    public interface ICatalogsProvider
    {
        Task<IEnumerable<ICatalog>> GetCatalogsAsync();
        Task<IEnumerable<IManga>> GetCatalogContentAsync(CatalogType catalog, SortType sort, int page);
    }
}