using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Models.Mangas;
using ProjectHeleus.MangaService.Parsers.Contracts;

namespace ProjectHeleus.MangaService.Parsers
{
    public class MangaFoxParser 
        : IParser
    {
        public string Url { get; set; }

        #region Implementation of IParser

        public Task<IEnumerable<ListManga>> GetUpdateContent(int page)
        {
            throw new System.NotImplementedException();
        }
        public Task<IEnumerable<ListManga>> GetNewContent(int page)
        {
            throw new System.NotImplementedException();
        }

        public Task<Manga> GetMangaContent(string mangaId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ListManga>> GetRatingContent(int page)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<string>> GetMangaChapterContent(string w)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}