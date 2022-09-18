using JWTValidatorTests.Helpers.Interfaces;
using JWTValidatorTests.Helpers.Models;

using System;
using System.Collections.Generic;

namespace JWTValidatorTests.Helpers.Instances;

public class JWTOptionsBuilderFinisher
{
    private IJWTOptionsBuilder JWTBuilder;

    internal JWTOptionsBuilderFinisher(IJWTOptionsBuilder jWTBuilder)
    {
        JWTBuilder = jWTBuilder;
    }

    public JWTOptionsBuilderFinisher WithIssuer(String issuer)
    {
        JWTBuilder.WithIssuer(issuer);
        return this;
    }

    public JWTOptionsBuilderFinisher WithAudience(String audience)
    {
        JWTBuilder.WithAudience(audience);
        return this;
    }

    public JWTOptionsBuilderFinisher WithExpiryDate(DateTime expiryDate)
    {
        JWTBuilder.WithExpirationDate(expiryDate);
        return this;
    }

    public JWTOptionsBuilderFinisher WithCliams(List<KeyValuePair<String, String>> claims)
    {
        JWTBuilder.WithClaims(claims);
        return this;
    }

    public JWTOptions Build() => JWTBuilder.Build();
}