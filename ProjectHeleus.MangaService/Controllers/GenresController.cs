namespace ProjectHeleus.MangaService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core;
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

        [Route("api/[controller]/{catalog}/{genre}")]
        public async Task<IEnumerable<IManga>> GetAllFromGenre(string catalog, string genre)
        {
            return await GetAllFromGenre(catalog, genre, "rating", 0);
        }

        [Route("api/[controller]/{catalog}/{genre}/{page:int}")]
        public async Task<IEnumerable<IManga>> GetAllFromGenre(string catalog, string genre, int page)
        {
            return await GetAllFromGenre(catalog, genre, "rating", page);
        }

        [Route("api/[controller]/{catalog}/{genre}/{sort}")]
        public async Task<IEnumerable<IManga>> GetAllFromGenre(string catalog, string genre, string sort)
        {
            return await GetAllFromGenre(catalog, genre, sort, 0);
        }

        [Route("api/[controller]/{catalog}/{genre}/{sort}/{page:int}")]
        public async Task<IEnumerable<IManga>> GetAllFromGenre(string catalog, string genre, string sort, int page)
        {
            var sortType = (SortType)Enum.Parse(typeof(SortType), sort, true);
            var catalogType = catalog.GetCatalogType();

            return await _genresProvider.GetAllFromGenreAsync(catalogType, sortType, genre, page);
        }
    }
}