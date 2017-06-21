namespace ProjectHeleus.MangaService.Parsers
{
    using Microsoft.Extensions.Logging;

    using Interfaces;
    using Microsoft.Extensions.Caching.Distributed;

    public partial class MangaFoxParser 
        : IParser
    {
        #region Private Members

        private readonly ILogger<MangaFoxParser> _logger;
        private readonly IDistributedCache _cache;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Class logger instance</param>
        public MangaFoxParser(ILogger<MangaFoxParser> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        #region Implementation of IParser

        public string Url { get; set; } = "http://mangafox.me";

        #endregion
    }
}