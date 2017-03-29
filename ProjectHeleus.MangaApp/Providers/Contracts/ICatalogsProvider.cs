using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectHeleus.MangaApp.Models;

namespace ProjectHeleus.MangaApp.Providers.Contracts
{
    public interface ICatalogsProvider
    {
        Task<IEnumerable<Catalog>> GetAllCatalogs();
    }
}