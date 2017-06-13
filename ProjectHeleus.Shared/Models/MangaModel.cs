namespace ProjectHeleus.Shared.Models
{
    using System;
    using System.Collections.Generic;
    using Converters;
    using Types;
    using Interfaces;
    using Newtonsoft.Json;

    public class MangaModel 
        : IManga
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> AlternateNames { get; set; }

        public StatusType Status { get; set; }
        public int Published { get; set; }
        public string Volumes { get; set; }
        public int Views { get; set; }
        public float Rating { get; set; }
        public float RatingLimit { get; set; }
        public string Description { get; set; }

        public IEnumerable<string> Covers { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<IEnumerable<GenreModel>>))]
        public IEnumerable<IGenre> Genres { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<IEnumerable<AuthorModel>>))]
        public IEnumerable<IAuthor> Authors { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<IEnumerable<TranslatorModel>>))]
        public IEnumerable<ITranslator> Translators { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<IEnumerable<ChapterModel>>))]
        public IEnumerable<IChapter> Chapters { get; set; }
    }
}