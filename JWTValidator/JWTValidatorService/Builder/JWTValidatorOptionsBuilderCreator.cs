namespace JWTValidatorService.Builder;

public class JWTValidatorOptionsBuilderCreator : IJWTValidatorOptionsBuilderCreator
{
    private IJWTValidatorOptionsBuilder _jWTValidatorOptionsBuilder;

    private JWTValidatorOptionsBuilderCreator() => _jWTValidatorOptionsBuilder = new JWTValidatorOptions.JWTValidatorOptionsBuilder();

    public static JWTValidatorOptionsBuilderCreator Create() => new JWTValidatorOptionsBuilderCreator();

    public IJWTValidatorOptionsBuilderFinisher WithSigningKeyFromSecret(String secret)
    {
        if (String.IsNullOrEmpty(secret))
        {
            throw new ArgumentNullException(nameof(secret));
        }

        _jWTValidatorOptionsBuilder.WithSigningKeyFromSecret(secret);
        return new JWTValidatorOptionsBuilderFinisher(_jWTValidatorOptionsBuilder);
    }

    public IJWTValidatorOptionsBuilderFinisher WithSigningKeyFromOpenIdUrl(String openIdUrl)
    {
        if (String.IsNullOrEmpty(openIdUrl))
        {
            throw new ArgumentNullException(nameof(openIdUrl));
        }

        _jWTValidatorOptionsBuilder.WithSigningKeyFromOpenIdUrl(openIdUrl);
        return new JWTValidatorOptionsBuilderFinisher(_jWTValidatorOptionsBuilder);
    }
}