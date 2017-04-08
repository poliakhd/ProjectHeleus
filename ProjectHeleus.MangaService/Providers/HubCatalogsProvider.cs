using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Extensions;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Models.Contracts;
using ProjectHeleus.MangaService.Providers.Contracts;
using StructureMap;

namespace ProjectHeleus.MangaService.Providers
{
    public class HubCatalogsProvider
        : ICatalogsProvider
    {
        #region Private Members

        private readonly IContainer _container;
        private readonly ILogger<ICatalogsProvider> _logger;

        #endregion
        
        public HubCatalogsProvider(IContainer container, ILogger<ICatalogsProvider> logger)
        {
            _container = container;
            _logger = logger;
        }

        #region Implementation of ICatalogsProvider

        public async Task<IEnumerable<ICatalog>> GetCatalogsAsync()
        {
            var catalogs = new[]
            {
                new CatalogModel {Id = "mangafox.me", Url = "http://mangafox.me/"},
                new CatalogModel {Id = "readmanga.me", Url = "http://readmanga.me/"}
            };

            return await Task.FromResult(catalogs);
        }

        public async Task<IEnumerable<IManga>> GetCatalogContentAsync(CatalogType catalogType, SortType sort, int page)
        {
            var parser = _container.GetParser(catalogType);

            switch (sort)
            {
                case SortType.New:
                    return await parser.GetNewContent(page);
                case SortType.Popular:
                    return await parser.GetPopularContent(page);
                case SortType.Rating:
                    return await parser.GetRatingContent(page);
                case SortType.Update:
                    return await parser.GetUpdateContent(page);
            }

            return null;
        }

        #endregion
    }
}