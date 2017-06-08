namespace ProjectHeleus.MangaService.Providers.Interfaces
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Shared.Types;
    using Shared.Models.Interfaces;

    public interface IGenresProvider
    {
        Task<IEnumerable<IGenre>> GetAllGenresAsync(CatalogType catalogType);
        Task<IEnumerable<IManga>> GetAllFromGenreAsync(CatalogType catalogType, SortType sortType, string url, int page);
    }
}