using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Models.Contracts;
using ProjectHeleus.MangaService.Parsers.Contracts;

namespace ProjectHeleus.MangaService.Parsers.Core
{
    public class DefaultParser : IParser
    {
        #region Implementation of IParser

        public virtual string Url { get; set; }

        public virtual Task<IEnumerable<IManga>> GetUpdateContent(int page)
        {
            return null;
        }

        public virtual Task<IEnumerable<IManga>> GetNewContent(int page)
        {
            return null;
        }

        public virtual Task<IEnumerable<IManga>> GetRatingContent(int page)
        {
            return null;
        }

        public virtual Task<IManga> GetMangaContent(string mangaId)
        {
            return null;
        }

        public virtual Task<IEnumerable<string>> GetMangaChapterContent(string manga)
        {
            return null;
        }
        
        public virtual Task<IEnumerable<IManga>> GetPopularContent(int page)
        {
            throw new System.NotImplementedException();
        }
        
        #endregion
    }
}