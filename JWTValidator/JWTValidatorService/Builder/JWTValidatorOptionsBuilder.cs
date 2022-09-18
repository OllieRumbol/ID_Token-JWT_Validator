using JWTValidatorService.Models;

namespace JWTValidatorService.Builder;

internal class JWTValidatorOptionsBuilder : IJWTValidatorOptionsBuilder
{
    private JWTValidatorOptions jWTValidatorOptions;

    public JWTValidatorOptionsBuilder()
    {
        jWTValidatorOptions = new JWTValidatorOptions();
    }

    public JWTValidatorOptions Build()
    {
        return jWTValidatorOptions;
    }

    public void WithAudience(string audience)
    {
        jWTValidatorOptions.Audience = audience;
    }

    public void WithIssuer(string issuer)
    {
        jWTValidatorOptions.Issuer = issuer;
    }

    public void WithSigningKeyFromOpenIdUrl(string openIdUrl)
    {
        jWTValidatorOptions.OpenIdUrl = openIdUrl;
    }

    public void WithSigningKeyFromSecret(string secret)
    {
        jWTValidatorOptions.Secret = secret;
    }

    public void WithExpiryDate()
    {
        jWTValidatorOptions.ExpiryDate = true;
    }
}