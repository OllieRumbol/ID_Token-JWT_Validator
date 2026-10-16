using JWTValidatorService.Builder;
using JWTValidatorService.Validator;

using JWTValidatorTests.Helpers.Builder;
using JWTValidatorTests.Helpers.Factory;

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JWTValidatorTests;

[TestClass]
public class JWTValidatorTests
{
    [TestMethod]
    public void ValidateJWTFromSecretTest()
    {
        // Arrange
        String secret = Guid.NewGuid().ToString();
        JWTOptions jwtOptions = JWTOptionsBuilderCreator
            .Create()
            .WithSecret(secret)
            .WithCliams(new List<KeyValuePair<String, String>>
            {
                new KeyValuePair<String, String>("role", "member")
            })
            .WithExpiryDate(DateTime.UtcNow.AddDays(2))
            .WithAudience("")
            .WithIssuer("")
            .Build();

        String jwt = new JWTFactory().GenerateToken(jwtOptions);

        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret(secret)
            .Build();

        // Act
        Boolean isJWTValid = new JWTValidator().TryValidateJWT(jwt, jwtValidatorOptions, out Dictionary<String, List<String>> result);

        // Assert
        Assert.IsTrue(isJWTValid);
        Assert.AreEqual(1, result.Count);
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

        CollectionAssert.AreEquivalent(expected.Keys, result.Keys);

        foreach (KeyValuePair<String, List<String>> expectedClaim in expected)
        {
            CollectionAssert.AreEqual(expectedClaim.Value, result[expectedClaim.Key], $"Claim '{expectedClaim.Key}'");
        }
    }
}