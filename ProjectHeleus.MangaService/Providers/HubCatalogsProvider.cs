using System.Collections.Generic;
using System.Threading.Tasks;
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

        #endregion
        
        public HubCatalogsProvider(IContainer container)
        {
            _container = container;
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

        public async Task<IEnumerable<IManga>> GetUpdateCatalogContentAsync(CatalogType catalogType, int page)
        {
            var parser = _container.GetParser(catalogType);
            var mangas = await parser.GetUpdateContent(page);

            return await Task.FromResult(mangas);
        }
        public async Task<IEnumerable<IManga>> GetNewCatalogContentAsync(CatalogType catalogType, int page)
        {
            var parser = _container.GetParser(catalogType);
            var mangas = await parser.GetNewContent(page);

            return await Task.FromResult(mangas);
        }
        public async Task<IEnumerable<IManga>> GetRatingCatalogContentAsync(CatalogType catalogType, int page)
        {
            var parser = _container.GetParser(catalogType);
            var mangas = await parser.GetRatingContent(page);

            return await Task.FromResult(mangas);
        }
        public async Task<IEnumerable<IManga>> GetPopularCatalogContentAsync(CatalogType catalogType, int page)
        {
            var parser = _container.GetParser(catalogType);
            var mangas = await parser.GetPopularContent(page);

            return await Task.FromResult(mangas);
        }

        #endregion
    }
}