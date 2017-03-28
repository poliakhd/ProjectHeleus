using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Parsers.Contracts;

namespace ProjectHeleus.MangaService.Parsers
{
    public class MangaFoxSourceParser 
        : ISourceParser
    {
        public Task<IEnumerable<Manga>> GetLatestContent()
        {
            throw new System.NotImplementedException();
        }
    }
}