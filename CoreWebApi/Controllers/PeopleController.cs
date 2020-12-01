using AutoMapper;
using CoreWebApi.DTOs;
using CoreWebApi.Entities;
using CoreWebApi.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    [Route("api/People")]
    [ApiController]
    public class PeopleController : Controller
    {
        private ApplicationDbContext _context;
        private IMapper _mapper;

        public PeopleController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonDto>>> Get([FromQuery]  PaginationDTO pagination)
        {
            var queryable = _context.Persons.AsQueryable();
            await HttpContext.InsertPaginationParametersInRespnse(queryable, pagination.RecordsPerPage);
            var persons = await queryable.Paginate(pagination).ToListAsync();
            return _mapper.Map<List<PersonDto>>(persons);
            //  return personsDto;
        }

        [HttpGet("{id}", Name = "GetPerson")]
        public async Task<ActionResult<PersonDto>> Get(int id)
        {
            var person = await _context.Persons.FirstOrDefaultAsync(x => x.Id == id);
            PersonDto personDto = _mapper.Map<PersonDto>(person);
            return personDto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PersonCreationDto personCreationDto)
        {
            Person person = _mapper.Map<Person>(personCreationDto);
            string dbpath = "";
            if (personCreationDto.Picture != null)
                if (FileHelper.CheckIfImageFile(personCreationDto.Picture))
                {
                    dbpath = await FileHelper.WriteFile(personCreationDto.Picture);
                }
                else
                {
                    return BadRequest(new { message = "Invalid file extension" });
                }
            person.Name = personCreationDto.Name;
            person.Biography = personCreationDto.Biography;
            person.DateOfBirth = personCreationDto.DateOfBirth;
            person.Picture = dbpath;
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
            PersonDto personDto = _mapper.Map<PersonDto>(person);
            return new CreatedAtRouteResult("GetPerson", new Person { Id = person.Id }, personDto);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PersonPatchDTO> patchDocument)
        {
            if (patchDocument == null) { return BadRequest(); }
            var entityDBPerson = await _context.Persons.FirstOrDefaultAsync(p => p.Id == id);
            if (entityDBPerson == null) { return NotFound(); }
            var entityDTO = _mapper.Map<PersonPatchDTO>(entityDBPerson);
            patchDocument.ApplyTo(entityDTO, ModelState);
            var isVaid = TryValidateModel(entityDTO);
            if (!isVaid)
            {
                return BadRequest(ModelState);
            }
             _mapper.Map(entityDTO, entityDBPerson);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePerson(int id, [FromForm] PersonCreationDto personCreationDto)
        {
            var personDb = await _context.Persons.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (personDb == null)
            {
                return NotFound();
            }
            else
            {
                if (personCreationDto.Picture != null)
                {
                    FileHelper.DeleteFile(personDb.Picture);
                    personDb.Picture = await FileHelper.WriteFile(personCreationDto.Picture);
                }
                Person person = _mapper.Map<Person>(personCreationDto);
                person.Id = id;
                person.Picture = personDb.Picture;
                _context.Entry(person).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                PersonDto personDto = _mapper.Map<PersonDto>(person);
                return new CreatedAtRouteResult("GetPerson", new Person { Id = person.Id }, personDto);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePerson(int id)
        {
            Person person = await _context.Persons.FirstOrDefaultAsync(x => x.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            try
            {
                if (person.Picture != null)
                {
                    var imageToBeDelete = Path.Combine(Directory.GetCurrentDirectory(), person.Picture);
                    FileHelper.DeleteFile(person.Picture);
                }
            }
            catch (Exception e)
            {
            }
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
