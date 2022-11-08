namespace JWTValidatorService.Builder;

public class JWTValidatorOptionsBuilderFinisher : IJWTValidatorOptionsBuilderFinisher
{
    private IJWTValidatorOptionsBuilder JwtValidatorOptionsBuilder;

    internal JWTValidatorOptionsBuilderFinisher(IJWTValidatorOptionsBuilder jwtValidatorOptionsBuilder) => JwtValidatorOptionsBuilder = jwtValidatorOptionsBuilder;

    public IJWTValidatorOptionsBuilderFinisher WithAudience(String audience)
    {
        if (String.IsNullOrEmpty(audience))
        {
            throw new ArgumentNullException(nameof(audience));
        }

        JwtValidatorOptionsBuilder.WithAudience(audience);
        return this;
    }

    public IJWTValidatorOptionsBuilderFinisher WithIssuer(String issuer)
    {
        if (String.IsNullOrEmpty(issuer))
        {
            throw new ArgumentNullException(nameof(issuer));
        }

        JwtValidatorOptionsBuilder.WithIssuer(issuer);
        return this;
    }

    public IJWTValidatorOptionsBuilderFinisher WithExpiryDate()
    {
        JwtValidatorOptionsBuilder.WithExpiryDate();
        return this;
    }

    public JWTValidatorOptions Build() => JwtValidatorOptionsBuilder.Build();
}