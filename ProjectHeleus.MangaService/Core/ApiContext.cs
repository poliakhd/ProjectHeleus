using Microsoft.EntityFrameworkCore;
using ProjectHeleus.MangaService.Models;

namespace ProjectHeleus.MangaService.Core
{
    public class ApiContext
        : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        { }
        
        public DbSet<Source> Sources { get; set; }
    }
}