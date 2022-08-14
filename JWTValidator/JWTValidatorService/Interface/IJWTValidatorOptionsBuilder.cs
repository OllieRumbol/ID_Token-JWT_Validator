using JWTValidatorModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTValidatorService.Interface
{
    public interface IJWTValidatorOptionsBuilder
    {
        JWTValidatorOptionsBuilder WithSigningKeyFromSecret(String secret);

        JWTValidatorOptionsBuilder WithSigningKeyFromOpenIdUrl(String openIdUrl);

        JWTValidatorOptionsBuilder WithIssuer(String issuer);

        JWTValidatorOptionsBuilder WithAudience(String audience);

        JWTValidatorOptionsBuilder WithExpiryDate();

        JWTValidatorOptions Build();
    }
}
