namespace JWTValidatorService
{
    public class JWTValidatorOptions
    {
        public String Secret { get; set; } = "";

        public String OpenIdUrl { get; set; } = "";

        public Boolean ValidateOnExpiryDate { get; set; }
    }

    public interface IJWTValidatorOptionsBuilder
    {
        JWTValidatorOptionsBuilder WithSigningKeyFromSecret(String secret);

        JWTValidatorOptionsBuilder WithSigningKeyFromOpenIdUrl(String openIdUrl);

        JWTValidatorOptionsBuilder WithValidateOnExpiryDate();

        JWTValidatorOptions Build();
    }


    public class JWTValidatorOptionsBuilder : IJWTValidatorOptionsBuilder
    {
        private JWTValidatorOptions jWTValidatorOptions;

        private JWTValidatorOptionsBuilder()
        {
            jWTValidatorOptions = new JWTValidatorOptions();
        }

        public static JWTValidatorOptionsBuilder Create() => new JWTValidatorOptionsBuilder();

        public JWTValidatorOptions Build() => jWTValidatorOptions;

        public JWTValidatorOptionsBuilder WithSigningKeyFromOpenIdUrl(String openIdUrl)
        {
            jWTValidatorOptions.OpenIdUrl = openIdUrl;
            return this;
        }

        public JWTValidatorOptionsBuilder WithSigningKeyFromSecret(String secret)
        {
            jWTValidatorOptions.Secret = secret;
            return this;
        }

        public JWTValidatorOptionsBuilder WithValidateOnExpiryDate()
        {
            jWTValidatorOptions.ValidateOnExpiryDate = true;
            return this;
        }
    }

    //public class JWTValidatorCreator
    //{
    //    private IJWTValidatorBuilder _jWTValidatorBuilder;

    //    public JWTValidatorCreator(IJWTValidatorBuilder jWTValidatorBuilder)
    //    {
    //        _jWTValidatorBuilder = jWTValidatorBuilder;
    //    }

    //    public JWTValidatorCreator()
    //    {
    //        _jWTValidatorBuilder = new JWTValidatorBuilder();
    //    }

    //    public void WithSigningKeyFromSecret(string secret)
    //    {
    //        _jWTValidatorBuilder.WithSigningKeyFromSecret(secret);
    //    }

    //    public JWTValidator Build()
    //    {
    //        return _jWTValidatorBuilder.Build();
    //    }

    //    public void WithValidateOnExpiryDate(Boolean validateOnExpiryDate)
    //    {
    //        _jWTValidatorBuilder.WithValidateOnExpiryDate(validateOnExpiryDate);
    //    }
    //}
}