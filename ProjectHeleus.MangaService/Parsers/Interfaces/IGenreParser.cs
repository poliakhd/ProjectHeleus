namespace ProjectHeleus.MangaService.Parsers.Interfaces
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Shared.Types;
    using Shared.Models.Interfaces;

    public interface IGenreParser
    {
        /// <summary>
        /// Get available genres from catalog 
        /// </summary>
        /// <returns>List of catalog genres</returns>
        Task<IEnumerable<IGenre>> GetCatalogGenresAsync();

        /// <summary>
        /// Get list of available manga from genre
        /// </summary>
        /// <param name="sortType">Sorting option</param>
        /// <param name="url">Genre url</param>
        /// <param name="page">Requested page</param>
        /// <returns>List of manga from genre</returns>
        Task<IEnumerable<IManga>> GetGenreMangasAsync(SortType sortType, string url, int page);
    }
}