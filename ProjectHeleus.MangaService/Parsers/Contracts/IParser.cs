using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Core;
using ProjectHeleus.MangaService.Models.Contracts;

namespace ProjectHeleus.MangaService.Parsers.Contracts
{
    public interface IParser
    {
        string Url { get; set; }

        Task<IEnumerable<IManga>> GetCatalogContent(SortType sortType, int page);
            
        //Task<IEnumerable<IManga>> GetUpdateContent(int page);
        //Task<IEnumerable<IManga>> GetNewContent(int page);
        //Task<IEnumerable<IManga>> GetRatingContent(int page);
        //Task<IEnumerable<IManga>> GetPopularContent(int page);

        Task<IManga> GetMangaContent(string url);
        Task<IChapterContent> GetMangaChapterContent(string url);

        Task<IEnumerable<IGenre>> GetGenres();
    }
}