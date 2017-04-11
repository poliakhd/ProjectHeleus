namespace ProjectHeleus.MangaService.Providers
{
    using System.Threading.Tasks;

    using StructureMap;

    using Core;
    using Extensions;
    using Models.Interfaces;
    using Contracts;

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
            var mangas = await parser.GetMangaAsync(manga);

            return mangas;
        }
        public async Task<IChapterImages> GetMangaChapterContentAsync(CatalogType catalogType, string manga)
        {
            var parser = _container.GetParser(catalogType);
            var images = await parser.GetMangaChapterAsync(manga);

            return images;
        }

        #endregion
    }
}