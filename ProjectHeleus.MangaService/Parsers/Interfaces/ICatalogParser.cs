namespace ProjectHeleus.MangaService.Parsers.Interfaces
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Shared.Types;
    using Shared.Models.Interfaces;

    public interface ICatalogParser
    {
        /// <summary>
        /// Get available catalog sortings
        /// </summary>
        /// <returns>List of catalog sortings</returns>
        Task<IEnumerable<ISort>> GetCatalogSortings();

        /// <summary>
        /// Get list of available manga from catalog
        /// </summary>
        /// <param name="sortType">Sorting option</param>
        /// <param name="page">Requested page</param>
        /// <returns>List of manga from catalog</returns>
        Task<IEnumerable<IManga>> GetCatalogMangasAsync(SortType sortType, int page);
    }
}