using JWTValidatorService.Builder;
using JWTValidatorService.Validator;

using JWTValidatorTests.Helpers.Builder;
using JWTValidatorTests.Helpers.Factory;

using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JWTValidatorTests;

[TestClass]
public class JWTValidatorTests
{
    // The lifetime of a test token, relative to the real clock.
    public enum TokenLifetime
    {
        Current,
        Expired,
        NotYetValid
    }

    // Plan 7.2 test 6, the claims example in spec 6.2. The fixed times give the same claims on every run.
    [TestMethod]
    public void ValidateJWTFromSecretTest()
    {
        // Arrange
        String secret = Guid.NewGuid().ToString();
        DateTime issuedAt = new DateTime(2026, 10, 29, 13, 0, 0, DateTimeKind.Utc);
        DateTime expiryDate = issuedAt.AddDays(2);
        JWTOptions jwtOptions = JWTOptionsBuilderCreator
            .Create()
            .WithSecret(secret)
            .WithCliams(new List<KeyValuePair<String, String>>
            {
                new KeyValuePair<String, String>("role", "member")
            })
            .WithIssuedAt(issuedAt)
            .WithNotBefore(issuedAt)
            .WithExpiryDate(expiryDate)
            .Build();

        String jwt = new JWTFactory().GenerateToken(jwtOptions);

        // Lifetime is not validated, so the fixed times never make this token invalid.
        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret(secret)
            .Build();

        // Act
        Boolean isJWTValid = new JWTValidator().TryValidateJWT(jwt, jwtValidatorOptions, out Dictionary<String, List<String>> result);

        // Assert
        Dictionary<String, List<String>> expected = new Dictionary<String, List<String>>
        {
            { "role", new List<String> { "member" } },
            { "exp", new List<String> { "1793451600" } },
            { "nbf", new List<String> { "1793278800" } },
            { "iat", new List<String> { "1793278800" } }
        };

        Assert.IsTrue(isJWTValid);
        AssertClaimsAreExactly(expected, result);
    }

    // Plan 7.2 test 14, issuer. A token is rejected only when the option is set and does not match.
    // A null expected exception means the token is valid.
    [TestMethod]
    [DataRow("issuer", "issuer", null, DisplayName = "Issuer set and matches")]
    [DataRow("issuer", "other-issuer", typeof(SecurityTokenInvalidIssuerException), DisplayName = "Issuer set and does not match")]
    [DataRow(null, "issuer", typeof(SecurityTokenInvalidIssuerException), DisplayName = "Issuer set and token has no issuer")]
    [DataRow("issuer", null, null, DisplayName = "Issuer not set")]
    public void ValidateJWT_IssuerOption_RejectsOnlyWhenSetAndNotMatching(String? tokenIssuer, String? validIssuer, Type? expectedExceptionType)
    {
        // Arrange
        String secret = Guid.NewGuid().ToString();
        String jwt = CreateToken(secret, tokenIssuer, null, TokenLifetime.Current);

        IJWTValidatorOptionsBuilderFinisher jwtValidatorOptionsBuilder = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret(secret);

        if (validIssuer is not null)
        {
            jwtValidatorOptionsBuilder.WithIssuer(validIssuer);
        }

        // Act
        Type? exceptionType = GetValidationExceptionType(jwt, jwtValidatorOptionsBuilder.Build());

        // Assert
        Assert.AreEqual(expectedExceptionType, exceptionType);
    }

    // Plan 7.2 test 14, audience. A token is rejected only when the option is set and does not match.
    // A null expected exception means the token is valid.
    [TestMethod]
    [DataRow("audience", "audience", null, DisplayName = "Audience set and matches")]
    [DataRow("audience", "other-audience", typeof(SecurityTokenInvalidAudienceException), DisplayName = "Audience set and does not match")]
    [DataRow(null, "audience", typeof(SecurityTokenInvalidAudienceException), DisplayName = "Audience set and token has no audience")]
    [DataRow("audience", null, null, DisplayName = "Audience not set")]
    public void ValidateJWT_AudienceOption_RejectsOnlyWhenSetAndNotMatching(String? tokenAudience, String? validAudience, Type? expectedExceptionType)
    {
        // Arrange
        String secret = Guid.NewGuid().ToString();
        String jwt = CreateToken(secret, null, tokenAudience, TokenLifetime.Current);

        IJWTValidatorOptionsBuilderFinisher jwtValidatorOptionsBuilder = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret(secret);

        if (validAudience is not null)
        {
            jwtValidatorOptionsBuilder.WithAudience(validAudience);
        }

        // Act
        Type? exceptionType = GetValidationExceptionType(jwt, jwtValidatorOptionsBuilder.Build());

        // Assert
        Assert.AreEqual(expectedExceptionType, exceptionType);
    }

    // Plan 7.2 test 14, lifetime. A token is rejected only when the option is set and the token has expired
    // or is not yet valid. A null expected exception means the token is valid.
    [TestMethod]
    [DataRow(true, TokenLifetime.Current, null, DisplayName = "Lifetime set and token not expired")]
    [DataRow(true, TokenLifetime.Expired, typeof(SecurityTokenExpiredException), DisplayName = "Lifetime set and token expired")]
    [DataRow(true, TokenLifetime.NotYetValid, typeof(SecurityTokenNotYetValidException), DisplayName = "Lifetime set and token not yet valid")]
    [DataRow(false, TokenLifetime.Current, null, DisplayName = "Lifetime not set and token not expired")]
    [DataRow(false, TokenLifetime.Expired, null, DisplayName = "Lifetime not set and token expired")]
    [DataRow(false, TokenLifetime.NotYetValid, null, DisplayName = "Lifetime not set and token not yet valid")]
    public void ValidateJWT_LifetimeOption_RejectsOnlyWhenSetAndOutsideLifetime(Boolean validateLifetime, TokenLifetime tokenLifetime, Type? expectedExceptionType)
    {
        // Arrange
        String secret = Guid.NewGuid().ToString();
        String jwt = CreateToken(secret, null, null, tokenLifetime);

        IJWTValidatorOptionsBuilderFinisher jwtValidatorOptionsBuilder = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret(secret);

        if (validateLifetime)
        {
            jwtValidatorOptionsBuilder.WithExpiryDate();
        }

        // Act
        Type? exceptionType = GetValidationExceptionType(jwt, jwtValidatorOptionsBuilder.Build());

        // Assert
        Assert.AreEqual(expectedExceptionType, exceptionType);
    }

    // Plan 7.2 test 17. The expected values were recorded from a run with IdentityModel 6.22.0,
    // before the upgrade, so the test shows any change in how claim values are written.
    [TestMethod]
    public void ValidateJWT_ClaimsOfEachJsonType_ReturnsClaimValuesAsBefore()
    {
        // Arrange
        String secret = Guid.NewGuid().ToString();
        Dictionary<String, Object> claims = new Dictionary<String, Object>
        {
            { "name", "Alice" },
            { "email_verified", true },
            { "age", 42 },
            { "score", 4.75 },
            { "roles", new[] { "admin", "member" } },
            { "address", new Dictionary<String, Object> { { "city", "London" }, { "floor", 3 } } }
        };
        DateTime issuedAt = new DateTime(2026, 10, 31, 11, 0, 0, DateTimeKind.Utc);
        DateTime notBefore = new DateTime(2026, 10, 31, 12, 0, 0, DateTimeKind.Utc);
        DateTime expiryDate = new DateTime(2026, 10, 31, 13, 0, 0, DateTimeKind.Utc);

        String jwt = new JWTFactory().GenerateToken(secret, claims, issuedAt, notBefore, expiryDate);

        // Lifetime is not validated, so the fixed times never make this token invalid.
        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret(secret)
            .Build();

        // Act
        Dictionary<String, List<String>> result = new JWTValidator().ValidateJWT(jwt, jwtValidatorOptions);

        // Assert
        Dictionary<String, List<String>> expected = new Dictionary<String, List<String>>
        {
            { "name", new List<String> { "Alice" } },
            { "email_verified", new List<String> { "true" } },
            { "age", new List<String> { "42" } },
            { "score", new List<String> { "4.75" } },
            { "roles", new List<String> { "admin", "member" } },
            { "address", new List<String> { "{\"city\":\"London\",\"floor\":3}" } },
            { "nbf", new List<String> { "1793448000" } },
            { "exp", new List<String> { "1793451600" } },
            { "iat", new List<String> { "1793444400" } }
        };

        AssertClaimsAreExactly(expected, result);
    }

    // Plan 7.2 test 7, the secret length example in spec 6.2. HS256 needs a secret of at least 32 bytes.
    [TestMethod]
    public void TryValidateJWT_SecretOf32Bytes_ReturnsTrue()
    {
        // Arrange
        String secret = new String('k', 32);
        String jwt = SignTokenWithAlgorithm("HS256", secret);

        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret(secret)
            .Build();

        // Act
        Boolean isJWTValid = new JWTValidator().TryValidateJWT(jwt, jwtValidatorOptions, out Dictionary<String, List<String>> result);

        // Assert
        Assert.IsTrue(isJWTValid);
        CollectionAssert.AreEqual(new List<String> { "member" }, result["role"]);
    }

    // Plan 7.2 test 8, the secret length rows in spec 6.2. Each token is correctly signed with its own secret,
    // so a secret shorter than the algorithm's minimum is the only reason it can fail.
    // IdentityModel cannot use a key that is too short, so it reports that no signing key matched.
    // A null expected exception means the token is valid.
    [TestMethod]
    [DataRow("HS256", 31, typeof(SecurityTokenSignatureKeyNotFoundException), DisplayName = "HS256 with a 31-byte secret")]
    [DataRow("HS512", 63, typeof(SecurityTokenSignatureKeyNotFoundException), DisplayName = "HS512 with a 63-byte secret")]
    [DataRow("HS512", 64, null, DisplayName = "HS512 with a 64-byte secret")]
    public void ValidateJWT_SecretShorterThanAlgorithmMinimum_IsRejected(String algorithm, Int32 secretLength, Type? expectedExceptionType)
    {
        // Arrange
        String secret = new String('k', secretLength);
        String jwt = SignTokenWithAlgorithm(algorithm, secret);

        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret(secret)
            .Build();

        // Act
        Boolean isJWTValid = new JWTValidator().TryValidateJWT(jwt, jwtValidatorOptions, out _);
        Type? exceptionType = GetValidationExceptionType(jwt, jwtValidatorOptions);

        // Assert
        Assert.AreEqual(expectedExceptionType is null, isJWTValid);
        Assert.AreEqual(expectedExceptionType, exceptionType);
    }

    // Checks the exact set of claim keys, then the values of each claim in order.
    private static void AssertClaimsAreExactly(Dictionary<String, List<String>> expected, Dictionary<String, List<String>> actual)
    {
        CollectionAssert.AreEquivalent(expected.Keys, actual.Keys);

        foreach (KeyValuePair<String, List<String>> expectedClaim in expected)
        {
            CollectionAssert.AreEqual(expectedClaim.Value, actual[expectedClaim.Key], $"Claim '{expectedClaim.Key}'");
        }
    }

    // A null issuer or audience leaves that claim out of the token.
    private static String CreateToken(String secret, String? issuer, String? audience, TokenLifetime tokenLifetime)
    {
        // IdentityModel checks the lifetime against the real clock, so these times use it too.
        // Each margin is at least one hour, so a slow test run cannot change the result (plan 7.1).
        DateTime issuedAt = DateTime.UtcNow.AddHours(-1);
        DateTime notBefore = issuedAt;
        DateTime expiryDate = DateTime.UtcNow.AddDays(1);

        if (tokenLifetime == TokenLifetime.Expired)
        {
            issuedAt = new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            notBefore = issuedAt;
            expiryDate = new DateTime(2020, 1, 2, 12, 0, 0, DateTimeKind.Utc);
        }

        if (tokenLifetime == TokenLifetime.NotYetValid)
        {
            notBefore = DateTime.UtcNow.AddDays(1);
            expiryDate = DateTime.UtcNow.AddDays(2);
        }

        IJWTOptionsBuilderFinisher jwtOptionsBuilder = JWTOptionsBuilderCreator
            .Create()
            .WithSecret(secret)
            .WithIssuedAt(issuedAt)
            .WithNotBefore(notBefore)
            .WithExpiryDate(expiryDate);

        if (issuer is not null)
        {
            jwtOptionsBuilder.WithIssuer(issuer);
        }

        if (audience is not null)
        {
            jwtOptionsBuilder.WithAudience(audience);
        }

        return new JWTFactory().GenerateToken(jwtOptionsBuilder.Build());
    }

    // Signs by hand, because the token handler cannot sign with a key shorter than the algorithm's minimum.
    // The token has no exp, so it never expires.
    private static String SignTokenWithAlgorithm(String algorithm, String secret)
    {
        String header = $$"""{"alg":"{{algorithm}}","typ":"JWT"}""";
        String payload = """{"role":"member"}""";

        return new JWTFactory().SignToken(header, payload, secret);
    }

    // Returns the exact type of the exception that ValidateJWT throws, or null when the token is valid.
    private static Type? GetValidationExceptionType(String jwt, JWTValidatorOptions jwtValidatorOptions)
    {
        try
        {
            new JWTValidator().ValidateJWT(jwt, jwtValidatorOptions);
            return null;
        }
        catch (Exception exception)
        {
            return exception.GetType();
        }
    }
}
