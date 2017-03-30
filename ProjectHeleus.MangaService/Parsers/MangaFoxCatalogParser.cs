using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Parsers.Contracts;

namespace ProjectHeleus.MangaService.Parsers
{
    public class MangaFoxCatalogParser 
        : ICatalogParser
    {
        public string Url { get; set; }

        public Task<IEnumerable<Manga>> GetLatestContent(int page)
        {
            throw new System.NotImplementedException();
        }
        public Task<IEnumerable<Manga>> GetNewestContent(int page)
        {
            throw new System.NotImplementedException();
        }
    }
}