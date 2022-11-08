using System;

namespace JWTValidatorTests.Helpers.Builder;

public interface IJWTOptionsBuilderCreator
{
    IJWTOptionsBuilderFinisher WithSecret(String Secret);
}