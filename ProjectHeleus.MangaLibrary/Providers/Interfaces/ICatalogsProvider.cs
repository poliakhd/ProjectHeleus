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
        Task<BindableCollection<MangaShortModel>> GetCatalogContent(CatalogModel catalog, SortModel sort, int page);

        Task<BindableCollection<GenreModel>> GetCatalogGenres(CatalogModel catalog);
        Task<BindableCollection<SortModel>> GetCatalogSorts(CatalogModel catalog);
    }
}