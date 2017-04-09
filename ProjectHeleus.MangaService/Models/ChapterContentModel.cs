using System.Collections.Generic;
using ProjectHeleus.MangaService.Models.Contracts;

namespace ProjectHeleus.MangaService.Models
{
    public class ChapterContentModel 
        : IChapterContent
    {
        public IEnumerable<string> Images { get; set; }
    }
}