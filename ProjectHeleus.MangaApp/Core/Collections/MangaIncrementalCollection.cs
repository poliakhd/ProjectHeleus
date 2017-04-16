namespace ProjectHeleus.MangaApp.Core.Collections
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Toolkit.Uwp;
    using Models;
    using Providers.Contracts;

    public class MangaIncrementalCollection : IIncrementalSource<MangaShortModel>
    {
        #region Private Members

        private readonly CatalogModel _catalog;
        private readonly ICatalogsProvider _catalogsProvider;

        #endregion

        public MangaIncrementalCollection(CatalogModel catalog, ICatalogsProvider catalogsProvider)
        {
            _catalog = catalog;
            _catalogsProvider = catalogsProvider;
        }

        #region Implementation of IIncrementalSource<MangaModel>

        public async Task<IEnumerable<MangaShortModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _catalogsProvider.GetCatalogContent(_catalog, pageIndex);
        }

        #endregion
    }
}