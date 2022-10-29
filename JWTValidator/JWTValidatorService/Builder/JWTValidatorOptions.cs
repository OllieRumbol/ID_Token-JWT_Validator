namespace JWTValidatorService.Builder;

public class JWTValidatorOptions
{
    public String Secret { get; private set; } = "";

    public String OpenIdUrl { get; private set; } = "";

    public String Issuer { get; private set; } = "";

    public String Audience { get; private set; } = "";

    public Boolean ExpiryDate { get; private set; }

    public String Print()
    {
        return
            $"Secret: {(String.IsNullOrEmpty(Secret) ? "None" : Secret)}\n" +
            $"OpenIdUrl: {(String.IsNullOrEmpty(OpenIdUrl) ? "None" : OpenIdUrl)}\n" +
            $"Issuer: {(String.IsNullOrEmpty(Issuer) ? "None" : Issuer)}\n" +
            $"Audience: {(String.IsNullOrEmpty(Audience) ? "None" : Audience)}\n" +
            $"Validate expiry date: {ExpiryDate}\n";
    }

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
}