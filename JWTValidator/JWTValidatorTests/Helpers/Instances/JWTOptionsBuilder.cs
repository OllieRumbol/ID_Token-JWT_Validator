using JWTValidatorTests.Helpers.Interfaces;
using JWTValidatorTests.Helpers.Models;
using System;
using System.Collections.Generic;

namespace JWTValidatorTests.Helpers.Instances;

internal class JWTOptionsBuilder : IJWTOptionsBuilder
{
    private JWTOptions JWTOptions;

    public JWTOptionsBuilder()
    {
        JWTOptions = new JWTOptions();
    }

    public void WithSecret(String secret)
    {
        JWTOptions.Secret = secret;
    }

    public void WithIssuer(String issuer)
    {
        JWTOptions.Issuer = issuer;
    }

    public void WithAudience(String audience)
    {
        JWTOptions.Audience = audience;
    }

    public JWTOptions Build()
    {
        return JWTOptions;
    }

    public void WithExpirationDate(DateTime expirationDate)
    {
        JWTOptions.ExpiryDate = expirationDate;
    }

    public void WithClaims(List<KeyValuePair<String, String>> claims)
    {
        JWTOptions.Claims = claims;
    }
}