namespace ProjectHeleus.MangaService.Providers
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using StructureMap;

    using Extensions;
    using Interfaces;
    using Shared.Types;
    using Shared.Models.Interfaces;

    public class HubGenresProvider : IGenresProvider
    {
        #region Private Members

        private readonly IContainer _container;

        #endregion

        public HubGenresProvider(IContainer container)
        {
            _container = container;
        }

        #region Implementation of IGenresProvider

        public async Task<IEnumerable<IGenre>> GetAllGenresAsync(CatalogType catalogType)
        {
            var parser = _container.GetParser(catalogType);
            return await parser.GetAllGenresAsync();
        }

        public async Task<IEnumerable<IManga>> GetAllFromGenreAsync(CatalogType catalogType, SortType sortType, string url, int page)
        {
            var parser = _container.GetParser(catalogType);
            return await parser.GetAllFromGenreGenreAsync(sortType, url, page);
        }

        #endregion
    }
}