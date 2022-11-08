using JWTValidatorService.Validator;

using JWTValidatorTests.Helpers.Builder;
using JWTValidatorTests.Helpers.Factory;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JWTValidatorTests;

[TestClass]
public class JWTExpiryCheckerTests
{
    [TestMethod]
    public void WhenDoesJWTExpireTestWithJWTAndSecret()
    {
        // Arrange   
        String secret = Guid.NewGuid().ToString();
        DateTime expiryDate = DateTime.UtcNow.AddDays(1);
        JWTOptions jwtOptions = JWTOptionsBuilderCreator
            .Create()
            .WithSecret(secret)
            .WithExpiryDate(expiryDate)
            .Build();

        String jwt = new JWTFactory().GenerateToken(jwtOptions);
        JWTExpiryChecker jwtExpiryChecker = new JWTExpiryChecker();

        // Act
        DateTime result = jwtExpiryChecker.WhenDoesJWTExpire(jwt, secret);

        // Assert
        Assert.AreEqual(expiryDate.ToString("dd/MM/yyyy HH:mm:ss"), result.ToString("dd/MM/yyyy HH:mm:ss"));
    }

    [TestMethod]
    public void TryWhenDoesJWTExpireTestWithJWTAndSecret()
    {
        // Arrange   
        String secret = Guid.NewGuid().ToString();
        DateTime expiryDate = DateTime.UtcNow.AddDays(1);
        JWTOptions jwtOptions = JWTOptionsBuilderCreator
            .Create()
            .WithSecret(secret)
            .WithExpiryDate(expiryDate)
            .Build();

        String jwt = new JWTFactory().GenerateToken(jwtOptions);
        JWTExpiryChecker jwtExpiryChecker = new JWTExpiryChecker();

        // Act
        Boolean result = jwtExpiryChecker.TryGetWhenDoesJWTExpire(jwt, secret, out DateTime expiryDateResult);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(expiryDate.ToString("dd/MM/yyyy HH:mm:ss"), expiryDateResult.ToString("dd/MM/yyyy HH:mm:ss"));
    }

    [TestMethod]
    public void HasJWTExpiredTestWithJWTAndSecret()
    {
        // Arrange   
        String secret = Guid.NewGuid().ToString();
        DateTime expiryDate = DateTime.UtcNow.AddDays(1);
        JWTOptions jwtOptions = JWTOptionsBuilderCreator
            .Create()
            .WithSecret(secret)
            .WithExpiryDate(expiryDate)
            .Build();

        String jwt = new JWTFactory().GenerateToken(jwtOptions);
        JWTExpiryChecker jwtExpiryChecker = new JWTExpiryChecker();

        // Act
        Boolean result = jwtExpiryChecker.HasJWTExpired(jwt, secret);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TryHasJWTExpiredTestWithJWTAndSecret()
    {
        // Arrange   
        String secret = Guid.NewGuid().ToString();
        DateTime expiryDate = DateTime.UtcNow.AddDays(1);
        JWTOptions jwtOptions = JWTOptionsBuilderCreator
            .Create()
            .WithSecret(secret)
            .WithExpiryDate(expiryDate)
            .Build();

        String jwt = new JWTFactory().GenerateToken(jwtOptions);
        JWTExpiryChecker jwtExpiryChecker = new JWTExpiryChecker();

        // Act
        Boolean result = jwtExpiryChecker.TryHasJWTExpired(jwt, secret, out Boolean isExpired);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(isExpired);
    }
}