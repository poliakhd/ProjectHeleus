using System.Threading.Tasks;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Models.Contracts;

namespace ProjectHeleus.MangaService.Providers.Contracts
{
    public interface IMangaProvider
    {
        Task<IManga> GetMangaContentAsync(CatalogType catalogType, string manga);
        Task<IChapterContent> GetMangaChapterContentAsync(CatalogType catalogType, string manga);
    }
}