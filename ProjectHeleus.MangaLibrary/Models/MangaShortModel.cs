namespace ProjectHeleus.MangaLibrary.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class MangaShortModel 
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Cover { get; set; }
        public int Views { get; set; }
        public float Rating { get; set; }
        public float RatingLimit { get; set; }

        [JsonIgnore]
        public int RatingMax => Convert.ToInt32(RatingLimit);
        public IEnumerable<AuthorModel> Authors { get; set; }

        public IEnumerable<GenreModel> Genres { get; set; }
    }
}