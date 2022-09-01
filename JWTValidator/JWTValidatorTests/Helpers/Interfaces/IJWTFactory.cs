using System;
using JWTValidatorTests.Helpers.Models;

namespace JWTValidatorTests.Helpers.Interfaces;

public interface IJWTFactory
{
    String GenerateToken(JWTOptions jWTOptions);
}