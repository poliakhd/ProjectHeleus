using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Models.Contracts;

namespace ProjectHeleus.MangaService.Providers.Contracts
{
    public interface IMangaProvider
    {
        Task<IManga> GetMangaContentAsync(CatalogType catalogType, string mangaId);
        Task<IEnumerable<string>> GetMangaChapterContentAsync(CatalogType catalogType, string mangaId);
    }
}