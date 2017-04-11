using ProjectHeleus.MangaService.Models.Interfaces;

namespace ProjectHeleus.MangaService.Parsers.Interfaces
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Core;

    public interface ICatalogParser
    {
        Task<IEnumerable<IManga>> GetAllFromCatalogAsync(SortType sortType, int page);
    }
}