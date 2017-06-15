namespace ProjectHeleus.MangaApp.ViewModels
{
    using System;
    using WindowsLibrary.Extenstions;
    using Caliburn.Micro;
    using Microsoft.Toolkit.Uwp;

    using Shared.Models;
    using MangaLibrary.Core.Collections;
    using MangaLibrary.Core.Messages;
    using MangaLibrary.Providers.Interfaces;
    using Microsoft.Toolkit.Uwp.UI.Controls;

    public class CatalogsPageViewModel 
        : Screen, IHandle<BeginIncrementalLoading>, IHandle<EndIncrementalLoading>
    {
        #region Private Members

        private readonly ICatalogsProvider _catalogsProvider;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;

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
        private bool _isCatalogsLoading;
        private BindableCollection<CatalogModel> _catalogs;
        private BindableCollection<GenreModel> _genres;
        private bool _isSortsLoading;
        private bool _isGenresLoading;
        private BindableCollection<SortModel> _sorts;
        private bool _isMangasLoading;
        private MangaPreviewModel _selectedManga;
        private AdaptiveGridView _grid;

        #endregion

        public bool IsNestedPaneOpen
        {
            get { return _isNestedPaneOpen; }
            set
            {
                _isNestedPaneOpen = value;
                NotifyOfPropertyChange();
            }
        }

        public CatalogsPageViewModel(ICatalogsProvider catalogsProvider, IEventAggregator eventAggregator, INavigationService navigationService)
        {
            _catalogsProvider = catalogsProvider;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;

            Initialize();
        }

        private async void Initialize()
        {
            _eventAggregator.Subscribe(this);

            IsCatalogsLoading = true;
            Catalogs = await _catalogsProvider.GetAllCatalogs();
        }

        #region Mangas

        public IncrementalLoadingCollection<MangaCollection, MangaPreviewModel> Mangas { get; set; }

        public MangaPreviewModel SelectedManga
        {
            get { return _selectedManga; }
            set
            {
                _selectedManga = value;

                if(value != null)
                    _navigationService.NavigateToViewModel<DetailPageViewModel>(new Tuple<MangaPreviewModel, CatalogModel>(value, _catalog));
            }
        }

        public bool IsMangasLoading
        {
            get { return _isMangasLoading; }
            set
            {
                _isMangasLoading = value;
                NotifyOfPropertyChange();
            }
        }

        private void SetMangaCollection(CatalogModel catalog, SortModel sort, GenreModel genre)
        {
            var mangaCollection = IoC.Get<MangaCollection>();

            mangaCollection.SetCatalog(catalog);
            mangaCollection.SetSort(sort);
            mangaCollection.SetGenre(genre);

            Mangas = new IncrementalLoadingCollection<MangaCollection, MangaPreviewModel>(mangaCollection);

            NotifyOfPropertyChange(nameof(Mangas));
        }

        private async void MangaArticlesLoaded(object obj)
        {
            _grid = obj as AdaptiveGridView;

            if (_selectedManga != null)
                await _grid.ScrollToItemAsync(_selectedManga);
        }

        #endregion

        #region Catalogs

        public CatalogModel Catalog
        {
            get { return _catalog; }
            set
            {
                if (value is null)
                    return;

                _catalog = value;

                SetMangaCollection(value, null, null);
                SetGenresCollection(value);
                SetSortsCollection(value);

                NotifyOfPropertyChange();
            }
        }
        public BindableCollection<CatalogModel> Catalogs
        {
            get { return _catalogs; }
            set
            {
                IsCatalogsLoading = false;

                _catalogs = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsCatalogsLoading
        {
            get { return _isCatalogsLoading; }
            set
            {
                _isCatalogsLoading = value;
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

        public void ShowCatalogs()
        {
            IsSortsInPaneVisible = IsGenresInPaneVisible = false;
            IsNestedPaneOpen = IsCatalogsInPaneVisible = true;
        }

        #endregion

        #region Sorts


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
        public BindableCollection<SortModel> Sorts
        {
            get { return _sorts; }
            set
            {
                IsSortsLoading = false;

                _sorts = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsSortsLoading
        {
            get { return _isSortsLoading; }
            set
            {
                _isSortsLoading = value;
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

        public void ShowSorts()
        {
            IsCatalogsInPaneVisible = IsGenresInPaneVisible = false;
            IsNestedPaneOpen = IsSortsInPaneVisible = true;
        }
        private async void SetSortsCollection(CatalogModel catalog)
        {
            IsSortsLoading = true;
            Sorts = await _catalogsProvider.GetCatalogSorts(catalog);
        }

        #endregion

        #region Genres

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
        public BindableCollection<GenreModel> Genres
        {
            get { return _genres; }
            set
            {
                IsGenresLoading = false;

                _genres = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsGenresLoading
        {
            get { return _isGenresLoading; }
            set
            {
                _isGenresLoading = value;
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

        public void ShowGenres()
        {
            IsSortsInPaneVisible = IsCatalogsInPaneVisible = false;
            IsNestedPaneOpen = IsGenresInPaneVisible = true;
        }
        private async void SetGenresCollection(CatalogModel catalog)
        {
            IsGenresLoading = true;
            Genres = await _catalogsProvider.GetCatalogGenres(catalog);
        }

        #endregion


        #region Overrides of Screen

        protected override void OnActivate()
        {
            _isNestedPaneOpen = false;

            base.OnActivate();
        }

        #endregion

        #region Implementation of IHandle<BeginIncrementalLoading>

        public void Handle(BeginIncrementalLoading message)
        {
            IsMangasLoading = true;
        }

        #endregion

        #region Implementation of IHandle<EndIncrementalLoading>

        public void Handle(EndIncrementalLoading message)
        {
            IsMangasLoading = false;
        }

        #endregion
    }
}