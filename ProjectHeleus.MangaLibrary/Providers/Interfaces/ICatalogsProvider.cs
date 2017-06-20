namespace ProjectHeleus.MangaLibrary.Providers.Interfaces
{
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using Shared.Models;

    public interface ICatalogsProvider : IBaseProvider
    {
        Task<ProviderRespose<BindableCollection<CatalogModel>>> GetAllCatalogs();

        Task<ProviderRespose<BindableCollection<MangaPreviewModel>>> GetCatalogContent(CatalogModel catalog);
        Task<ProviderRespose<BindableCollection<MangaPreviewModel>>> GetCatalogContent(CatalogModel catalog, int page);
        Task<ProviderRespose<BindableCollection<MangaPreviewModel>>> GetCatalogContent(CatalogModel catalog, SortModel sort, int page);

        Task<ProviderRespose<BindableCollection<MangaPreviewModel>>> GetCatalogContent(CatalogModel catalog, GenreModel genre, int page);
        Task<ProviderRespose<BindableCollection<MangaPreviewModel>>> GetCatalogContent(CatalogModel catalog, GenreModel genre, SortModel sort, int page);

        Task<ProviderRespose<BindableCollection<SortModel>>> GetCatalogSorts(CatalogModel catalog);
        Task<ProviderRespose<BindableCollection<GenreModel>>> GetCatalogGenres(CatalogModel catalog);
    }
}