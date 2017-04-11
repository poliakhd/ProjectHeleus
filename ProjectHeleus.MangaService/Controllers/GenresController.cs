using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectHeleus.MangaService.Models.Interfaces;

namespace ProjectHeleus.MangaService.Controllers
{
    public class GenresController : Controller
    {
        [Route("api/[controller]/{catalog}")]
        public async Task<IGenre> GetAllGenres()
        {
            return null;
        }
    }
}