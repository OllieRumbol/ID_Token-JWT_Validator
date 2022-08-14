using JWTValidatorModel;
using JWTValidatorService.Interface;

namespace JWTValidatorService
{
    public class JWTValidatorOptionsBuilder : IJWTValidatorOptionsBuilder
    {
        private JWTValidatorOptions jWTValidatorOptions;

        private JWTValidatorOptionsBuilder()
        {
            jWTValidatorOptions = new JWTValidatorOptions();
        }

        public static JWTValidatorOptionsBuilder Create() => new JWTValidatorOptionsBuilder();

        public JWTValidatorOptions Build()
        {
            if(String.IsNullOrEmpty(jWTValidatorOptions.OpenIdUrl) && String.IsNullOrEmpty(jWTValidatorOptions.Secret))
            {
                throw new BuildingException("Builder must contain secret or OpenId url");
            }

            return jWTValidatorOptions;
        }

        public JWTValidatorOptionsBuilder WithAudience(string audience)
        {
            jWTValidatorOptions.Audience = audience;
            return this;
        }

        public JWTValidatorOptionsBuilder WithIssuer(string issuer)
        {
            jWTValidatorOptions.Issuer = issuer;
            return this;
        }

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

        public JWTValidatorOptionsBuilder WithExpiryDate()
        {
            jWTValidatorOptions.ExpiryDate = true;
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