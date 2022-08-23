using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;
        public GenericRepository(HotelListingDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        // en el controller original antes de repository
        //public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
        //{
        //    var country = _mapper.Map<Country>(createCountryDto);
        //    _context.Countries.Add(country);
        //    await _context.SaveChangesAsync();
        //    return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        //}
        public async Task<T> AddAsync(T entity)
        {
            // EF ( al ocupar AddAsync ) deduce la entity y su tipo (T) xlo q no es
            // necesario poner el .Countries en _context.Countries.Add(country);
            // deduce solo que entity es de tipo Country xlo q hace el cambio el 
            // la tabla .Countries
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        // previo en controller
        //private bool CountryExists(int id)
        //{
        //    return _context.Countries.Any(e => e.Id == id);
        //}
        public async Task<bool> Exist(int id)
        {
            var entity = await GetAsync(id);
            return entity != null;
        }

        // en controller antes de repository
        //public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        //{
        //    var countries = await _context.Countries.ToListAsync();
        //    var records = _mapper.Map<List<GetCountryDto>>(countries);

        //    return Ok(records);
        //}
        public async Task<List<T>> GetAllAsync()
        {
            // .Set<T> para q agarre el DbSet asociado a T, => si se le pasa un
            // tipo Country seria "_context.Countries.ToList...
            // pero como no lo sabe este es el generico

            return await _context.Set<T>().ToListAsync();
        }

        public async Task<PagedResults<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters)
        {
            // <T> representaria el modelo, <TResult> representaria el Dto
            // projectTo va a hacer q en el query q manda solo pida la info q se necesita
            // en el Dto ( es mas eficiente para la DB de esta forma ), en lugar de pedir
            // todo y aca pasarlo a Dto
            var totalSize = await _context.Set<T>().CountAsync();

            var items = await _context.Set<T>()
                .Skip(queryParameters.StartIndex)
                .Take(queryParameters.PageSize)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResults<TResult>
            {
                Items = items,
                PageNumber = queryParameters.PageNumber,
                RecordNumber = queryParameters.PageSize,
                TotalCount = totalSize
            };
        }

        // en controller antes de repository
        //public async Task<ActionResult<CountryDto>> GetCountry(int id)
        //{
        //    // var country = await _context.Countries.FindAsync(id);
        //    var country = await _context.Countries.Include(c => c.Hotels)
        //        .FirstOrDefaultAsync(c => c.Id == id);

        //    if (country == null)
        //    {
        //        return NotFound();
        //    }
        //    var countryDto = _mapper.Map<CountryDto>(country);
        //    return countryDto;
        //}
        public async Task<T> GetAsync(int? id)
        {
            if (id == null)
                return null;

            return await _context.Set<T>().FindAsync(id);            
        }

        // previo en controller
        //public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        //{
        //    if (id != updateCountryDto.Id)
        //        return BadRequest();

        //    var country = await _context.Countries.FindAsync(id);

        //    if (country == null)
        //        return NotFound();

        //    _mapper.Map(updateCountryDto, country);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch ...
        //}
        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
