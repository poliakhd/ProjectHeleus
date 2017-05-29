
namespace ProjectHeleus.MangaService.Parsers.Interfaces
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Core;
    using Models.Interfaces;

    public interface ICatalogParser
    {
        Task<IEnumerable<ISort>> GetCatalogSorts();
        Task<IEnumerable<IManga>> GetAllFromCatalogAsync(SortType sortType, int page);
    }
}