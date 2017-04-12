namespace ProjectHeleus.MangaService.Providers
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Core;
    using Extensions;
    using Interfaces;
    using Models.Interfaces;
    using StructureMap;

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

        public Task<IEnumerable<IManga>> GetAllFromGenreAsync(CatalogType catalogType, string url)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}