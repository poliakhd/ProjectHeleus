namespace ProjectHeleus.Shared.Models
{
    using System.Collections.Generic;

    using Interfaces;

    public class ChapterImagesModel 
        : IChapterImages
    {
        public IEnumerable<string> Images { get; set; }
    }
}