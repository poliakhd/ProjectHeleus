using Caliburn.Micro;
using Microsoft.Toolkit.Uwp;

namespace ProjectHeleus.MangaApp.ViewModels
{
    using System;
    using Windows.UI.Xaml;
    using MangaLibrary.Core.Collections;
    using MangaLibrary.Core.Messages;
    using MangaLibrary.Models;
    using MangaLibrary.Providers.Interfaces;
    using SharedLibrary.Extenstions;

    public class CatalogsPageViewModel 
        : Screen, IHandle<BeginIncrementalLoading>, IHandle<EndIncrementalLoading>
    {
        #region Private Members

        private readonly ICatalogsProvider _catalogsProvider;
        private readonly IEventAggregator _eventAggregator;
        private BindableCollection<CatalogModel> _catalogs;
        private bool _isSourcesPaneOpen;
        private CatalogModel _catalog;
        private IncrementalLoadingCollection<MangaCollection, MangaShortModel> _mangas;
        private bool _isBusy;

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

                var w = IoC.Get<MangaCollection>();
                w.SetCatalog(value);
                _mangas = new IncrementalLoadingCollection<MangaCollection, MangaShortModel>(w);

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
        public IncrementalLoadingCollection<MangaCollection, MangaShortModel> Mangas
        {
            get { return _mangas; }
            set
            {
                _mangas = value; 
                NotifyOfPropertyChange();
            }
        }

        public CatalogsPageViewModel(ICatalogsProvider catalogsProvider, IEventAggregator eventAggregator)
        {
            _catalogsProvider = catalogsProvider;
            _eventAggregator = eventAggregator;

            Initialize();
        }

        private async void Initialize()
        {
            _eventAggregator.Subscribe(this);
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

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange();
            }
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

        private void SizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            //
        }
    }
}