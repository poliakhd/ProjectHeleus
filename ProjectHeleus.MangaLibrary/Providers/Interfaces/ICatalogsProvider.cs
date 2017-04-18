namespace ProjectHeleus.MangaLibrary.Providers.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface ICatalogsProvider
    {
        string Url { get; set; }

        Task<IEnumerable<CatalogModel>> GetAllCatalogs();
        Task<IEnumerable<MangaShortModel>> GetCatalogContent(CatalogModel catalog);
        Task<IEnumerable<MangaShortModel>> GetCatalogContent(CatalogModel catalog, int page);
    }
}