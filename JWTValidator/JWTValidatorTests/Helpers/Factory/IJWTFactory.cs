using System;
using JWTValidatorTests.Helpers.Builder;

namespace JWTValidatorTests.Helpers.Factory;

public interface IJWTFactory
{
    String GenerateToken(JWTOptions jwtOptions);
}