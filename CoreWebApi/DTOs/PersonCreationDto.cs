using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.DTOs
{
    public class PersonCreationDto: PersonPatchDTO
    {        
        public IFormFile Picture { get; set; }
    }
}
