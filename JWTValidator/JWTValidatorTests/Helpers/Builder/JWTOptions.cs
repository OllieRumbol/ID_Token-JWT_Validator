using System;
using System.Collections.Generic;

namespace JWTValidatorTests.Helpers.Builder;

public class JWTOptions
{
    public String Secret { get; private set; } = default!;

    public String Issuer { get; private set; } = default!;

    public String Audience { get; private set; } = default!;

    public DateTime ExpiryDate { get; private set; }

    // When "not before" or "issued at" is not set, the token handler sets it to now.
    // A test with an expiry in the past must also set "not before", or the token handler refuses to create the token.
    public DateTime? NotBefore { get; private set; }

    public DateTime? IssuedAt { get; private set; }

    public List<KeyValuePair<String, String>> Claims { get; private set; } = default!;

    internal class JWTOptionsBuilder : IJWTOptionsBuilder
    {
        private JWTOptions JwtOptions;

        public JWTOptionsBuilder() => JwtOptions = new JWTOptions();

        public void WithSecret(String secret) => JwtOptions.Secret = secret;

        public void WithIssuer(String issuer) => JwtOptions.Issuer = issuer;

        public void WithAudience(String audience) => JwtOptions.Audience = audience;

        public void WithExpirationDate(DateTime expirationDate) => JwtOptions.ExpiryDate = expirationDate;

        public void WithNotBefore(DateTime notBefore) => JwtOptions.NotBefore = notBefore;

        public void WithIssuedAt(DateTime issuedAt) => JwtOptions.IssuedAt = issuedAt;

        public void WithClaims(List<KeyValuePair<String, String>> claims) => JwtOptions.Claims = claims;

        public JWTOptions Build() => JwtOptions;
    }
}