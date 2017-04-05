using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Models.Mangas;
using ProjectHeleus.MangaService.Parsers.Contracts;

namespace ProjectHeleus.MangaService.Parsers.Core
{
    public class DefaultParser : IParser
    {
        #region Implementation of IParser

        public virtual string Url { get; set; }

        public virtual Task<IEnumerable<ListManga>> GetUpdateContent(int page)
        {
            return null;
        }

        public virtual Task<IEnumerable<ListManga>> GetNewContent(int page)
        {
            return null;
        }

        public virtual Task<IEnumerable<ListManga>> GetRatingContent(int page)
        {
            return null;
        }

        public virtual Task<Manga> GetMangaContent(string mangaId)
        {
            return null;
        }

        public virtual Task<IEnumerable<string>> GetMangaChapterContent(string manga)
        {
            return null;
        }
        
        public virtual Task<IEnumerable<ListManga>> GetPopularContent(int page)
        {
            throw new System.NotImplementedException();
        }
        
        #endregion
    }
}