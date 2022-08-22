using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Models.Hotel;
using Microsoft.AspNetCore.Authorization;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HotelsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHotelsRepository _hotelsRepository;
        
        public HotelsController(IMapper mapper, IHotelsRepository hotelsRepository)
        {
            this._mapper = mapper;
            this._hotelsRepository = hotelsRepository;
        }

        /////////////////////////////////
        ////////////////////////////////////
        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
        {
            var hotels = await _hotelsRepository.GetAllAsync();
            var records = _mapper.Map<List<HotelDto>>(hotels);

            return records;
        }

        /////////////////////////////////
        ////////////////////////////////////
        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var hotel = await _hotelsRepository.GetAsync(id);

            if (hotel == null)
            {
                return NotFound();
            }

            var hotelDto = _mapper.Map<HotelDto>(hotel);

            return hotelDto;
        }

        /////////////////////////////////
        ////////////////////////////////////
        // PUT: api/Hotels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, HotelDto hotelDto)
        {
            if (id != hotelDto.Id)
                return BadRequest();

            var hotel = await _hotelsRepository.GetAsync(id);
            if(hotel == null)
                return NotFound();

            _mapper.Map(hotelDto, hotel);

            try
            {
                await _hotelsRepository.UpdateAsync(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelExists(id))
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
        ////////////////////////////////////
        // POST: api/Hotels
        [HttpPost]
        public async Task<ActionResult<CreateHotelDto>> PostHotel(CreateHotelDto createHotelDto)
        {

            var hotel = _mapper.Map<Hotel>(createHotelDto);
            await _hotelsRepository.AddAsync(hotel);

            // puedo crear un dto para mandar como respuesta el lugar de hotel, para q 
            // no me ponga el Country null o poner en el HotelsRepository un GetDetails
            // como con CountriesRepository para q mande el dato.
            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        /////////////////////////////////
        ////////////////////////////////////
        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _hotelsRepository.GetAsync(id);

            if (hotel == null)
                return NotFound();

            await _hotelsRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotelsRepository.Exist(id);
        }
    }
}
