using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Models;

namespace ProjectHeleus.MangaService.Parsers.Contracts
{
    public interface ICatalogParser
    {
        string Url { get; set; }

        Task<IEnumerable<Manga>> GetLatestContent(int page);
        Task<IEnumerable<Manga>> GetNewestContent(int page);
    }
}