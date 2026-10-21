using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using JWTValidatorTests.Helpers.Builder;

namespace JWTValidatorTests.Helpers.Factory;

public interface IJWTFactory
{
    String GenerateToken(JWTOptions jwtOptions);

    String GenerateToken(String secret, Dictionary<String, Object> claims, DateTime issuedAt, DateTime notBefore, DateTime expiryDate);

    String GenerateToken(RSA rsa, String keyId, Dictionary<String, Object> claims, DateTime issuedAt, DateTime notBefore, DateTime expiryDate);

    String SignToken(String header, String payload, String secret);
}