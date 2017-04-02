using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Core;
using ProjectHeleus.MangaService.Extensions;
using ProjectHeleus.MangaService.Models.Mangas;
using ProjectHeleus.MangaService.Providers.Contracts;
using StructureMap;

namespace ProjectHeleus.MangaService.Providers
{
    public class HubCatalogsProvider
        : ICatalogsProvider
    {
        #region Private Members

        private readonly ApiContext _context;
        private readonly IContainer _container;

        #endregion
        
        public HubCatalogsProvider(ApiContext context, IContainer container)
        {
            _context = context;
            _container = container;
        }

        #region Implementation of ICatalogsProvider

        public async Task<IEnumerable<Models.Catalog>> GetCatalogsAsync()
        {
            return await _context.Sources.ToListAsync();
        }

        public async Task<IEnumerable<ListManga>> GetUpdateCatalogContentAsync(CatalogType catalogType, int page)
        {
            var parser = catalogType.GetParser(_container);
            var mangas = await parser.GetUpdateContent(page);

            return await Task.FromResult(mangas);
        }
        public async Task<IEnumerable<ListManga>> GetNewCatalogContentAsync(CatalogType catalogType, int page)
        {
            var parser = catalogType.GetParser(_container);
            var mangas = await parser.GetNewContent(page);

            return await Task.FromResult(mangas);
        }
        public async Task<IEnumerable<ListManga>> GetRatingCatalogContentAsync(CatalogType catalogType, int page)
        {
            var parser = catalogType.GetParser(_container);
            var mangas = await parser.GetRatingContent(page);

            return await Task.FromResult(mangas);
        }

        #endregion
    }
}