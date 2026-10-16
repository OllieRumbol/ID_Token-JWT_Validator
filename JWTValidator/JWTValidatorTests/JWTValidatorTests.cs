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
}