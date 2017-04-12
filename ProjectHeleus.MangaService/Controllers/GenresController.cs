namespace ProjectHeleus.MangaService.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using Extensions;
    using Models.Interfaces;
    using Providers.Interfaces;

    public class GenresController : Controller
    {
        #region Private Members

        private readonly IGenresProvider _genresProvider;

        #endregion

        public GenresController(IGenresProvider genresProvider)
        {
            _genresProvider = genresProvider;
        }

        [Route("api/[controller]/{catalog}")]
        public async Task<IEnumerable<IGenre>> GetAllGenres(string catalog)
        {
            return await _genresProvider.GetAllGenresAsync(catalog.GetCatalogType());
        }
    }
}