using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using AutoMapper;

// hay que evitar mandar o recibir data models ( Country o Hotels ) y para ello se ocupan 
// los data transfer objects, que va a ser simplemente un modelo solo con algunas props
// de las q tiene el q se ocupa con la DB, van a estar en la subcarpeta de la carpeta
// Models, estos son los "data transfer objects ( dto ) "
// tambien NO es recomendable interactuar directamente con DbContext

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        // ( injecta el context en el controller )
        // como se registro el DbContext as part of our services, en Program.cs 
        // builder.Services.AddDbContext<Hote... , => ahora lo podemos injectar en cualquier
        // file o parte del programa.
        // de esta forma ahora puedo interactuar con la db desde aca a traves de _context, asi no tengo q declarar una nueva instancia de dbcontext cada vez q tengo una nueva clase
        public CountriesController(HotelListingDbContext context, IMapper mapper)
        {
            _context = context;
            this._mapper = mapper;
        }

        /////////////////////////////////
        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            //             Select * form Countries
            var countries = await _context.Countries.ToListAsync();

            // no puedo mapear una collection a un object
            //var records = _mapper.Map<GetCountryDto>(countries);
            var records = _mapper.Map<List<GetCountryDto>>(countries);
                        
            return Ok(records);
        }

        /////////////////////////////////
        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            // me devuelve null en hotels, asi que hay q ocupar eager-loading
            //var country = await _context.Countries.FindAsync(id);
            var country = await _context.Countries.Include(c => c.Hotels)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (country == null)
            {
                return NotFound();
            }

            var countryDto = _mapper.Map<CountryDto>(country);

            return countryDto;
        }

        /////////////////////////////////
        // PUT: api/Countries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, Country country)
        {
            if (id != country.Id)
            {
                return BadRequest();
            }

            _context.Entry(country).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /////////////////////////////////
        // POST: api/Countries
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
        {
            //var country = new Country
            //{
            //    // para evitar este mapeo de forma manual es q se ocupa AutoMapper, se ocupa con dependency injection
            //    Name = createCountryDto.Name,
            //    ShortName = createCountryDto.ShortName,
            //};

            //me mapea un createCountryDto en un object de tipo Country
            var country = _mapper.Map<Country>(createCountryDto);

            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        /////////////////////////////////
        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}
