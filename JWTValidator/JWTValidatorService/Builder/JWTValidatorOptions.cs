namespace JWTValidatorService.Builder;

public class JWTValidatorOptions
{
    public String Secret { get; private set; } = default!;

    public String OpenIdUrl { get; private set; } = default!;

    public String Issuer { get; private set; } = default!;

    public String Audience { get; private set; } = default!;

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
        private JWTValidatorOptions Options;

        public JWTValidatorOptionsBuilder() => Options = new JWTValidatorOptions();

        public JWTValidatorOptions Build() => Options;

        public void WithAudience(String audience) => Options.Audience = audience;

        public void WithIssuer(String issuer) => Options.Issuer = issuer;

        public void WithSigningKeyFromOpenIdUrl(String openIdUrl) => Options.OpenIdUrl = openIdUrl;

        public void WithSigningKeyFromSecret(String secret) => Options.Secret = secret;

        public void WithExpiryDate() => Options.ExpiryDate = true;
    }
}