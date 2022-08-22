using HotelListing.API.Contracts;
using HotelListing.API.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// para rastrear con el logger lo que pasa en un archivo, lo meto x dependency injection
// aca para ver lo q pasa al logearse

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public readonly IAuthManager _authManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthManager authManager, ILogger<AccountController> logger)
        {
            this._authManager = authManager;
            this._logger = logger;
        }

        //POST: api/Account/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] ApiUserDto apiUserDto)
        {
            // para logear cuando alguien intenta registrarse
            _logger.LogInformation($"Registration Attempt for {apiUserDto.Email}");

            var errors = await _authManager.Register(apiUserDto);

            // el ModelState es donde estan guardados esos errores q se manejan xdefault
            // como cundo tengo nombre [Required] y no lo pongo y me manda el error de 
            // q tengo q ponerlo
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                // mando el bad request con los errores que agregue, q van a venir del 
                // AuthManager.cs q es el que tiene la logica del business
                return BadRequest(ModelState);
            }

            return Ok();
        }

        //POST: api/Account/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation($"Login attempt for {loginDto.Email}");

            var authResponse = await _authManager.Login(loginDto);
            if (authResponse == null)
                return Unauthorized();

            // en authResponse esta el token y userId
            return Ok(authResponse);
        }

        //POST: api/Account/refreshToken
        [HttpPost]
        [Route("refreshToken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RefreshToken([FromBody] AuthResponseDto request)
        {
            var authResponse = await _authManager.VerifyRefreshToken(request);

            if (authResponse == null)
            {
                return Unauthorized();
            }

            // en authResponse esta el token, userId y refreshToken
            return Ok(authResponse);
        }

    }
}
