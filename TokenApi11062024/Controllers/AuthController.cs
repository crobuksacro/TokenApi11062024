using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokenApi11062024.Model.Binding;
using TokenApi11062024.Model.ViewModel;
using TokenApi11062024.Model;
using TokenApi11062024.Services.Interfaces;

namespace TokenApi11062024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.Admin)]
    public class AuthController : ControllerBase
    {
        private IAccountService accountService;


        public AuthController(IAccountService accountService)
        {
            this.accountService = accountService;

        }
        /// <summary>
        /// Authenticates a user and generates an access token and a refresh token.
        /// </summary>
        /// <param name="model">The login binding model containing the user's credentials.</param>
        /// <returns>
        /// A Task resulting in a <see cref="TokenViewModel"/> containing the access and refresh tokens for the user. Returns null if authentication fails.
        /// </returns>
        /// <remarks>
        /// This method attempts to authenticate a user using their username and password. If authentication succeeds, it generates a JWT access token and a refresh token for the user. The refresh token is stored in the user's record along with its expiry time, and both tokens are returned in a TokenViewModel. If authentication fails, the method returns null.
        /// </remarks>
        [AllowAnonymous]
        [Route("token")]
        [HttpPost]
        [ProducesResponseType(typeof(TokenViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Token([FromBody] LoginBinding model)
        {
            if (ModelState.IsValid)
            {
                var token = await accountService.GetToken(model);
                if (token == null)
                {
                    return BadRequest(new
                    {
                        Msg = "Invalid username or password!",
                    });
                }
                return Ok(token);
            }

            return BadRequest();
        }

        [HttpGet("test")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetExamsForCurrentUser()
        {
            return Ok(new { msg = "done" });
        }



    }
}
