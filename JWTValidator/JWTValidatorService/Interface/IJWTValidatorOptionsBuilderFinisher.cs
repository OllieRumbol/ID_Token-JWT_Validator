using JWTValidatorService.Models;

namespace JWTValidatorService.Interface;

public interface IJWTValidatorOptionsBuilderFinisher
{
    IJWTValidatorOptionsBuilderFinisher WithAudience(String audience);

    IJWTValidatorOptionsBuilderFinisher WithIssuer(String issuer);

    IJWTValidatorOptionsBuilderFinisher WithExpiryDate();

    JWTValidatorOptions Build();
}
