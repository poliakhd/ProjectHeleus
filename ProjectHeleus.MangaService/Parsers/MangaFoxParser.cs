using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Extensions;
using ProjectHeleus.MangaService.Models.Mangas;
using ProjectHeleus.MangaService.Parsers.Contracts;

namespace ProjectHeleus.MangaService.Parsers
{
    public class MangaFoxParser 
        : IParser
    {
        #region Private Members

        private string _updateUrl;

        #endregion

        public string Url { get; set; } = "http://mangafox.me";

        public MangaFoxParser()
        {
            _updateUrl = $"{Url}/directory/";
        }

        #region Implementation of IParser

        public async Task<IEnumerable<ListManga>> GetUpdateContent(int page)
        {
            return await GetListContent(_updateUrl, page);
        }
        public Task<IEnumerable<ListManga>> GetNewContent(int page)
        {
            throw new System.NotImplementedException();
        }

        public Task<Manga> GetMangaContent(string mangaId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ListManga>> GetRatingContent(int page)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<string>> GetMangaChapterContent(string w)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        private async Task<IEnumerable<ListManga>> GetListContent(string sourceUrl, int page)
        {
            throw new System.NotImplementedException();
        }
    }
}