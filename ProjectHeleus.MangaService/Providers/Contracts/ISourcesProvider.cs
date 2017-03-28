using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Parsers.Core;

namespace ProjectHeleus.MangaService.Providers.Contracts
{
    public interface ISourcesProvider
    {
        Task<IEnumerable<Source>> GetAllSourcesAsync();
        Task<IEnumerable<Manga>> GetLatestSourceContentAsync(SourceType source);
    }
}