using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key; //a type of encryption where only one key is used to encrypt and decrypt
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };
            //all the claims or attributes about the user

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature); //sign the token
            //Represents the cryptographic key and security algorithms that are used to generate a digital signature.

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credentials    //the signature signed by using the TokenKey
            };
            //SecurityTokenDescriptor: a place holder for all the attributes related to the issued token.

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

            //TokenKey => _key => credentials => tokenDescriptor(credentials)   => token = tokenHandler(tokenDescriptor)
            //user.UserName => claim          => tokenDescriptor(claim)
        }
    }
}