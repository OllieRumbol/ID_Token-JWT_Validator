namespace JWTValidatorService.Builder;

public interface IJWTValidatorOptionsBuilder
{
    void WithSigningKeyFromSecret(String secret);

    void WithSigningKeyFromOpenIdUrl(String openIdUrl);

    void WithIssuer(String issuer);

    void WithAudience(String audience);

    void WithExpiryDate();

    JWTValidatorOptions Build();
}