namespace JWTValidatorService
{
    public interface IJWTValidatorOptionsBuilder
    {
        JWTValidatorOptionsBuilder WithSigningKeyFromSecret(String secret);

        JWTValidatorOptionsBuilder WithValidateOnExpiryDate(Boolean validateOnExpiryDate);

        JWTValidatorOptions Build();

    }

    public class JWTValidatorOptions
    {
        public String Secret { get; set; }

        public Boolean ValidateOnExpiryDate { get; set; }
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

        public JWTValidatorOptionsBuilder WithSigningKeyFromSecret(String secret)
        {
            jWTValidatorOptions.Secret = secret;
            return this;
        }

        public JWTValidatorOptionsBuilder WithValidateOnExpiryDate(Boolean validateOnExpiryDate)
        {
            jWTValidatorOptions.ValidateOnExpiryDate = validateOnExpiryDate;
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