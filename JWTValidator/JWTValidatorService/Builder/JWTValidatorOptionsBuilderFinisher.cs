using JWTValidatorService.Models;

namespace JWTValidatorService.Builder;

public class JWTValidatorOptionsBuilderFinisher : IJWTValidatorOptionsBuilderFinisher
{
    private IJWTValidatorOptionsBuilder _jWTValidatorOptionsBuilder;

    internal JWTValidatorOptionsBuilderFinisher(IJWTValidatorOptionsBuilder jWTValidatorOptionsBuilder) => _jWTValidatorOptionsBuilder = jWTValidatorOptionsBuilder;

    public IJWTValidatorOptionsBuilderFinisher WithAudience(string audience)
    {
        _jWTValidatorOptionsBuilder.WithAudience(audience);
        return this;
    }

    public IJWTValidatorOptionsBuilderFinisher WithIssuer(string issuer)
    {
        _jWTValidatorOptionsBuilder.WithIssuer(issuer);
        return this;
    }

    public IJWTValidatorOptionsBuilderFinisher WithExpiryDate()
    {
        _jWTValidatorOptionsBuilder.WithExpiryDate();
        return this;
    }

    public JWTValidatorOptions Build() => _jWTValidatorOptionsBuilder.Build();
}