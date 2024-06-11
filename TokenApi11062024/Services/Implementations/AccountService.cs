using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TokenApi11062024.Context;
using TokenApi11062024.Model;
using TokenApi11062024.Model.Binding;
using TokenApi11062024.Model.Dbo;
using TokenApi11062024.Model.ViewModel;
using TokenApi11062024.Services.Interfaces;

namespace TokenApi11062024.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private AppSettings appSettings;

        public AccountService(IMapper mapper, ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IOptions<AppSettings> appSettings)
        {
            this.mapper = mapper;
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.appSettings = appSettings.Value;
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
        public async Task<TokenViewModel> GetToken(LoginBinding model)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);
            if (user == null)
            {
                return null;
            }

            var signInResult = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (signInResult.Succeeded)
            {
                var role = await userManager.GetRolesAsync(user);
                if (user != null)
                {
                    DateTime expires = DateTime.Now.AddYears(30);
                    var token = await CreateToken(user);
                    var refreshToken = GenerateRefreshToken();
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(appSettings.TokenValidityInMinutes);
                    await userManager.UpdateAsync(user);
                    return new TokenViewModel
                    {
                        AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                        RefreshToken = refreshToken,
                        UserId = user.Id
                    };
                }
            }
            return null;
        }
        /// <summary>
        /// Generates a random string to be used as a refresh token.
        /// </summary>
        /// <returns>
        /// A string representing the generated refresh token.
        /// </returns>
        /// <remarks>
        /// This method generates a refresh token by creating a random 64-byte number and converting it to a Base64 string. This token is used to securely refresh authentication tokens when they expire.
        /// </remarks>
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private async Task<JwtSecurityToken> CreateToken(ApplicationUser user)
        {
            var role = await userManager.GetRolesAsync(user);
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Key));


            DateTime expires = DateTime.Now.AddMinutes(appSettings.TokenValidityInMinutes);


            var token = new JwtSecurityToken(
                issuer: appSettings.Jwt.Issuer,
                audience: appSettings.Jwt.Audience,
                expires: DateTime.Now.AddMinutes(appSettings.TokenValidityInMinutes),
                claims: new List<Claim>
                {
                                    new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("PhoneNumber", string.IsNullOrWhiteSpace(user.PhoneNumber) ? string.Empty : user.PhoneNumber),
                new Claim(ClaimTypes.Role, role.First()),
                new Claim(ClaimTypes.Expiration, expires.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,user.Id)

                },
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)

                );

            return token;
        }


    }
}
