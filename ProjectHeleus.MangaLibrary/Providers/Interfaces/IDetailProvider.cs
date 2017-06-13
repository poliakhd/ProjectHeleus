namespace ProjectHeleus.MangaLibrary.Providers.Interfaces
{
    using System.Threading.Tasks;
    using Shared.Models;

    public interface IDetailProvider
    {
        Task<MangaModel> GetMangaContent(CatalogModel catalog, MangaPreviewModel mangaPreview);
    }
}