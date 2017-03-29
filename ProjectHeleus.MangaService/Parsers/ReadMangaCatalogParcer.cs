using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using ProjectHeleus.MangaService.Models;
using ProjectHeleus.MangaService.Parsers.Contracts;

namespace ProjectHeleus.MangaService.Parsers
{
    public class ReadMangaCatalogParcer 
        : ICatalogParser
    {
        private const string NewUrl = "http://readmanga.me/list?sortType=created";
        private const string UpdatedUrl = "http://readmanga.me/list?sortType=updated";
        
        public async Task<IEnumerable<Manga>> GetLatestContent()
        {
            return await GetContent(UpdatedUrl);
        }
        public async Task<IEnumerable<Manga>> GetNewContent()
        {
            return await GetContent(NewUrl);
        }

        private async Task<IEnumerable<Manga>> GetContent(string siteUrl)
        {
            var source = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(siteUrl);
            var list = source.GetElementsByClassName("tile col-sm-6");

            var mangas = new List<Manga>();

            if (list.Any())
            {
                try
                {
                    foreach (var element in list)
                    {
                        var manga = new Manga();

                        #region Title

                        var title = element.GetElementsByTagName("h3");

                        if (title.Any())
                            manga.Title = title[0].TextContent.Replace("\n", "").TrimStart(' ').TrimEnd(' ');

                        #endregion

                        #region TitleAlt

                        var titleAlt = element.GetElementsByTagName("h4");

                        if (titleAlt.Any())
                            manga.TitleAlt = titleAlt[0].TextContent.Replace("\n","").TrimStart(' ').TrimEnd(' ');

                        #endregion

                        #region Url

                        var url = element.GetElementsByClassName("img");

                        if (url.Any())
                        {
                            url = url[0].GetElementsByTagName("a");

                            if (url.Any())
                                manga.Url = url[0].GetAttribute("href");
                        }

                        #endregion

                        #region ImageUrl

                        var imageUrl = element.GetElementsByClassName("img");

                        if (imageUrl.Any())
                        {
                            imageUrl = imageUrl[0].GetElementsByTagName("a");

                            if (imageUrl.Any())
                            {
                                imageUrl = imageUrl[0].GetElementsByTagName("img");

                                if (imageUrl.Any())
                                {
                                    manga.ImageUrl = imageUrl[0].GetAttribute("src");
                                }
                            }
                        }

                        #endregion

                        #region Rating

                        var rating = element.GetElementsByClassName("desc");

                        if (rating.Any())
                        {
                            rating = rating[0].GetElementsByClassName("rating");

                            if (rating.Any())
                            {
                                var ratingStatus = rating[0].GetAttribute("title").Split(' ');

                                manga.Rating = float.Parse(ratingStatus[0]);
                                manga.RatingLimit = float.Parse(ratingStatus[2]);
                            }
                        }

                        #endregion

                        #region TileInfo

                        var tileInfo = element.GetElementsByClassName("tile-info");

                        #region Views

                        if (tileInfo.Any())
                        {
                            var views = tileInfo[0].GetElementsByTagName("p");

                            if (views.Any())
                            {
                                manga.Views =
                                    int.Parse(views[0].TextContent.Substring(views[0].TextContent.LastIndexOf(' ')));
                            }
                        }

                        #endregion

                        #region Authors

                        if (tileInfo.Any())
                        {
                            var brIndex =
                                tileInfo[0].Children.Index(
                                    tileInfo[0].ChildNodes.FirstOrDefault(x => x is IHtmlBreakRowElement));

                            var authors = tileInfo[0].GetElementsByTagName("a");

                            if (authors.Any())
                            {
                                var authorsLocal = new List<Author>();

                                foreach (var author in authors.Take(brIndex))
                                    authorsLocal.Add(new Author()
                                    {
                                        Name = author.TextContent,
                                        Url = author.GetAttribute("href")
                                    });

                                manga.Authors = authorsLocal;
                            }
                        }

                        #endregion

                        #region Genres

                        if (tileInfo.Any())
                        {
                            var brIndex =
                                tileInfo[0].Children.Index(
                                    tileInfo[0].ChildNodes.FirstOrDefault(x => x is IHtmlBreakRowElement));

                            var genres = tileInfo[0].GetElementsByTagName("a");

                            if (genres.Any())
                            {
                                var genresLocal = new List<Genre>();

                                foreach (var genre in genres.Skip(brIndex))
                                    genresLocal.Add(new Genre()
                                    {
                                        Title = genre.TextContent,
                                        Url = genre.GetAttribute("href")
                                    });

                                manga.Genres = genresLocal;
                            }
                        }

                        #endregion

                        #endregion

                        mangas.Add(manga);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return mangas;
        }
    }
}