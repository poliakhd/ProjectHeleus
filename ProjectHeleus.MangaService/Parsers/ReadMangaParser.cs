namespace ProjectHeleus.MangaService.Parsers
{
    using Microsoft.Extensions.Logging;

    using Interfaces;

    public partial class ReadMangaParser 
        : IParser
    {
        #region Private Members

        private readonly ILogger<ReadMangaParser> _logger;

        #endregion

        public ReadMangaParser(ILogger<ReadMangaParser> logger)
        {
            _logger = logger;
        }

        #region Implementation of IParser

        public virtual string Url { get; set; } = "http://readmanga.me";

        #endregion
    }
}