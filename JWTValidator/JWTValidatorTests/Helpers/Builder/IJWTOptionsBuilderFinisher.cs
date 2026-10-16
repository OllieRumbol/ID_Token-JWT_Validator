using System;
using System.Collections.Generic;

namespace JWTValidatorTests.Helpers.Builder;

public interface IJWTOptionsBuilderFinisher
{
    IJWTOptionsBuilderFinisher WithIssuer(String issuer);

    IJWTOptionsBuilderFinisher WithAudience(String audience);

    IJWTOptionsBuilderFinisher WithExpiryDate(DateTime expiryDate);

    IJWTOptionsBuilderFinisher WithNotBefore(DateTime notBefore);

    IJWTOptionsBuilderFinisher WithIssuedAt(DateTime issuedAt);

    IJWTOptionsBuilderFinisher WithCliams(List<KeyValuePair<String, String>> claims);

    JWTOptions Build();
}