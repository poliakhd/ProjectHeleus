using ProjectHeleus.MangaService.Models.Interfaces;

namespace ProjectHeleus.MangaService.Parsers.Interfaces
{
    using System.Threading.Tasks;

    public interface IMangaParser
    {
        Task<IManga> GetMangaAsync(string url);
        Task<IChapterImages> GetMangaChapterAsync(string url);
    }
}