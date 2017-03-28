using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Extensions;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Parsers.Contracts;

namespace ProjectHeleus.MangaService.Parsers
{
    public class ReadMangaSourceParcer 
        : ISourceParser
    {
        private const string UpdatedUrl = "http://readmanga.me/list?sortType=updated";

        public async Task<IEnumerable<Manga>> GetLatestContent()
        {
            var dom = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(UpdatedUrl);
            var list = dom.QuerySelectorAll("tile col-sm-6");


            return null;
        }
    }
}