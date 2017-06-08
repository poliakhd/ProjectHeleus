namespace ProjectHeleus.MangaLibrary.Core.Collections
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Caliburn.Micro;
    using Microsoft.Toolkit.Uwp;

    using Messages;
    using Shared.Models;
    using Providers.Interfaces;

    public class MangaCollection 
        : IIncrementalSource<MangaShortModel>
    {
        #region Private Members

        private readonly ICatalogsProvider _catalogsProvider;
        private readonly IEventAggregator _eventAggregator;

        private CatalogModel _catalog;
        private SortModel _sort;
        private GenreModel _genre;

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

        public void SetSort(SortModel sort)
        {
            _sort = sort;
        }

        public void SetGenre(GenreModel genre)
        {
            _genre = genre;
        }

        #region Implementation of IIncrementalSource<MangaModel>

        public async Task<IEnumerable<MangaShortModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = new CancellationToken())
        {
            _eventAggregator.PublishOnUIThread(new BeginIncrementalLoading());

            IEnumerable<MangaShortModel> fetchedResult = null;

            if (_genre is null)
            {
                if (_sort is null)
                    fetchedResult = await _catalogsProvider.GetCatalogContent(_catalog, pageIndex);
                else
                    fetchedResult = await _catalogsProvider.GetCatalogContent(_catalog, _sort, pageIndex);
            }
            else
            {
                if (_sort is null)
                    fetchedResult = await _catalogsProvider.GetCatalogContent(_catalog, _genre, pageIndex);
                else
                    fetchedResult = await _catalogsProvider.GetCatalogContent(_catalog, _genre, _sort, pageIndex);
            }

            _eventAggregator.PublishOnUIThread(new EndIncrementalLoading());

            return fetchedResult;
        }

        #endregion
    }
}