using Caliburn.Micro;
using Microsoft.Toolkit.Uwp;

namespace ProjectHeleus.MangaApp.ViewModels
{
    using System;
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

        private bool _isMangasBusy;
        private bool _isCatalogsBusy;
        private bool _isSourcesPaneOpen;
        private GenreModel _genre;
        private SortModel _sort;
        private bool _isSortBusy;
        private bool _isGenresBusy;

        #endregion

        public CatalogModel Catalog
        {
            get { return _catalog; }
            set
            {
                if(value is null)
                    return;

                _catalog = value;

                SetMangaCollection(value, null, null);
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
                
                SetMangaCollection(_catalog, _sort, value);
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

                SetMangaCollection(_catalog, value, _genre);
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

        public bool IsMangasBusy
        {
            get { return _isMangasBusy; }
            set
            {
                _isMangasBusy = value;
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
        public bool IsSortBusy
        {
            get { return _isSortBusy; }
            set
            {
                _isSortBusy = value;
                NotifyOfPropertyChange();
            }
        }
        public bool IsGenresBusy
        {
            get { return _isGenresBusy; }
            set
            {
                _isGenresBusy = value;
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

        private void SetMangaCollection(CatalogModel catalog, SortModel sort, GenreModel genre)
        {
            var mangaCollection = IoC.Get<MangaCollection>();
            mangaCollection.SetCatalog(catalog);
            mangaCollection.SetSort(sort);
            mangaCollection.SetGenre(genre);

            Mangas = new IncrementalLoadingCollection<MangaCollection, MangaShortModel>(mangaCollection);

            NotifyOfPropertyChange(nameof(Mangas));
        }
        private async void SetGenresCollection(CatalogModel catalog)
        {
            IsGenresBusy = true;

            Genres = await _catalogsProvider.GetCatalogGenres(catalog);

            NotifyOfPropertyChange(nameof(Genres));

            IsGenresBusy = false;
        }
        private async void SetSortsCollection(CatalogModel catalog)
        {
            IsSortBusy = true;

            Sorts = await _catalogsProvider.GetCatalogSorts(catalog);

            NotifyOfPropertyChange(nameof(Sorts));

            IsSortBusy = false;
        }

        #region Overrides of Screen

        protected override void OnDeactivate(bool close)
        {
            Mangas?.Clear();
            Catalogs?.Clear();
            Genres?.Clear();
            Sorts?.Clear();

            _catalog = null;
            _genre = null;
            _sort = null;

            Mangas = null;
            Catalogs = null;
            Genres = null;
            Sorts = null;

            GC.WaitForPendingFinalizers();
            GC.Collect();

            base.OnDeactivate(close);
        }

        #endregion

        #region Implementation of IHandle<BeginIncrementalLoading>

        public void Handle(BeginIncrementalLoading message)
        {
            IsMangasBusy = true;
        }

        #endregion

        #region Implementation of IHandle<EndIncrementalLoading>

        public void Handle(EndIncrementalLoading message)
        {
            IsMangasBusy = false;
        }

        #endregion
    }
}