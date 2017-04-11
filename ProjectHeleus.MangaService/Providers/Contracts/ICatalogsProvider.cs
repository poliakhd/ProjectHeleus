namespace ProjectHeleus.MangaService.Providers.Contracts
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Core;
    using Models.Interfaces;

    public interface ICatalogsProvider
    {
        Task<IEnumerable<ICatalog>> GetCatalogsAsync();

        Task<IEnumerable<IManga>> GetAllFromCatalogAsync(CatalogType catalog, SortType sort, int page);
    }
}