using System;
using System.Collections.Generic;

namespace JWTValidatorTests.Helpers.Builder;

public class JWTOptions
{
    public String Secret { get; private set; } = default!;

    public String Issuer { get; private set; } = default!;

    public String Audience { get; private set; } = default!;

    public DateTime ExpiryDate { get; private set; }

    public List<KeyValuePair<String, String>> Claims { get; private set; } = default!;

    internal class JWTOptionsBuilder : IJWTOptionsBuilder
    {
        private JWTOptions JwtOptions;

        public JWTOptionsBuilder() => JwtOptions = new JWTOptions();

        public void WithSecret(String secret) => JwtOptions.Secret = secret;

        public void WithIssuer(String issuer) => JwtOptions.Issuer = issuer;

        public void WithAudience(String audience) => JwtOptions.Audience = audience;

        public void WithExpirationDate(DateTime expirationDate) => JwtOptions.ExpiryDate = expirationDate;

        public void WithClaims(List<KeyValuePair<String, String>> claims) => JwtOptions.Claims = claims;

        public JWTOptions Build() => JwtOptions;
    }
}