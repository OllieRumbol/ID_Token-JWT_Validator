using JWTValidatorService.Models;

namespace JWTValidatorService.Builder;

public interface IJWTValidatorOptionsBuilder
{
    void WithSigningKeyFromSecret(string secret);

    void WithSigningKeyFromOpenIdUrl(string openIdUrl);

    void WithIssuer(string issuer);

    void WithAudience(string audience);

    void WithExpiryDate();

    JWTValidatorOptions Build();
}