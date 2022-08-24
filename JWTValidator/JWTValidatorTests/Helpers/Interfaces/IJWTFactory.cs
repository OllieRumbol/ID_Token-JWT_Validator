using System;
using JWTValidatorTests.Helpers.Models;

namespace JWTValidatorTests.Helpers.Interfaces;

internal interface IJWTFactory
{
    String GenerateToken(JWTOptions jWTOptions);
}