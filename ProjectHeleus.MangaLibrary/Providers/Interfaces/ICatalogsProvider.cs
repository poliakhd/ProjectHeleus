namespace ProjectHeleus.MangaLibrary.Providers.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Models;

    public interface ICatalogsProvider
    {
        string Url { get; set; }

        Task<BindableCollection<CatalogModel>> GetAllCatalogs();

        Task<BindableCollection<MangaShortModel>> GetCatalogContent(CatalogModel catalog);
        Task<BindableCollection<MangaShortModel>> GetCatalogContent(CatalogModel catalog, int page);
    }
}