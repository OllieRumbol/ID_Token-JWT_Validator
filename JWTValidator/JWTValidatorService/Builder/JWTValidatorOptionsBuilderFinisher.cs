namespace JWTValidatorService.Builder;

public class JWTValidatorOptionsBuilderFinisher : IJWTValidatorOptionsBuilderFinisher
{
    private IJWTValidatorOptionsBuilder _jWTValidatorOptionsBuilder;

    internal JWTValidatorOptionsBuilderFinisher(IJWTValidatorOptionsBuilder jWTValidatorOptionsBuilder) => _jWTValidatorOptionsBuilder = jWTValidatorOptionsBuilder;

    public IJWTValidatorOptionsBuilderFinisher WithAudience(String audience)
    {
        if (String.IsNullOrEmpty(audience))
        {
            throw new ArgumentNullException(nameof(audience));
        }

        _jWTValidatorOptionsBuilder.WithAudience(audience);
        return this;
    }

    public IJWTValidatorOptionsBuilderFinisher WithIssuer(String issuer)
    {
        if (String.IsNullOrEmpty(issuer))
        {
            throw new ArgumentNullException(nameof(issuer));
        }

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