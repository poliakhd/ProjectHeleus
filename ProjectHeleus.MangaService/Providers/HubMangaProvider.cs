using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Core;
using ProjectHeleus.MangaService.Extensions;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Models.Contracts;
using ProjectHeleus.MangaService.Providers.Contracts;
using StructureMap;

namespace ProjectHeleus.MangaService.Providers
{
    public class HubMangaProvider
        : IMangaProvider
    {
        #region Private Members

        private readonly IContainer _container;

        #endregion

        public HubMangaProvider(IContainer container)
        {
            _container = container;
        }

        #region Implementation of IMangaProvider

        public async Task<IManga> GetMangaContentAsync(CatalogType catalogType, string manga)
        {
            var parser = _container.GetParser(catalogType);
            var mangas = await parser.GetMangaContent(manga);

            return mangas;
        }
        public async Task<IChapterContent> GetMangaChapterContentAsync(CatalogType catalogType, string manga)
        {
            var parser = _container.GetParser(catalogType);
            var images = await parser.GetMangaChapterContent(manga);

            return images;
        }

        #endregion
    }
}