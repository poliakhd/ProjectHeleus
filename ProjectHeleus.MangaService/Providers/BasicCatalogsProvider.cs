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
    public class BasicCatalogsProvider
        : ICatalogsProvider
    {
        #region Private Members

        private readonly ApiContext _context;
        private readonly IContainer _container;

        #endregion
        
        public BasicCatalogsProvider(ApiContext context, IContainer container)
        {
            _context = context;
            _container = container;
        }

        #region Implementation of ICatalogsProvider

        public async Task<IEnumerable<Models.Catalog>> GetAllSourcesAsync()
        {
            return await _context.Sources.ToListAsync();
        }

        public async Task<IEnumerable<Manga>> GetLatestSourceContentAsync(SourceType sourceType)
        {
            var parser = await GetSourceParser(sourceType);
            var mangas = await parser.GetLatestContent();

            return await Task.FromResult(mangas);
        }
        public async Task<IEnumerable<Manga>> GetNewSourceContentAsync(SourceType sourceType)
        {
            var parser = await GetSourceParser(sourceType);
            var mangas = await parser.GetNewContent();

            return await Task.FromResult(mangas);
        }

        #endregion

        private async Task<ICatalogParser> GetSourceParser(SourceType sourceType)
        {
            ICatalogParser parser = null;

            switch (sourceType)
            {
                case SourceType.MangaFox:
                    parser = _container.GetInstance<ICatalogParser>(nameof(MangaFoxCatalogParser));
                    break;
                case SourceType.ReadManga:
                    parser = _container.GetInstance<ICatalogParser>(nameof(ReadMangaCatalogParcer));
                    break;
            }

            return await Task.FromResult(parser);
        }
    }
}