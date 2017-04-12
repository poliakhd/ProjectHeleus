namespace ProjectHeleus.MangaService.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using Extensions;
    using Models.Interfaces;
    using Providers.Interfaces;


    public class MangaController : Controller
    {
        #region Private Members

        private readonly IMangasProvider _mangasProvider;

        #endregion

        public MangaController(IMangasProvider mangasProvider)
        {
            _mangasProvider = mangasProvider;
        }

        [Route("api/[controller]/{catalog}/{manga}")]
        public async Task<IManga> GetMangaContent(string catalog, string manga)
        {
            return await _mangasProvider.GetMangaContentAsync(catalog.GetCatalogType(), manga);
        }

        [Route("api/[controller]/{catalog}/{manga}/{volume}/{chapter}")]
        public async Task<IChapterImages> GetMangaChapterContent(string catalog, string manga, string volume, string chapter)
        {
            return await _mangasProvider.GetMangaChapterContentAsync(catalog.GetCatalogType(), $"/{manga}/{volume}/{chapter}");
        }

        [Route("api/[controller]/{catalog}/{manga}/{volume}/{chapter}/{page}")]
        public async Task<IChapterImages> GetMangaChapterContent(string catalog, string manga, string volume, string chapter, string page)
        {
            return await _mangasProvider.GetMangaChapterContentAsync(catalog.GetCatalogType(), $"/{manga}/{volume}/{chapter}/{page}");
        }
    }
}