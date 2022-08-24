using JWTValidatorTests.Helpers.Interfaces;
using JWTValidatorTests.Helpers.Models;
using System;
using System.Collections.Generic;

namespace JWTValidatorTests.Helpers.Instances;

public class JWTBuilderFinisher
{
    private IJWTOptionsBuilder JWTBuilder;

    public JWTBuilderFinisher(IJWTOptionsBuilder jWTBuilder)
    {
        JWTBuilder = jWTBuilder;
    }

    public JWTBuilderFinisher WithIssuer(String issuer)
    {
        JWTBuilder.WithIssuer(issuer);
        return this;
    }

    public JWTBuilderFinisher WithAudience(String audience)
    {
        JWTBuilder.WithAudience(audience);
        return this;
    }

    public JWTBuilderFinisher WithExpiryDate(DateTime expiryDate)
    {
        JWTBuilder.WithExpirationDate(expiryDate);
        return this;
    }

    public JWTBuilderFinisher WithCliams(List<KeyValuePair<String, String>> claims)
    {
        JWTBuilder.WithClaims(claims);
        return this;
    }

    public JWTOptions Build() => JWTBuilder.Build();
}