using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Extensions;
using ProjectHeleus.MangaService.Models.Mangas;
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

        public async Task<Manga> GetMangaContentAsync(CatalogType catalogType, string relativeUrl)
        {
            var parser = catalogType.GetParser(_container);
            var mangas = await parser.GetMangaContent(relativeUrl);

            return await Task.FromResult(mangas);
        }

        public async Task<IEnumerable<string>> GetMangaChapterContentAsync(CatalogType catalogType, string relativeUrl)
        {
            var parser = catalogType.GetParser(_container);
            var images = await parser.GetMangaChapterContent(relativeUrl);

            return await Task.FromResult(images);
        }

        #endregion
    }
}