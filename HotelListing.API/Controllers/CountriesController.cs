using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Core.Models.Country;
using HotelListing.API.Core.Models;
using HotelListing.API.Core.Exceptions;

namespace HotelListing.API.Controllers
{
    // previo a las versiones
    //[Route("api/[controller]")]
    //[ApiController]
    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    public class CountriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(IMapper mapper, 
            ICountriesRepository countriesRepository,ILogger<CountriesController> logger)
        {
            this._mapper = mapper;
            this._countriesRepository = countriesRepository;
            this._logger = logger;
        }

        /////////////////////////////////
        /////////////////////////////////
        // GET: api/Countries/GetAll
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            //public async Task<List<T>> GetAllAsync()
            //    return await _context.Set<T>().ToListAsync();

            var countries = await _countriesRepository.GetAllAsync();
            var records = _mapper.Map<List<GetCountryDto>>(countries);

            return Ok(records);
        }

        /////////////////////////////////
        /////////////////////////////////
        ///PAGING
        // GET: api/Countries/?StartIndex=0&PageSize=25&PageNumber=1
        [HttpGet]
        public async Task<ActionResult<PagedResults<GetCountryDto>>> GetPagedCountries([FromQuery] QueryParameters queryParameters)
        {
            var pagedCountriesResults = await _countriesRepository.GetAllAsync<GetCountryDto>(queryParameters);
            //var records = _mapper.Map<List<GetCountryDto>>(countries);

            return Ok(pagedCountriesResults);
        }

        /////////////////////////////////
        /////////////////////////////////
        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            // para no modificar el generico, qes de donde se saca todo el resto, mejor 
            // sobreescribo elgenerico en ICountriesRepository y xende en CountriesRepository
            var country = await _countriesRepository.GetDetails(id);

            if (country == null)
                throw new NotFoundException(nameof(GetCountry),id);

            var countryDto = _mapper.Map<CountryDto>(country);

            return countryDto;
        }

        /////////////////////////////////
        // PUT: api/Countries/5
        [HttpPut("{id}")]
        [Authorize] // cualquier usuario pero autorizado
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id)
            {
                return BadRequest();
            }

            var country = await _countriesRepository.GetAsync(id);

            if (country == null)
                throw new NotFoundException(nameof(PutCountry), id);

            // usa la data de updatedCountryDto para editar country, y ya 
            _mapper.Map(updateCountryDto, country);

            try
            {
                //await _context.SaveChangesAsync();
                await _countriesRepository.UpdateAsync(country);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
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
        [Authorize]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
        {
            //me mapea un createCountryDto en un object de tipo Country
            var country = _mapper.Map<Country>(createCountryDto);

            await _countriesRepository.AddAsync(country);

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        /////////////////////////////////
        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")] // solo admins logeados
        // en el token estan los reles del usuario
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _countriesRepository.GetAsync(id);

            // con el exceptionMiddleware puedo tirar las exceptions y alla las agarro
            // sin que todo crashee
            if (country == null)
                throw new NotFoundException(nameof(DeleteCountry), id);

            await _countriesRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            return await _countriesRepository.Exist(id);
        }
    }
}

//
//
//ActionResult < T > return type for web API controller actions.
//
//

/////////////////////////
///
/// Previo REPOSITORY
///
/////////////////////////

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using HotelListing.API.Data;
//using HotelListing.API.Models.Country;
//using AutoMapper;

//// hay que evitar mandar o recibir data models ( Country o Hotels ) y para ello se ocupan 
//// los data transfer objects, que va a ser simplemente un modelo solo con algunas props
//// de las q tiene el q se ocupa con la DB, van a estar en la subcarpeta de la carpeta
//// Models, estos son los " data transfer objects ( dto ) "
//// tambien NO es recomendable interactuar directamente con DbContext

//namespace HotelListing.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CountriesController : ControllerBase
//    {
//        private readonly HotelListingDbContext _context;
//        private readonly IMapper _mapper;

//        // ( injecta el context en el controller )
//        // como se registro el DbContext as part of our services, en Program.cs 
//        // builder.Services.AddDbContext<Hote... , => ahora lo podemos injectar en
//        // cualquier file o parte del programa. De esta forma ahora puedo interactuar
//        // con la db desde aca a traves de _context, asi no tengo q declarar una nueva instancia de dbcontext cada vez q tengo una nueva clase
//        public CountriesController(HotelListingDbContext context, IMapper mapper)
//        {
//            _context = context;
//            this._mapper = mapper;
//        }

//        /////////////////////////////////
//        // GET: api/Countries
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
//        {
//            //                     Select * form Countries
//            var countries = await _context.Countries.ToListAsync();

//            // no puedo mapear una collection a un object
//            //var records = _mapper.Map<GetCountryDto>(countries);
//            var records = _mapper.Map<List<GetCountryDto>>(countries);

//            return Ok(records);
//        }

//        /////////////////////////////////
//        // GET: api/Countries/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<CountryDto>> GetCountry(int id)
//        {
//            // me devuelve null en hotels, asi que hay q ocupar eager-loading
//            // var country = await _context.Countries.FindAsync(id);
//            var country = await _context.Countries.Include(c => c.Hotels)
//                .FirstOrDefaultAsync(c => c.Id == id);


//            if (country == null)
//            {
//                return NotFound();
//            }

//            var countryDto = _mapper.Map<CountryDto>(country);

//            return countryDto;
//        }

//        /////////////////////////////////
//        // PUT: api/Countries/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
//        {
//            if (id != updateCountryDto.Id)
//            {
//                return BadRequest();
//            }

//            //_context.Entry(country).State = EntityState.Modified;

//            var country = await _context.Countries.FindAsync(id);

//            if (country == null)
//            {
//                return NotFound();
//            }

//            // usa la data de updatedCountryDto para editar country, y ya 
//            // abajo se guardan los cambios
//            _mapper.Map(updateCountryDto, country);

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!CountryExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        /////////////////////////////////
//        // POST: api/Countries
//        [HttpPost]
//        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
//        {
//            //var country = new Country
//            //{
//            //    // para evitar este mapeo de forma manual es q se ocupa AutoMapper, se ocupa con dependency injection
//            //    Name = createCountryDto.Name,
//            //    ShortName = createCountryDto.ShortName,
//            //};

//            //me mapea un createCountryDto en un object de tipo Country
//            var country = _mapper.Map<Country>(createCountryDto);

//            _context.Countries.Add(country);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
//        }

//        /////////////////////////////////
//        // DELETE: api/Countries/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteCountry(int id)
//        {
//            var country = await _context.Countries.FindAsync(id);
//            if (country == null)
//            {
//                return NotFound();
//            }

//            _context.Countries.Remove(country);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool CountryExists(int id)
//        {
//            return _context.Countries.Any(e => e.Id == id);
//        }
//    }
//}
