using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaService.Models;

namespace ProjectHeleus.MangaService.Providers.Contracts
{
    public interface ISourcesProvider
    {
        Task<IEnumerable<Source>> GetAllSourcesAsync();
        Task<IEnumerable<Manga>> GetSourceContentAsync(int sourceId);
    }
}