namespace ProjectHeleus.MangaService.Providers.Interfaces
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Core;
    using Models.Interfaces;

    public interface IGenresProvider
    {
        Task<IEnumerable<IGenre>> GetAllGenresAsync(CatalogType catalogType);
        Task<IEnumerable<IManga>> GetAllFromGenreAsync(CatalogType catalogType, string url);
    }
}