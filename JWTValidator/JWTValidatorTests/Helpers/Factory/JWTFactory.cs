using JWTValidatorTests.Helpers.Builder;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace JWTValidatorTests.Helpers.Factory;

public class JWTFactory : IJWTFactory
{
    public String GenerateToken(JWTOptions jwtOptions)
    {
        SecurityTokenDescriptor tokenDescriptor = CreateTokenDetails(jwtOptions);
        return CreateToken(tokenDescriptor);
    }

    private SecurityTokenDescriptor CreateTokenDetails(JWTOptions jwtOptions)
    {
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Secret));

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GetClaims(jwtOptions),
            Expires = jwtOptions.ExpiryDate,
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        return tokenDescriptor;
    }

    private ClaimsIdentity GetClaims(JWTOptions jwtOptions)
    {
        if (jwtOptions.Claims is null)
        {
            return new ClaimsIdentity();
        }

        return new ClaimsIdentity(
            jwtOptions.Claims
            .Select(c => new Claim(c.Key, c.Value))
            .ToArray());
    }

    private String CreateToken(SecurityTokenDescriptor tokenDescriptor)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}