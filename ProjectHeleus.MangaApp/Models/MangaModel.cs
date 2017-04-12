namespace ProjectHeleus.MangaApp.Models
{
    using System;
    using System.Collections.Generic;

    public class MangaModel 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> AlternateNames { get; set; }

        public int Status { get; set; }
        public int Published { get; set; }
        public string Volumes { get; set; }
        public int Views { get; set; }
        public float Rating { get; set; }
        public float RatingLimit { get; set; }
        public string Description { get; set; }

        public IEnumerable<Uri> Covers { get; set; }
        public IEnumerable<GenreModel> Genres { get; set; }

        public IEnumerable<AuthorModel> Authors { get; set; }
        public IEnumerable<TranslatorModel> Translators { get; set; }
        
        public IEnumerable<ChapterModel> Chapters { get; set; }
    }
}