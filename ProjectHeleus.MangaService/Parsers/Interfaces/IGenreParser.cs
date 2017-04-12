namespace ProjectHeleus.MangaService.Parsers.Interfaces
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Core;
    using Models.Interfaces;

    public interface IGenreParser
    {
        Task<IEnumerable<IGenre>> GetAllGenresAsync();
        Task<IEnumerable<IManga>> GetAllFromGenreGenreAsync(SortType sortType, string url, int page);
    }
}