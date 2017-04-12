using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Toolkit.Uwp;
using ProjectHeleus.MangaApp.Models;
using ProjectHeleus.MangaApp.Providers;
using ProjectHeleus.MangaApp.Providers.Contracts;

namespace ProjectHeleus.MangaApp.ViewModels
{
    using System;

    public class CatalogsPageViewModel 
        : Screen
    {
        private readonly ICatalogsProvider _catalogsProvider;
        private IncrementalLoadingCollection<GeneratingDataSource, MangaShortModel> _mangas;


        public IncrementalLoadingCollection<GeneratingDataSource, MangaShortModel> Mangas
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
            Mangas = new IncrementalLoadingCollection<GeneratingDataSource, MangaShortModel>(new GeneratingDataSource(_catalogsProvider), 70);
        }
    }

    public class GeneratingDataSource : IIncrementalSource<MangaShortModel>
    {
        private readonly ICatalogsProvider _catalogsProvider;

        public GeneratingDataSource(ICatalogsProvider catalogsProvider)
        {
            _catalogsProvider = catalogsProvider;
        }

        #region Implementation of IIncrementalSource<Manga>

        public async Task<IEnumerable<MangaShortModel>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = new CancellationToken())
        {            
            var catalogs = await _catalogsProvider.GetAllCatalogs();
            var catalog = catalogs.FirstOrDefault(x => x.Id == "readmanga.me");

            return await Task.FromResult(await _catalogsProvider.GetCatalogContent(catalog, pageIndex));
        }

        #endregion
    }
}