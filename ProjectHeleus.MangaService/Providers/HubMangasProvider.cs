namespace ProjectHeleus.MangaService.Providers
{
    using System.Threading.Tasks;

    using StructureMap;

    using Extensions;
    using Interfaces;
    using Shared.Types;
    using Shared.Models.Interfaces;

    public class HubMangasProvider
        : IMangasProvider
    {
        #region Private Members

        private readonly IContainer _container;

        #endregion

        public HubMangasProvider(IContainer container)
        {
            _container = container;
        }

        #region Implementation of IMangaProvider

        public async Task<IManga> GetMangaContentAsync(CatalogType catalogType, string url)
        {
            var parser = _container.GetParser(catalogType);
            var mangas = await parser.GetMangaAsync(url);

            return mangas;
        }
        public async Task<IChapterImages> GetMangaChapterContentAsync(CatalogType catalogType, string url)
        {
            var parser = _container.GetParser(catalogType);
            var images = await parser.GetMangaChapterAsync(url);

            return images;
        }

        #endregion
    }
}