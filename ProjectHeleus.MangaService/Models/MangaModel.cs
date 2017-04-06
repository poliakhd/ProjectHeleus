using System;
using System.Collections.Generic;
using ProjectHeleus.MangaService.Models.Contracts;
using ProjectHeleus.MangaService.Models.Core;

namespace ProjectHeleus.MangaService.Models
{
    public class MangaModel : IManga
    {
        public string Name { get; set; }
        public string AlternativeName { get; set; }
        public string OriginalName { get; set; }

        public MangaStatusModel Status { get; set; }
        public int Published { get; set; }
        public int Volumes { get; set; }
        public int Views { get; set; }
        public float Rating { get; set; }
        public string Description { get; set; }

        public IEnumerable<Uri> Covers { get; set; }
        public IEnumerable<IGenre> Genres { get; set; }

        public IEnumerable<IAuthor> Authors { get; set; }
        public IEnumerable<ITranslator> Translators { get; set; }
        
        public IEnumerable<IChapter> Chapters { get; set; }
    }
}