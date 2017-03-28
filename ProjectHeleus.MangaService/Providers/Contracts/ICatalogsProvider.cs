using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Models;

namespace ProjectHeleus.MangaService.Providers.Contracts
{
    public interface ICatalogsProvider
    {
        Task<IEnumerable<Catalog>> GetAllSourcesAsync();
        Task<IEnumerable<Manga>> GetLatestSourceContentAsync(SourceType source);
        Task<IEnumerable<Manga>> GetNewSourceContentAsync(SourceType source);
    }
}