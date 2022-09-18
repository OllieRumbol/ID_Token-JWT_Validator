using JWTValidatorService.Interface;
using JWTValidatorService.Models;

namespace JWTValidatorService.Insistence;

public class JWTValidatorOptionsBuilder: IJWTValidatorOptionsBuilder
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

    public void WithAudience(String audience)
    {
        jWTValidatorOptions.Audience = audience;
    }

    public void WithIssuer(String issuer)
    {
        jWTValidatorOptions.Issuer = issuer;
    }

    public void WithSigningKeyFromOpenIdUrl(String openIdUrl)
    {
        jWTValidatorOptions.OpenIdUrl = openIdUrl;
    }

    public void WithSigningKeyFromSecret(String secret)
    {
        jWTValidatorOptions.Secret = secret;
    }

    public void WithExpiryDate()
    {
        jWTValidatorOptions.ExpiryDate = true;
    }
}