namespace ProjectHeleus.MangaService.Models
{
    using System;
    using System.Collections.Generic;

    using Core;
    using Interfaces;

    public class MangaModel 
        : IManga
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> AlternateNames { get; set; }

        public MangaStatusModel Status { get; set; }
        public int Published { get; set; }
        public string Volumes { get; set; }
        public int Views { get; set; }
        public float Rating { get; set; }
        public float RatingLimit { get; set; }
        public string Description { get; set; }

        public IEnumerable<Uri> Covers { get; set; }
        public IEnumerable<IGenre> Genres { get; set; }

        public IEnumerable<IAuthor> Authors { get; set; }
        public IEnumerable<ITranslator> Translators { get; set; }
        
        public IEnumerable<IChapter> Chapters { get; set; }
    }
}