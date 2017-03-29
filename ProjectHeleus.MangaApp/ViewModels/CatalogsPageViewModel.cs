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
    public class CatalogsPageViewModel 
        : Screen
    {
        private readonly ICatalogsProvider _catalogsProvider;
        private IncrementalLoadingCollection<GeneratingDataSource, Manga> _mangas;


        public IncrementalLoadingCollection<GeneratingDataSource, Manga> Mangas
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
            Mangas = new IncrementalLoadingCollection<GeneratingDataSource, Manga>(new GeneratingDataSource(_catalogsProvider), 10);
        }
    }

    public class GeneratingDataSource : IIncrementalSource<Manga>
    {
        private readonly ICatalogsProvider _catalogsProvider;

        public GeneratingDataSource(ICatalogsProvider catalogsProvider)
        {
            _catalogsProvider = catalogsProvider;
        }

        #region Implementation of IIncrementalSource<Manga>

        public async Task<IEnumerable<Manga>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = new CancellationToken())
        {
            var catalogs = await _catalogsProvider.GetAllCatalogs();
            return await Task.FromResult(await catalogs.FirstOrDefault(x => x.Id == 2).GetCatalogContent());
        }

        #endregion
    }
}