using System;
using System.Collections.Generic;

namespace ProjectHeleus.MangaService.Models.Mangas
{
    public enum MangaStatus
    {
        Hold,
        Ongoing,
        Completed,
        Closed
    }

    public class Manga
    {
        public string Name { get; set; }
        public string AlternativeName { get; set; }
        public string OriginalName { get; set; }

        public MangaStatus Status { get; set; }
        public int Published { get; set; }
        public int Volumes { get; set; }
        public int Views { get; set; }
        public float Rating { get; set; }
        public string Description { get; set; }

        public IEnumerable<Uri> Covers { get; set; }
        public IEnumerable<Genre> Genres { get; set; }

        public IEnumerable<Author> Authors { get; set; }
        public IEnumerable<Translator> Translators { get; set; }
        
        public IEnumerable<MangaChapter> Chapters { get; set; }
    }
}