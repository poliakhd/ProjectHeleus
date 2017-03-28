using System;
using ProjectHeleus.MangaService.Models;

namespace ProjectHeleus.MangaService.Core
{
    public class ApiContextInitializer
    {
        public static void Initialize(ApiContext context)
        {
            context.Database.EnsureCreated();

            var sources = new Catalog[]
            {
                new Catalog() {Title = "readmanga.ru", Url = "http://readmange.ru"},
                new Catalog() {Title = "mangafox.com", Url = "http://mangafox.com"}
            };

            context.Sources.AddRange(sources);
            context.SaveChanges();
        }
    }
}