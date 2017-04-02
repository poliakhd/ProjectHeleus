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
                new Catalog() {Title = "mangafox.me", Url = "http://mangafox.me/"},
                new Catalog() {Title = "readmanga.me", Url = "http://readmanga.me/"}
            };

            context.Sources.AddRange(sources);
            context.SaveChanges();
        }
    }
}