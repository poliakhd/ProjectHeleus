namespace ProjectHeleus.MangaApp.ViewModels
{
    using System;

    using Caliburn.Micro;
    using Microsoft.Toolkit.Uwp;

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
        private bool _isCatalogsInPaneVisible;
        private bool _isSortsInPaneVisible;
        private bool _isGenresInPaneVisible;
        private bool _isNestedPaneOpen;

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
            _eventAggregator.Subscribe(this);
            Catalogs = await _catalogsProvider.GetAllCatalogs();

            NotifyOfPropertyChange(nameof(Catalogs));
        }

        public bool IsNestedPaneOpen
        {
            get { return _isNestedPaneOpen; }
            set
            {
                _isNestedPaneOpen = value; 
                NotifyOfPropertyChange();
            }
        }

        public bool IsCatalogsInPaneVisible
        {
            get { return _isCatalogsInPaneVisible; }
            set
            {
                _isCatalogsInPaneVisible = value; 
                NotifyOfPropertyChange();
            }
        }
        public bool IsSortsInPaneVisible
        {
            get { return _isSortsInPaneVisible; }
            set
            {
                _isSortsInPaneVisible = value;
                NotifyOfPropertyChange();
            }
        }
        public bool IsGenresInPaneVisible
        {
            get { return _isGenresInPaneVisible; }
            set
            {
                _isGenresInPaneVisible = value;
                NotifyOfPropertyChange();
            }
        }

        public void ShowCatalogs()
        {
            IsSortsInPaneVisible = IsGenresInPaneVisible = false;
            IsNestedPaneOpen = IsCatalogsInPaneVisible = true;
        }
        public void ShowSorts()
        {
            IsCatalogsInPaneVisible = IsGenresInPaneVisible = false;
            IsNestedPaneOpen = IsSortsInPaneVisible = true;
        }
        public void ShowGenres()
        {
            IsSortsInPaneVisible = IsCatalogsInPaneVisible = false;
            IsNestedPaneOpen = IsGenresInPaneVisible = true;
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
            Genres = await _catalogsProvider.GetCatalogGenres(catalog);

            NotifyOfPropertyChange(nameof(Genres));
        }
        private async void SetSortsCollection(CatalogModel catalog)
        {
            Sorts = await _catalogsProvider.GetCatalogSorts(catalog);

            NotifyOfPropertyChange(nameof(Sorts));
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
        }

        #endregion

        #region Implementation of IHandle<EndIncrementalLoading>

        public void Handle(EndIncrementalLoading message)
        {
        }

        #endregion
    }
}