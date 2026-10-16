using System;
using System.Collections.Generic;

namespace JWTValidatorTests.Helpers.Builder;

public interface IJWTOptionsBuilder
{
    void WithSecret(String secret);

    void WithIssuer(String issuer);

    void WithAudience(String audience);

    void WithExpirationDate(DateTime expirationDate);

    void WithNotBefore(DateTime notBefore);

    void WithIssuedAt(DateTime issuedAt);

    void WithClaims(List<KeyValuePair<String, String>> claims);

    JWTOptions Build();
}