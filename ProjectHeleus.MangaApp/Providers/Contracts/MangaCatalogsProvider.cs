using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;
using ProjectHeleus.MangaApp.Models;

namespace ProjectHeleus.MangaApp.Providers.Contracts
{
    public class MangaCatalogsProvider
        : ICatalogsProvider
    {
        public Task<IEnumerable<Catalog>> GetAllCatalogs()
        {
            using (var client = new HttpClient())
            {
                //client.GetStringAsync()
            }

            return null;
        }
    }
}