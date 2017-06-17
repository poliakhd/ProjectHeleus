namespace ProjectHeleus.MangaService.Parsers
{
    using System.Threading.Tasks;
    using Shared.Models.Interfaces;

    public partial class MintMangaParser
    {
        #region Overrides of ReadMangaParser

        public override Task<IChapterImages> GetMangaChapterImagesAsync(string url)
        {
            return base.GetMangaChapterImagesAsync(url + "?mature=1");
        }

        #endregion
    }
}