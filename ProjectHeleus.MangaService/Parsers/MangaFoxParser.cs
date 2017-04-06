using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Models.Contracts;
using ProjectHeleus.MangaService.Parsers.Contracts;
using ProjectHeleus.MangaService.Parsers.Core;

namespace ProjectHeleus.MangaService.Parsers
{
    public class MangaFoxParser 
        : DefaultParser
    {
        #region Private Members

        private string _updateUrl;
        private string _ratingUrl;
        private string _popularUrl;

        #endregion

        public new string Url { get; set; } = "http://mangafox.me";

        public MangaFoxParser()
        {
            _updateUrl = $"{Url}/directory/?latest";
            _ratingUrl = $"{Url}/directory/?rating";
            _popularUrl = $"{Url}/directory/";
        }

        #region Overrides of IParser

        public override async Task<IEnumerable<IManga>> GetUpdateContent(int page)
        {
            return await GetListContent(_updateUrl, page);
        }
        public override async Task<IEnumerable<IManga>> GetRatingContent(int page)
        {
            return await GetListContent(_ratingUrl, page);
        }
        public override async Task<IEnumerable<IManga>> GetPopularContent(int page)
        {
            return await GetListContent(_popularUrl, page);
        }
        
        public override Task<IManga> GetMangaContent(string mangaId)
        {
            throw new System.NotImplementedException();
        }
        public override Task<IEnumerable<string>> GetMangaChapterContent(string w)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        private async Task<IEnumerable<MangaShortModel>> GetListContent(string sourceUrl, int page)
        {
            var webSource = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(sourceUrl);
            var mangas = webSource.QuerySelectorAll("#mangalist .list li");

            var mangasResult = new List<MangaShortModel>();

            if (mangas.Any())
            {
                try
                {
                    foreach (var mangaItem in mangas)
                    {
                        #region Header

                        var manga = new MangaShortModel
                        {
                            Title = mangaItem.QuerySelector(".manga_text a")?.TextContent.Replace("\n", "").TrimStart(' ').TrimEnd(' '),
                            TitleAlt = mangaItem.QuerySelector(".manga_text a")?.TextContent.Replace("\n", "").TrimStart(' ').TrimEnd(' '),
                            Url = mangaItem.QuerySelector(".manga_text a")?.GetAttribute("href"),
                            Cover = mangaItem.QuerySelector(".manga_img div img")?.GetAttribute("src")
                        };

                        #endregion

                        #region Rating

                        var ratings = mangaItem.QuerySelector(".rate")?.TextContent;

                        if (ratings != null)
                        {
                            manga.Rating = float.Parse(ratings);
                            manga.RatingLimit = 5;
                        }

                        #endregion

                        var info = mangaItem.QuerySelectorAll(".info");

                        #region Views
                        
                        if (info.Length > 1)
                            manga.Views = int.Parse(info[1].TextContent.Split(' ')[1].Replace(".", "").Replace(",", ""));

                        #endregion

                        #region Genres

                        manga.Authors = null;
                        manga.Genres =
                            info[0].TextContent.Split(',')
                                .Select(x => x.Replace(".", "").Trim())
                                .Where(y => !string.IsNullOrEmpty(y))
                                .Select(x => new GenreModel() {Title = x, Url = null});

                        #endregion

                        mangasResult.Add(manga);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return mangasResult;
        }
    }
}