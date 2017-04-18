namespace ProjectHeleus.MangaLibrary.Core.Collections
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Messages;
    using Microsoft.Toolkit.Uwp;
    using Models;
    using Providers.Interfaces;

    public class MangaCollection 
        : IIncrementalSource<MangaShortModel>
    {
        #region Private Members

        private CatalogModel _catalog;
        private readonly ICatalogsProvider _catalogsProvider;
        private readonly IEventAggregator _eventAggregator;

        #endregion

        public MangaCollection(ICatalogsProvider catalogsProvider, IEventAggregator eventAggregator)
        {
            _catalogsProvider = catalogsProvider;
            _eventAggregator = eventAggregator;
        }

        public void SetCatalog(CatalogModel catalog)
        {
            _catalog = catalog;
        }

        #region Implementation of IIncrementalSource<MangaModel>

        public async Task<IEnumerable<MangaShortModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = new CancellationToken())
        {
            _eventAggregator.PublishOnUIThread(new BeginIncrementalLoading());
            var fetchedResult = await _catalogsProvider.GetCatalogContent(_catalog, pageIndex);
            _eventAggregator.PublishOnUIThread(new EndIncrementalLoading());

            return fetchedResult;
        }

        #endregion
    }
}