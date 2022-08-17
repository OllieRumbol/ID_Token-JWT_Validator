namespace JWTValidatorModel
{
    public class JWTValidatorOptions
    {
        public String Secret { get; set; } = "";

        public String OpenIdUrl { get; set; } = "";

        public String Issuer { get; set; } = "";

        public String Audience { get; set; } = "";

        public Boolean ExpiryDate { get; set; }

        public String Print()
        {
            return
                $"Secret: {(String.IsNullOrEmpty(Secret) ? "None" : Secret)}\n" +
                $"OpenIdUrl: {(String.IsNullOrEmpty(OpenIdUrl) ? "None" : OpenIdUrl)}\n" +
                $"Issuer: {(String.IsNullOrEmpty(Issuer) ? "None" : Issuer )}\n" +
                $"Audience: {(String.IsNullOrEmpty(Audience) ? "None" : Audience)}\n" +
                $"Validate expiry date: {ExpiryDate}\n";
        }
    }
}