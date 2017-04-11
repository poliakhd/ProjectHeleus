namespace ProjectHeleus.MangaService.Providers.Contracts
{
    using System.Threading.Tasks;

    using Core;
    using Models.Interfaces;

    public interface IMangaProvider
    {
        Task<IManga> GetMangaContentAsync(CatalogType catalogType, string manga);
        Task<IChapterImages> GetMangaChapterContentAsync(CatalogType catalogType, string manga);
    }
}