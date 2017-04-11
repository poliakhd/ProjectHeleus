using ProjectHeleus.MangaService.Models.Interfaces;

namespace ProjectHeleus.MangaService.Parsers.Interfaces
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IGenreParser
    {
        Task<IEnumerable<IGenre>> GetAllGenresAsync();
    }
}