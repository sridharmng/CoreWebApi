using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using AutoMapper;
using CoreWebApi.DTOs;
using CoreWebApi.Entities;
using CoreWebApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace CoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private ApplicationDbContext dbcontext;
        private IMapper mapper;

        public MoviesController(ApplicationDbContext context, IMapper mapper)
        {
            dbcontext = context;
            this.mapper = mapper;
        }

        public async Task<ActionResult<List<MovieDTO>>> Get()
        {
            List<Movie> movies = await dbcontext.Movies.ToListAsync();
            List<MovieDTO> moviesDto = mapper.Map<List<MovieDTO>>(movies);
            return moviesDto;
        }

        [HttpGet("{id}", Name = "GetMovie")]
        public async Task<ActionResult<MovieDTO>> Get(int id)
        {
            Movie movie = await dbcontext.Movies.FirstOrDefaultAsync(x => x.Id == id);
            MovieDTO movieDTo = mapper.Map<MovieDTO>(movie);
            return movieDTo;
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            Movie movie = mapper.Map<Movie>(movieCreationDTO);
            return Ok();
            string dbpath = "";
            if (movieCreationDTO.Poster != null)
                if (FileHelper.CheckIfImageFile(movieCreationDTO.Poster))
                {
                    dbpath = await FileHelper.WriteFile(movieCreationDTO.Poster);
                }
                else
                {
                    return BadRequest(new { message = "Invalid file extension" });
                }
            movie.Title = movieCreationDTO.Title;
            movie.Summary = movieCreationDTO.Summary;
            movie.InTheaters = movieCreationDTO.InTheaters;
            movie.ReleaseDate = movieCreationDTO.ReleaseDate;
            movie.Poster = dbpath;
            dbcontext.Movies.Add(movie);
            await dbcontext.SaveChangesAsync();
            MovieDTO movieDTO = mapper.Map<MovieDTO>(movie);
            return new CreatedAtRouteResult("GetMovie", new Movie { Id = movie.Id }, movieDTO);
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }
            var entityMovieDb = await dbcontext.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (entityMovieDb == null)
            {
                return NotFound();
            }
            var entityMovieDTO = mapper.Map<MoviePatchDTO>(entityMovieDb);
            patchDocument.ApplyTo(entityMovieDTO, ModelState);
            var isValid = TryValidateModel(entityMovieDTO);
            if (!isValid)
            {
                return BadRequest();
            }
            mapper.Map(entityMovieDTO, entityMovieDb);
            await dbcontext.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMovie(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            Movie movieDb = await dbcontext.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (movieDb == null)
            {
                return NotFound();
            }
            if (movieCreationDTO.Poster != null)
            {
                FileHelper.DeleteFile(movieDb.Poster);
                movieDb.Poster = await FileHelper.WriteFile(movieCreationDTO.Poster);
            }
            movieDb = mapper.Map(movieCreationDTO, movieDb);
            movieDb.Id = id;
            movieDb.Poster = movieDb.Poster;
            dbcontext.Entry(movieDb).State = EntityState.Modified;
            await dbcontext.SaveChangesAsync();
            MovieDTO movieDTO = mapper.Map<MovieDTO>(movieDb);
            return new CreatedAtRouteResult("", new Movie { Id = movieDb.Id }, movieDTO);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMovie(int id)
        {
           var exists = await dbcontext.Movies.AnyAsync(m => m.Id == id);
            if (!exists)
            {
                return NotFound();
            };
            dbcontext.Movies.Remove(new Movie() { Id = id });
            await dbcontext.SaveChangesAsync();
            return NoContent();
        }

    }
}