using System;
using ProjectHeleus.MangaService.Models;

namespace ProjectHeleus.MangaService.Core
{
    public class ApiContextInitializer
    {
        public static void Initialize(ApiContext context)
        {
            context.Database.EnsureCreated();

            var sources = new Source[]
            {
                new Source() {Title = "readmanga.ru", Url = "http://readmange.ru"},
                new Source() {Title = "mangafox.com", Url = "http://mangafox.com"}
            };

            context.Sources.AddRange(sources);
            context.SaveChanges();
        }
    }
}