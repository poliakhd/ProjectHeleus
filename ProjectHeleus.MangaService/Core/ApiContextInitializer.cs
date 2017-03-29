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
                new Catalog() {Title = "mangafox.com", Url = "http://mangafox.com"},
                new Catalog() {Title = "readmanga.ru", Url = "http://readmanga.ru"}
            };

            context.Sources.AddRange(sources);
            context.SaveChanges();
        }
    }
}