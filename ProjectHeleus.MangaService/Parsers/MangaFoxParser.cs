using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Models.Contracts;
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
        
        public override async Task<IManga> GetMangaContent(string manga)
        {
            var web = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync($"{Url}/manga/{manga}");
            var mangaRaw = web.QuerySelector("#page > .left");

            if (mangaRaw == null)
                return null;

            var formattedManga = new MangaModel();
            formattedManga.Id = manga;

            GetInformation(formattedManga, mangaRaw);

            GetVolume(mangaRaw, formattedManga);
            GetViews(mangaRaw, formattedManga);
            GetGenres(mangaRaw, formattedManga);
            GetAuthors(mangaRaw, formattedManga);
            GetTranslators(mangaRaw, formattedManga);
            GetPublishedYear(mangaRaw, formattedManga);
            GetChapters(mangaRaw, formattedManga);

            return formattedManga;
        }
        public override Task<IChapterContent> GetMangaChapterContent(string manga)
        {
            return null;
        }

        #endregion

        #region Internal Methods

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

                        var id = mangaItem.QuerySelector(".manga_text a")?.GetAttribute("href").TrimEnd('/');
                        id = id.Substring(id.LastIndexOf('/') + 1);

                        var manga = new MangaShortModel
                        {
                            Id = id,
                            Title = mangaItem.QuerySelector(".manga_text a")?.TextContent.Replace("\n", "").TrimStart(' ').TrimEnd(' '),
                            TitleAlt = mangaItem.QuerySelector(".manga_text a")?.TextContent.Replace("\n", "").TrimStart(' ').TrimEnd(' '),
                            Url = mangaItem.QuerySelector(".manga_text a")?.GetAttribute("href").Replace(Url, ""),
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
                                .Select(x => new GenreModel() { Title = x, Url = null });

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

        private void GetInformation(MangaModel formattedManga, IElement mangaRaw)
        {
            formattedManga.Name = mangaRaw.QuerySelector("#title h1")?.TextContent;
            formattedManga.AlternateNames = mangaRaw.QuerySelector("#title h3")?.TextContent.Split(';');
            formattedManga.Description = mangaRaw.QuerySelector(".summary")?.TextContent;
            formattedManga.Covers =
                mangaRaw.QuerySelectorAll("#series_info .cover img")?.Select(x => new Uri(x.GetAttribute("src")));

            var textRating = mangaRaw.QuerySelectorAll("#series_info > .data > span")?[2].TextContent;

            if (string.IsNullOrEmpty(textRating))
                return;

            var firstRatingIndex = textRating.IndexOf(' ') + 1;
            var lastRatinIndex = textRating.IndexOf('/') - 1;

            formattedManga.Rating =
                float.Parse(textRating.Substring(firstRatingIndex, lastRatinIndex - firstRatingIndex));
            formattedManga.RatingLimit = 5;
        }
        private void GetVolume(IElement mangaRaw, MangaModel formattedManga)
        {
            var volumes = mangaRaw.QuerySelectorAll(".volume");
            var lastVolume = volumes.FirstOrDefault()?.TextContent;

            if (!string.IsNullOrEmpty(lastVolume))
                formattedManga.Volumes = lastVolume.Contains("TBD") ? $"> {volumes[1].TextContent.Split(' ')[1]}" : $"{lastVolume.Split(' ')[1]}";
            else
                formattedManga.Volumes = "N/A";
        }
        private void GetViews(IElement mangaRaw, MangaModel formattedManga)
        {
            var views = mangaRaw.QuerySelectorAll("#series_info > .data > span")?[1].TextContent.TrimStart().Split(' ');

            formattedManga.Views = -1;

            if (views != null && views.Any())
                if (int.TryParse(views[3].Replace(",", ""), out int result))
                    formattedManga.Views = result;
        }
        private void GetGenres(IElement mangaRaw, MangaModel formattedManga)
        {
            var titleInfo = mangaRaw.QuerySelectorAll("#title table td");

            formattedManga.Genres =
                    titleInfo[3].QuerySelectorAll("a")
                        .Select(x => new GenreModel() { Title = x.TextContent, Url = x.GetAttribute("href").Replace(Url, "") });
        }
        private void GetAuthors(IElement mangaRaw, MangaModel formattedManga)
        {
            var titleInfo = mangaRaw.QuerySelectorAll("#title table td");

            formattedManga.Authors =
                    titleInfo[1].QuerySelectorAll("a")
                        .Select(x => new AuthorModel() { Name = x.TextContent, Url = x.GetAttribute("href").Replace(Url, "") });
        }
        private void GetTranslators(IElement mangaRaw, MangaModel formattedManga)
        {

        }
        private void GetPublishedYear(IElement mangaRaw, MangaModel formattedManga)
        {
            var titleInfo = mangaRaw.QuerySelectorAll("#title table td");
            formattedManga.Published = int.Parse(titleInfo[0].QuerySelector("a").TextContent);
        }
        private void GetChapters(IElement mangaRaw, MangaModel formattedManga)
        {
            var chaptersWeb = mangaRaw.QuerySelectorAll(".tips");
            var datesWeb = mangaRaw.QuerySelectorAll("#chapters .date");

            var chapters = new List<ChapterModel>();
            for (int i = 0; i < chaptersWeb.Length; i++)
            {
                chapters.Add(new ChapterModel()
                {
                    Name = chaptersWeb[i].TextContent,
                    Url = chaptersWeb[i].GetAttribute("href").Replace(Url, ""),
                    Date = datesWeb[i].TextContent
                });
            }

            formattedManga.Chapters = chapters;
        }

        #endregion
    }
}