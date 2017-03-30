using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Core;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Parsers;
using ProjectHeleus.MangaService.Parsers.Contracts;
using ProjectHeleus.MangaService.Providers.Contracts;
using StructureMap;

namespace ProjectHeleus.MangaService.Providers
{
    public class MangasCatalogsProvider
        : ICatalogsProvider
    {
        #region Private Members

        private readonly ApiContext _context;
        private readonly IContainer _container;

        #endregion
        
        public MangasCatalogsProvider(ApiContext context, IContainer container)
        {
            _context = context;
            _container = container;
        }

        #region Implementation of ICatalogsProvider

        public async Task<IEnumerable<Models.Catalog>> GetCatalogsAsync()
        {
            return await _context.Sources.ToListAsync();
        }

        public async Task<IEnumerable<Manga>> GetLatestCatalogContentAsync(CatalogType catalogType, int page)
        {
            var parser = await GetSourceParser(catalogType);
            var mangas = await parser.GetLatestContent(page);

            return await Task.FromResult(mangas);
        }
        public async Task<IEnumerable<Manga>> GetNewestCatalogContentAsync(CatalogType catalogType, int page)
        {
            var parser = await GetSourceParser(catalogType);
            var mangas = await parser.GetNewestContent(page);

            return await Task.FromResult(mangas);
        }

        #endregion

        private async Task<ICatalogParser> GetSourceParser(CatalogType catalogType)
        {
            ICatalogParser parser = null;

            switch (catalogType)
            {
                case CatalogType.MangaFox:
                    parser = _container.GetInstance<ICatalogParser>(nameof(MangaFoxCatalogParser));
                    break;
                case CatalogType.ReadManga:
                    parser = _container.GetInstance<ICatalogParser>(nameof(ReadMangaCatalogParcer));
                    break;
            }

            return await Task.FromResult(parser);
        }
    }
}