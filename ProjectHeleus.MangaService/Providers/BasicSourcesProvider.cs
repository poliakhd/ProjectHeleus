using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectHeleus.MangaService.Core;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Parsers;
using ProjectHeleus.MangaService.Parsers.Contracts;
using ProjectHeleus.MangaService.Parsers.Core;
using ProjectHeleus.MangaService.Providers.Contracts;
using StructureMap;

namespace ProjectHeleus.MangaService.Providers
{
    public class BasicSourcesProvider
        : ISourcesProvider
    {
        #region Private Members

        private readonly ApiContext _context;
        private readonly IContainer _container;

        #endregion
        
        public BasicSourcesProvider(ApiContext context, IContainer container)
        {
            _context = context;
            _container = container;
        }

        #region Implementation of ISourcesProvider

        public async Task<IEnumerable<Models.Source>> GetAllSourcesAsync()
        {
            return await _context.Sources.ToListAsync();
        }

        public async Task<IEnumerable<Manga>> GetLatestSourceContentAsync(SourceType sourceType)
        {
            var parser = await GetSourceParser(sourceType);
            var mangas = await parser.GetLatestContent();

            return await Task.FromResult(mangas);
        }

        #endregion

        private async Task<ISourceParser> GetSourceParser(SourceType sourceType)
        {
            ISourceParser parser = null;

            switch (sourceType)
            {
                case SourceType.MangaFox:
                    parser = _container.GetInstance<ISourceParser>(nameof(MangaFoxSourceParser));
                    break;
                case SourceType.ReadManga:
                    parser = _container.GetInstance<ISourceParser>(nameof(ReadMangaSourceParcer));
                    break;
            }

            return await Task.FromResult(parser);
        }
    }
}