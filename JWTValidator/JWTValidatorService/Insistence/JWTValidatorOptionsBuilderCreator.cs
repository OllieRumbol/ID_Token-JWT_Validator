using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWTValidatorService.Interface;

namespace JWTValidatorService.Insistence;

public class JWTValidatorOptionsBuilderCreator : IJWTValidatorOptionsBuilderCreator
{
    private IJWTValidatorOptionsBuilder _jWTValidatorOptionsBuilder;

    private JWTValidatorOptionsBuilderCreator()
    {
        _jWTValidatorOptionsBuilder = new JWTValidatorOptionsBuilder();
    }

    public static JWTValidatorOptionsBuilderCreator Create() => new JWTValidatorOptionsBuilderCreator();

    public IJWTValidatorOptionsBuilderFinisher WithSigningKeyFromSecret(String secret)
    {
        _jWTValidatorOptionsBuilder.WithSigningKeyFromSecret(secret);
        return new JWTValidatorOptionsBuilderFinisher(_jWTValidatorOptionsBuilder);
    }

    public IJWTValidatorOptionsBuilderFinisher WithSigningKeyFromOpenIdUrl(String openIdUrl)
    {
        _jWTValidatorOptionsBuilder.WithSigningKeyFromOpenIdUrl(openIdUrl);
        return new JWTValidatorOptionsBuilderFinisher(_jWTValidatorOptionsBuilder);
    }
}