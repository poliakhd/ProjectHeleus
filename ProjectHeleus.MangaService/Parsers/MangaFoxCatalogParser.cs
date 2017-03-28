using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Parsers.Contracts;

namespace ProjectHeleus.MangaService.Parsers
{
    public class MangaFoxCatalogParser 
        : ICatalogParser
    {
        public Task<IEnumerable<Manga>> GetLatestContent()
        {
            throw new System.NotImplementedException();
        }
        public Task<IEnumerable<Manga>> GetNewContent()
        {
            throw new System.NotImplementedException();
        }
    }
}