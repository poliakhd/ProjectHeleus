using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Models.Mangas;

namespace ProjectHeleus.MangaService.Providers.Contracts
{
    public interface IMangaProvider
    {
        Task<Manga> GetMangaContentAsync(CatalogType catalogType, string mangaId);
        Task<IEnumerable<string>> GetMangaChapterContentAsync(CatalogType catalogType, string mangaId);
    }
}