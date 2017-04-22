using Caliburn.Micro;
using Microsoft.Toolkit.Uwp;

namespace ProjectHeleus.MangaApp.ViewModels
{
    using MangaLibrary.Core.Collections;
    using MangaLibrary.Core.Messages;
    using MangaLibrary.Models;
    using MangaLibrary.Providers.Interfaces;

    public class CatalogsPageViewModel 
        : Screen, IHandle<BeginIncrementalLoading>, IHandle<EndIncrementalLoading>
    {
        #region Private Members

        private readonly ICatalogsProvider _catalogsProvider;
        private readonly IEventAggregator _eventAggregator;
        
        private CatalogModel _catalog;

        private bool _isBusy;
        private bool _isCatalogsBusy;
        private bool _isSourcesPaneOpen;
        private GenreModel _genre;
        private SortModel _sort;

        #endregion

        public CatalogModel Catalog
        {
            get { return _catalog; }
            set
            {
                _catalog = value;

                SetMangaCollection(value);
                SetGenresCollection(value);
                SetSortsCollection(value);

                IsSourcesPaneOpen = false;

                NotifyOfPropertyChange();
            }
        }
        public BindableCollection<CatalogModel> Catalogs { get; set; }

        public IncrementalLoadingCollection<MangaCollection, MangaShortModel> Mangas { get; set; }

        public GenreModel Genre
        {
            get { return _genre; }
            set
            {
                _genre = value;
                


                NotifyOfPropertyChange();
            }
        }

        public BindableCollection<GenreModel> Genres { get; set; }

        public SortModel Sort
        {
            get { return _sort; }
            set
            {
                _sort = value;

                SetMangaSortCollection(_catalog, value);
                NotifyOfPropertyChange();
            }
        }
        public BindableCollection<SortModel> Sorts { get; set; }

        public CatalogsPageViewModel(ICatalogsProvider catalogsProvider, IEventAggregator eventAggregator)
        {
            _catalogsProvider = catalogsProvider;
            _eventAggregator = eventAggregator;

            Initialize();
        }

        private async void Initialize()
        {
            IsCatalogsBusy = true;

            _eventAggregator.Subscribe(this);
            Catalogs = await _catalogsProvider.GetAllCatalogs();

            NotifyOfPropertyChange(nameof(Catalogs));

            IsCatalogsBusy = false;
        }

        private void MangaSources()
        {
            DoCatalogsPaneBehavior();
        }
        private void DoCatalogsPaneBehavior()
        {
            IsSourcesPaneOpen = !IsSourcesPaneOpen;
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange();
            }
        }
        public bool IsCatalogsBusy
        {
            get { return _isCatalogsBusy; }
            set
            {
                _isCatalogsBusy = value;
                NotifyOfPropertyChange();
            }
        }
        public bool IsSourcesPaneOpen
        {
            get { return _isSourcesPaneOpen; }
            set
            {
                _isSourcesPaneOpen = value;
                NotifyOfPropertyChange();
            }
        }

        private void SetMangaCollection(CatalogModel catalog)
        {
            var mangaCollection = IoC.Get<MangaCollection>();
            mangaCollection.SetCatalog(catalog);

            Mangas = new IncrementalLoadingCollection<MangaCollection, MangaShortModel>(mangaCollection);

            NotifyOfPropertyChange(nameof(Mangas));
        }
        private void SetMangaSortCollection(CatalogModel catalog, SortModel sort)
        {
            if (catalog != null)
            {
                var mangaCollection = IoC.Get<MangaCollection>();
                mangaCollection.SetCatalog(catalog);
                mangaCollection.SetSort(sort);

                Mangas = new IncrementalLoadingCollection<MangaCollection, MangaShortModel>(mangaCollection);

                NotifyOfPropertyChange(nameof(Mangas));
            }
        }

        private async void SetGenresCollection(CatalogModel catalog)
        {
            Genres = await _catalogsProvider.GetCatalogGenres(catalog);

            NotifyOfPropertyChange(nameof(Genres));
        }
        private async void SetSortsCollection(CatalogModel catalog)
        {
            Sorts = await _catalogsProvider.GetCatalogSorts(catalog);

            NotifyOfPropertyChange(nameof(Sorts));
        }

        #region Implementation of IHandle<BeginIncrementalLoading>

        public void Handle(BeginIncrementalLoading message)
        {
            IsBusy = true;
        }

        #endregion

        #region Implementation of IHandle<EndIncrementalLoading>

        public void Handle(EndIncrementalLoading message)
        {
            IsBusy = false;
        }

        #endregion
    }
}