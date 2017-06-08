namespace ProjectHeleus.MangaService.Providers.Interfaces
{
    using System.Threading.Tasks;

    using Shared.Types;
    using Shared.Models.Interfaces;
    
    public interface IMangasProvider
    {
        Task<IManga> GetMangaContentAsync(CatalogType catalogType, string url);
        Task<IChapterImages> GetMangaChapterContentAsync(CatalogType catalogType, string url);
    }
}