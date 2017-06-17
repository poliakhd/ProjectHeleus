namespace ProjectHeleus.MangaService.Parsers.Interfaces
{
    using System.Threading.Tasks;

    using Shared.Models.Interfaces;

    public interface IMangaParser
    {
        /// <summary>
        /// Get available manga from catalog
        /// </summary>
        /// <param name="url">Manga url</param>
        /// <returns>Catalot manga</returns>
        Task<IManga> GetCatalogMangaAsync(string url);


        /// <summary>
        /// Get available chapter images from catalog manga
        /// </summary>
        /// <param name="url">Chapter url</param>
        /// <returns>Manga chapter images</returns>
        Task<IChapterImages> GetMangaChapterImagesAsync(string url);
    }
}