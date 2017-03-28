using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Parsers.Core;
using ProjectHeleus.MangaService.Providers.Contracts;

namespace ProjectHeleus.MangaService.Controllers
{
    public class SourcesController : Controller
    {
        #region Private Members

        private readonly ISourcesProvider _sourcesProvider;

        #endregion

        public SourcesController(ISourcesProvider sourcesProvider)
        {
            _sourcesProvider = sourcesProvider;
        }

        [Route("api/[controller]")]
        public async Task<IEnumerable<Source>> GetAllSources()
        {
            return await _sourcesProvider.GetAllSourcesAsync();
        }

        [Route("api/[controller]/{source}")]
        public async Task<IEnumerable<Manga>> GetSourceContent(SourceType source)
        {
            return await _sourcesProvider.GetLatestSourceContentAsync(source);
        }

        [Route("api/[controller]/{source}/{sort}")]
        public async Task<IEnumerable<string>> GetSourceContent(SourceType source, string sortType)
        {
            return null;
        }
    }
}
