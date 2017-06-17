namespace ProjectHeleus.MangaService.Parsers
{
    using Microsoft.Extensions.Logging;

    using Interfaces;

    public partial class MangaFoxParser 
        : IParser
    {
        #region Private Members

        private readonly ILogger<MangaFoxParser> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Class logger instance</param>
        public MangaFoxParser(ILogger<MangaFoxParser> logger)
        {
            _logger = logger;
        }

        #region Implementation of IParser

        public string Url { get; set; } = "http://mangafox.me";

        #endregion
    }
}