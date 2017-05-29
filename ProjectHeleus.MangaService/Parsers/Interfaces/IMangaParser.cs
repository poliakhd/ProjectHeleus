namespace ProjectHeleus.MangaService.Parsers.Interfaces
{
    using System.Threading.Tasks;

    using Models.Interfaces;

    public interface IMangaParser
    {
        Task<IManga> GetMangaAsync(string url);
        Task<IChapterImages> GetMangaChapterAsync(string url);
    }
}