namespace ProjectHeleus.MangaService.Parsers
{
    using Microsoft.Extensions.Logging;

    public class MintMangaParser 
        : ReadMangaParser
    {
        #region Hides of IParser

        public override string Url { get; set; } = "http://mintmanga.com";

        #endregion

        public MintMangaParser(ILogger<ReadMangaParser> logger) 
            : base(logger)
        {

        }
    }
}