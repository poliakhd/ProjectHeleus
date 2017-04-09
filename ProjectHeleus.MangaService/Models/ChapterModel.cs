using ProjectHeleus.MangaService.Models.Contracts;

namespace ProjectHeleus.MangaService.Models
{
    public class ChapterModel
        : IChapter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Date { get; set; }
    }
}