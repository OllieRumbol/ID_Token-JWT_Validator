using JWTValidatorTests.Helpers.Interfaces;
using JWTValidatorTests.Helpers.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace JWTValidatorTests.Helpers.Instances;

public class JWTFactory : IJWTFactory
{
    public String GenerateToken(JWTOptions jWTOptions)
    {
        SecurityTokenDescriptor tokenDescriptor = CreateTokenDetails(jWTOptions);
        String jwt = CreateToken(tokenDescriptor);
        return jwt;
    }

    private SecurityTokenDescriptor CreateTokenDetails(JWTOptions jWTOptions)
    {
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jWTOptions.Secret));

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GetClaims(jWTOptions),
            Expires = jWTOptions.ExpiryDate,
            Issuer = jWTOptions.Issuer,
            Audience = jWTOptions.Audience,
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        return tokenDescriptor;
    }

    private ClaimsIdentity GetClaims(JWTOptions jWTOptions)
    {
        return new ClaimsIdentity(
            jWTOptions.Claims
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