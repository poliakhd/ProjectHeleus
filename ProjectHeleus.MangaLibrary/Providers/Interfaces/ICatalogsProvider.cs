namespace ProjectHeleus.MangaLibrary.Providers.Interfaces
{
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using Shared.Models;

    public interface ICatalogsProvider
    {
        string Url { get; set; }

        Task<BindableCollection<CatalogModel>> GetAllCatalogs();

        Task<BindableCollection<MangaShortModel>> GetCatalogContent(CatalogModel catalog);
        Task<BindableCollection<MangaShortModel>> GetCatalogContent(CatalogModel catalog, int page);
        Task<BindableCollection<MangaShortModel>> GetCatalogContent(CatalogModel catalog, SortModel sort, int page);

        Task<BindableCollection<MangaShortModel>> GetCatalogContent(CatalogModel catalog, GenreModel genre, int page);
        Task<BindableCollection<MangaShortModel>> GetCatalogContent(CatalogModel catalog, GenreModel genre, SortModel sort, int page);

        Task<BindableCollection<SortModel>> GetCatalogSorts(CatalogModel catalog);
        Task<BindableCollection<GenreModel>> GetCatalogGenres(CatalogModel catalog);
    }
}