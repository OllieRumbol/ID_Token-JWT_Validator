using JWTValidatorService.Models;

namespace JWTValidatorService.Builder;

public interface IJWTValidatorOptionsBuilderFinisher
{
    IJWTValidatorOptionsBuilderFinisher WithAudience(string audience);

    IJWTValidatorOptionsBuilderFinisher WithIssuer(string issuer);

    IJWTValidatorOptionsBuilderFinisher WithExpiryDate();

    JWTValidatorOptions Build();
}