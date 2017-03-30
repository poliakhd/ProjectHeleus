using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Providers.Contracts;

namespace ProjectHeleus.MangaService.Controllers
{
    public class CatalogsController : Controller
    {
        #region Private Members

        private readonly ICatalogsProvider _catalogsProvider;

        #endregion

        public CatalogsController(ICatalogsProvider catalogsProvider)
        {
            _catalogsProvider = catalogsProvider;
        }

        [Route("api/[controller]")]
        public async Task<IEnumerable<Catalog>> GetAllCatalogs()
        {
            return await _catalogsProvider.GetCatalogsAsync();
        }

        [Route("api/[controller]/{catalog}")]
        public async Task<IEnumerable<Manga>> GetCatalogContent(CatalogType catalog)
        {
            return await GetCatalogContent(catalog, 0);
        }

        [Route("api/[controller]/{catalog}/{page}")]
        public async Task<IEnumerable<Manga>> GetCatalogContent(CatalogType catalog, int page)
        {
            return await _catalogsProvider.GetLatestCatalogContentAsync(catalog, page);
        }

        [Route("api/[controller]/{catalog}/{sort}")]
        public async Task<IEnumerable<Manga>> GetCatalogContent(CatalogType catalog, string sort)
        {
            return await GetCatalogContent(catalog, sort, 0);
        }

        [Route("api/[controller]/{catalog}/{sort}/{page}")]
        public async Task<IEnumerable<Manga>> GetCatalogContent(CatalogType catalog, string sort, int page)
        {
            var sortType = (SortType)Enum.Parse(typeof(SortType), sort, true);

            switch (sortType)
            {
                case SortType.Newest:
                    return await _catalogsProvider.GetNewestCatalogContentAsync(catalog, page);
                case SortType.Latest:
                    return await _catalogsProvider.GetLatestCatalogContentAsync(catalog, page);
                default:
                    return await _catalogsProvider.GetNewestCatalogContentAsync(catalog, page);
            }
        }
    }
}
