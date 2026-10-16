using System;
using System.Collections.Generic;
using JWTValidatorTests.Helpers.Builder;

namespace JWTValidatorTests.Helpers.Factory;

public interface IJWTFactory
{
    String GenerateToken(JWTOptions jwtOptions);

    String GenerateToken(String secret, Dictionary<String, Object> claims, DateTime issuedAt, DateTime notBefore, DateTime expiryDate);
}