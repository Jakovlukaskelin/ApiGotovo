using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SuperStore_api.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SuperStore_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private const string SecretKey = "1234567891dwddwqdqwdqewdqewdewdewfewgfefewfewfew011";
        private const string Issuer = "jale";

       
        private static readonly Dictionary<string, string> refreshTokens = new Dictionary<string, string>();

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] User user)
        {
            if (user.UserName == "user" && user.Password == "password")
            {
                var accessToken = GenerateJwtToken(user.UserName);
                var refreshToken = Guid.NewGuid().ToString();
                
                refreshTokens[user.UserName] = refreshToken;

                return Ok(new { accessToken, refreshToken });
            }

            return Unauthorized();
        }

        [HttpPost, Route("refresh")]
        public IActionResult Refresh([FromBody] RefreshTokenRequest request)
        {
            var refreshToken = request.RefreshToken;
            // nadi username na temelju refresh token
            var user = refreshTokens.FirstOrDefault(x => x.Value == refreshToken).Key;
            if (user == null)
            {
                return Unauthorized();
            }

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = Guid.NewGuid().ToString();
            refreshTokens[user] = newRefreshToken; // Update the refresh token

            return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Issuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
