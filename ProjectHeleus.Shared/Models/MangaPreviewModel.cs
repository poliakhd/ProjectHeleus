namespace ProjectHeleus.Shared.Models
{
    using Interfaces;

    public class MangaPreviewModel 
        : IManga
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Cover { get; set; }
        public float Rating { get; set; }
        public float RatingLimit { get; set; }
    }
}