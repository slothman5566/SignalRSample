using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SignalRServer
{
    public class JwtHandler : IJwtHandler
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly int _expiryInSeconds;

        public JwtHandler(string secretKey, string issuer, int expiryInSeconds)
        {
            _secretKey = secretKey;
            _expiryInSeconds = expiryInSeconds;
            _issuer = issuer;
        }

        public string GenerateToken(string userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            };

            var token = new JwtSecurityToken(
                _issuer, _issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(_expiryInSeconds),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
