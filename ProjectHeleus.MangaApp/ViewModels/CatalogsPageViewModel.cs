using Caliburn.Micro;
using Microsoft.Toolkit.Uwp;
using ProjectHeleus.MangaApp.Models;
using ProjectHeleus.MangaApp.Providers.Contracts;

namespace ProjectHeleus.MangaApp.ViewModels
{
    using Core.Collections;
    using SharedLibrary.Extenstions;

    public class CatalogsPageViewModel 
        : Screen
    {
        #region Private Members

        private readonly ICatalogsProvider _catalogsProvider;
        private BindableCollection<CatalogModel> _catalogs;
        private bool _isSourcesPaneOpen;
        private CatalogModel _catalog;
        private IncrementalLoadingCollection<MangaIncrementalCollection, MangaShortModel> _mangas;

        #endregion

        public bool IsSourcesPaneOpen
        {
            get { return _isSourcesPaneOpen; }
            set
            {
                _isSourcesPaneOpen = value;
                NotifyOfPropertyChange();
            }
        }

        public CatalogModel Catalog
        {
            get { return _catalog; }
            set
            {
                _catalog = value;
                _mangas = new IncrementalLoadingCollection<MangaIncrementalCollection, MangaShortModel>(new MangaIncrementalCollection(value, _catalogsProvider));

                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(Mangas));

                DoCatalogsPaneBehavior();
            }
        }

        public BindableCollection<CatalogModel> Catalogs
        {
            get { return _catalogs; }
            set
            {
                _catalogs = value;
                NotifyOfPropertyChange();
            }
        }
        public IncrementalLoadingCollection<MangaIncrementalCollection, MangaShortModel> Mangas
        {
            get { return _mangas; }
            set
            {
                _mangas = value; 
                NotifyOfPropertyChange();
            }
        }

        public CatalogsPageViewModel(ICatalogsProvider catalogsProvider)
        {
            _catalogsProvider = catalogsProvider;
            Initialize();
        }

        private async void Initialize()
        {
            Catalogs = (await _catalogsProvider.GetAllCatalogs()).ToBindableCollection();
        }

        private void MangaSources()
        {
            DoCatalogsPaneBehavior();
        }

        private void DoCatalogsPaneBehavior()
        {
            IsSourcesPaneOpen = !IsSourcesPaneOpen;
        }
    }
}