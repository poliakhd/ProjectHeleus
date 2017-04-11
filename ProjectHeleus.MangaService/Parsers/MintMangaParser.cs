namespace ProjectHeleus.MangaService.Parsers
{
    using Microsoft.Extensions.Logging;

    public class MintMangaParser 
        : ReadMangaParser
    {
        #region Hides of IParser

        public new string Url { get; set; } = "http://mintmanga.com";

        #endregion

        public MintMangaParser(ILogger<ReadMangaParser> logger) 
            : base(logger)
        {
            NewUrl = $"{Url}/list?sortType=created";
            UpdateUrl = $"{Url}/list?sortType=updated";
            RatingUrl = $"{Url}/list?sortType=votes";
            PopularUrl = $"{Url}/list?sortType=rate";
        }
    }
}