using JWTValidatorTests.Helpers.Builder;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

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
        return GenerateToken(CreateSigningCredentials(secret), claims, issuedAt, notBefore, expiryDate);
    }

    // Signs with RS256 and puts the key ID in the header, as an OpenID provider does.
    public String GenerateToken(RSA rsa, String keyId, Dictionary<String, Object> claims, DateTime issuedAt, DateTime notBefore, DateTime expiryDate)
    {
        RsaSecurityKey securityKey = new RsaSecurityKey(rsa) { KeyId = keyId };
        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

        return GenerateToken(signingCredentials, claims, issuedAt, notBefore, expiryDate);
    }

    // Signs a token by hand, for a token that the token handler cannot create,
    // such as a token with no exp, one with an exp later than a DateTime can hold,
    // or one signed with a key shorter than the algorithm's minimum.
    // The "alg" in the header selects the algorithm: HS256 or HS512.
    public String SignToken(String header, String payload, String secret)
    {
        String encodedHeader = Base64UrlEncoder.Encode(header);
        String encodedPayload = Base64UrlEncoder.Encode(payload);
        Byte[] dataToSign = Encoding.ASCII.GetBytes($"{encodedHeader}.{encodedPayload}");

        // The library reads the secret as UTF-8, so the key here must be the same bytes.
        Byte[] key = Encoding.UTF8.GetBytes(secret);
        String? algorithm = GetAlgorithm(header);
        Byte[] signature = algorithm switch
        {
            "HS256" => HMACSHA256.HashData(key, dataToSign),
            "HS512" => HMACSHA512.HashData(key, dataToSign),
            _ => throw new ArgumentException($"Unsupported algorithm '{algorithm}'. Use HS256 or HS512.", nameof(header))
        };

        return $"{encodedHeader}.{encodedPayload}.{Base64UrlEncoder.Encode(signature)}";
    }

    private static String? GetAlgorithm(String header)
    {
        using JsonDocument headerDocument = JsonDocument.Parse(header);
        return headerDocument.RootElement.GetProperty("alg").GetString();
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

    private String GenerateToken(SigningCredentials signingCredentials, Dictionary<String, Object> claims, DateTime issuedAt, DateTime notBefore, DateTime expiryDate)
    {
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            IssuedAt = issuedAt,
            NotBefore = notBefore,
            Expires = expiryDate,
            SigningCredentials = signingCredentials
        };

        return CreateToken(tokenDescriptor);
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