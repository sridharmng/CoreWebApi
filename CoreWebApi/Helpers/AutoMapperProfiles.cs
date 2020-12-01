using AutoMapper;
using CoreWebApi.DTOs;
using CoreWebApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<GenereCreationDTO, Genre>().ReverseMap();

            CreateMap<Person, PersonDto>().ReverseMap();
            CreateMap<PersonCreationDto, Person>().ReverseMap();
            CreateMap<Person, PersonPatchDTO>().ReverseMap();
            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<MovieCreationDTO, Movie>().
               ForMember(x => x.MoviesGenres, opts => opts.MapFrom(MapMoviesGenres))
               .ForMember(x=> x.MoviesActors, opts => opts.MapFrom())
               .ReverseMap();
            CreateMap<Movie, MoviePatchDTO>().ReverseMap();


        }
        private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesGenres>();
            for (int i = 0; i <=movieCreationDTO.Genresids.Count(); i++)
            {
                result.Add(new MoviesGenres() { GenreId = i });
            }
            return result;
        }

        private List<MoviesActors> MapMovieActors(MovieCreationDTO movieCreationDTO,Movie movie)
        {
            var result = new List<MoviesActors>();
            foreach(var actor in movieCreationDTO.Actors)
            {
                result.Add(new MoviesActors() { ActorId = actor.PersonId, Character = actor.Character });
            }
            return result;
        }

    }
}
