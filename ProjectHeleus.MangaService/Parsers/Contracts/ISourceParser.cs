using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Models;

namespace ProjectHeleus.MangaService.Parsers.Contracts
{
    public interface ISourceParser
    {
        Task<IEnumerable<Manga>> GetLatestContent();
    }
}