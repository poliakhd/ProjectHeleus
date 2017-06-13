namespace ProjectHeleus.MangaApp.ViewModels
{
    using System;
    using System.Linq;
    using WindowsLibrary.Extenstions;
    using Caliburn.Micro;
    using MangaLibrary.Providers.Interfaces;
    using Shared.Models;

    public class CoverModel
    {
        public string Path { get; set; }
    }

    public class DetailPageViewModel : Screen
    {
        #region Private Members

        private readonly ICatalogsProvider _catalogsProvider;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDetailProvider _detailProvider;

        private Tuple<MangaPreviewModel, CatalogModel> _parameter;
        private MangaModel _mangaModel;

        #endregion

        public Tuple<MangaPreviewModel, CatalogModel> Parameter
        {
            get { return _parameter; }
            set
            {
                _parameter = value;
                GetMangaContent();
            }
        }

        public MangaModel MangaModel
        {
            get { return _mangaModel; }
            set
            {
                _mangaModel = value;
                NotifyOfPropertyChange(nameof(CoverModels));
            }
        }

        public BindableCollection<CoverModel> CoverModels => _mangaModel?.Covers.Select(x=>new CoverModel() {Path = x}).ToBindableCollection();

        public DetailPageViewModel(ICatalogsProvider catalogsProvider, IEventAggregator eventAggregator, IDetailProvider detailProvider)
        {
            _catalogsProvider = catalogsProvider;
            _eventAggregator = eventAggregator;
            _detailProvider = detailProvider;

            Initialize();
        }

        private async void Initialize()
        {
            _eventAggregator.Subscribe(this);
        }

        private async void GetMangaContent()
        {
            MangaModel = await _detailProvider.GetMangaContent(Parameter.Item2, Parameter.Item1);
        }
    }
}