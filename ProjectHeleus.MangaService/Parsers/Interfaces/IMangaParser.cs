namespace ProjectHeleus.MangaService.Parsers.Interfaces
{
    using System.Threading.Tasks;

    using Shared.Models.Interfaces;

    public interface IMangaParser
    {
        Task<IManga> GetMangaAsync(string url);
        Task<IChapterImages> GetMangaChapterAsync(string url);
    }
}