using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreWebApi.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CoreWebApi.DTOs;

namespace CoreWebApi.Controllers
{
    [Route("api/genres")]
    public class GenresController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet()]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            var genres = await context.Genres.ToListAsync();
            var genreDtos = mapper.Map<List<GenreDTO>>(genres);
            //    return await context.Genres.ToListAsync();
            return genreDtos;
        }
        [HttpGet("{Id:int}", Name = "getGenre")]
        public async Task<ActionResult<Genre>> Get(int id)
        {
            var genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);
            return genre;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]GenereCreationDTO genreCreation)
        {
            var genre = mapper.Map<Genre>(genreCreation);
            await context.AddAsync(genre);
            await context.SaveChangesAsync();
            var gernesdto = mapper.Map<GenreDTO>(genre);
            return new CreatedAtRouteResult("getGenre", new { id = genre.Id }, genre);

        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGenre(int id, [FromBody]GenereCreationDTO genreCreation)
        {
            var genre = mapper.Map<Genre>(genreCreation);
            genre.Id = id;
            context.Entry(genre).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGenre(int id)
        {
            var exists = await context.Genres.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }
            context.Genres.Remove(new Genre { Id = id });
            //dont Forget to call save changes after Add, Update,Delete
           await context.SaveChangesAsync();
            return NoContent();
        }

    }
}