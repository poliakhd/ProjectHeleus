using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectHeleus.MangaService.Core;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Providers.Contracts;

namespace ProjectHeleus.MangaService.Providers
{
    public class BasicSourcesProvider
        : ISourcesProvider
    {
        #region Private Members

        private readonly ApiContext _context;

        #endregion
        
        public BasicSourcesProvider(ApiContext context)
        {
            _context = context;
        }

        #region Implementation of ISourcesProvider

        public async Task<IEnumerable<Source>> GetAllSourcesAsync()
        {
            return await _context.Sources.ToListAsync();
        }

        public Task<IEnumerable<Manga>> GetSourceContentAsync(int sourceId)
        {
            return null;
        }

        #endregion
    }
}