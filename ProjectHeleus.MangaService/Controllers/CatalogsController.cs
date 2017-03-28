using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectHeleus.MangaService.Controllers.Core;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Providers.Contracts;

namespace ProjectHeleus.MangaService.Controllers
{
    public class CatalogsController : Controller
    {
        #region Private Members

        private readonly ICatalogsProvider _catalogsProvider;

        #endregion

        public CatalogsController(ICatalogsProvider catalogsProvider)
        {
            _catalogsProvider = catalogsProvider;
        }

        [Route("api/[controller]")]
        public async Task<IEnumerable<Catalog>> GetAllSources()
        {
            return await _catalogsProvider.GetAllSourcesAsync();
        }

        [Route("api/[controller]/{source}")]
        public async Task<IEnumerable<Manga>> GetSourceContent(SourceType source)
        {
            return await _catalogsProvider.GetLatestSourceContentAsync(source);
        }

        [Route("api/[controller]/{source}/{sort}")]
        public async Task<IEnumerable<Manga>> GetSourceContent(SourceType source, string sort)
        {
            var sortType = (SortType)Enum.Parse(typeof(SortType), sort, true);

            switch (sortType)
            {
                case SortType.Newest:
                    return await _catalogsProvider.GetNewSourceContentAsync(source);
                case SortType.Latest:
                    return await _catalogsProvider.GetLatestSourceContentAsync(source);
                default:
                    return await _catalogsProvider.GetLatestSourceContentAsync(source);
            }
        }
    }
}
