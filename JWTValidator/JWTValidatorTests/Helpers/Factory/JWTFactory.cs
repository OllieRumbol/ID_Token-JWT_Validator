using JWTValidatorTests.Helpers.Builder;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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

    // The claims can hold any JSON type, such as a boolean, a number, an array or a nested object.
    // The fixed times keep the claims that tests expect the same on every run.
    public String GenerateToken(String secret, Dictionary<String, Object> claims, DateTime issuedAt, DateTime notBefore, DateTime expiryDate)
    {
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            IssuedAt = issuedAt,
            NotBefore = notBefore,
            Expires = expiryDate,
            SigningCredentials = CreateSigningCredentials(secret)
        };

        return CreateToken(tokenDescriptor);
    }

    private SecurityTokenDescriptor CreateTokenDetails(JWTOptions jwtOptions)
    {
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GetClaims(jwtOptions),
            Expires = jwtOptions.ExpiryDate,
            NotBefore = jwtOptions.NotBefore,
            IssuedAt = jwtOptions.IssuedAt,
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            SigningCredentials = CreateSigningCredentials(jwtOptions.Secret)
        };

        return tokenDescriptor;
    }

    private SigningCredentials CreateSigningCredentials(String secret)
    {
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
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