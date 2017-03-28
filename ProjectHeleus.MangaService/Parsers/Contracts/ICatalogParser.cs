using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Models;

namespace ProjectHeleus.MangaService.Parsers.Contracts
{
    public interface ICatalogParser
    {
        Task<IEnumerable<Manga>> GetLatestContent();
        Task<IEnumerable<Manga>> GetNewContent();
    }
}