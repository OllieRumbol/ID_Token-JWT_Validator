namespace JWTValidatorService.Builder;

public class JWTValidatorOptionsBuilderCreator : IJWTValidatorOptionsBuilderCreator
{
    private IJWTValidatorOptionsBuilder JwtValidatorOptionsBuilder;

    private JWTValidatorOptionsBuilderCreator() => JwtValidatorOptionsBuilder = new JWTValidatorOptions.JWTValidatorOptionsBuilder();

    public static JWTValidatorOptionsBuilderCreator Create() => new JWTValidatorOptionsBuilderCreator();

    public IJWTValidatorOptionsBuilderFinisher WithSigningKeyFromSecret(String secret)
    {
        if (String.IsNullOrEmpty(secret))
        {
            throw new ArgumentNullException(nameof(secret));
        }

        JwtValidatorOptionsBuilder.WithSigningKeyFromSecret(secret);
        return new JWTValidatorOptionsBuilderFinisher(JwtValidatorOptionsBuilder);
    }

    public IJWTValidatorOptionsBuilderFinisher WithSigningKeyFromOpenIdUrl(String openIdUrl)
    {
        if (String.IsNullOrEmpty(openIdUrl))
        {
            throw new ArgumentNullException(nameof(openIdUrl));
        }

        JwtValidatorOptionsBuilder.WithSigningKeyFromOpenIdUrl(openIdUrl);
        return new JWTValidatorOptionsBuilderFinisher(JwtValidatorOptionsBuilder);
    }
}