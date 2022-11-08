using System;
using System.Collections.Generic;

namespace JWTValidatorTests.Helpers.Builder;

public class JWTOptionsBuilderFinisher : IJWTOptionsBuilderFinisher
{
    private IJWTOptionsBuilder JwtOptionBuilder;

    internal JWTOptionsBuilderFinisher(IJWTOptionsBuilder jwtOptionsBuilder) => JwtOptionBuilder = jwtOptionsBuilder;

    public IJWTOptionsBuilderFinisher WithIssuer(String issuer)
    {
        JwtOptionBuilder.WithIssuer(issuer);
        return this;
    }

    public IJWTOptionsBuilderFinisher WithAudience(String audience)
    {
        JwtOptionBuilder.WithAudience(audience);
        return this;
    }

    public IJWTOptionsBuilderFinisher WithExpiryDate(DateTime expiryDate)
    {
        JwtOptionBuilder.WithExpirationDate(expiryDate);
        return this;
    }

    public IJWTOptionsBuilderFinisher WithCliams(List<KeyValuePair<String, String>> claims)
    {
        JwtOptionBuilder.WithClaims(claims);
        return this;
    }

    public JWTOptions Build() => JwtOptionBuilder.Build();
}