namespace ProjectHeleus.MangaService.Providers
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    
    using Microsoft.Extensions.Logging;

    using StructureMap;

    using Core;
    using Models;
    using Extensions;
    using Interfaces;
    using Models.Interfaces;

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
                new CatalogModel {Id = "readmanga.me", Url = "http://readmanga.me/"},
                new CatalogModel {Id = "mintmanga.com", Url = "http://mintmanga.com/"}

            };

            return await Task.FromResult(catalogs);
        }

        public async Task<IEnumerable<IManga>> GetAllFromCatalogAsync(CatalogType catalogType, SortType sort, int page)
        {
            var parser = _container.GetParser(catalogType);
            return await parser.GetAllFromCatalogAsync(sort, page);
        }

        #endregion
    }
}