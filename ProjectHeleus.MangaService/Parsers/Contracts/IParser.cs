using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Models.Mangas;

namespace ProjectHeleus.MangaService.Parsers.Contracts
{
    public interface IParser
    {
        string Url { get; set; }

        Task<IEnumerable<ListManga>> GetUpdateContent(int page);
        Task<IEnumerable<ListManga>> GetNewContent(int page);
        Task<IEnumerable<ListManga>> GetRatingContent(int page);

        Task<Manga> GetMangaContent(string mangaId);
        Task<IEnumerable<string>> GetMangaChapterContent(string manga);
    }
}