using JWTValidatorService.Builder;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JWTValidatorTests;

[TestClass]
public class BuilderTests
{
    [TestMethod]
    public void BasicBuilderSecretTests()
    {
        // Arrange

        // Act
        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret("abc")
            .Build();

        // Assert
        Assert.AreEqual("abc", jwtValidatorOptions.Secret);
    }

    [TestMethod]
    public void BasicBuilderUrlTests()
    {
        // Arrange

        // Act
        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromOpenIdUrl("https://helloworld.com")
            .Build();

        // Assert
        Assert.AreEqual("https://helloworld.com", jwtValidatorOptions.OpenIdUrl);
    }

    [TestMethod]
    public void ComplexBuilderTests()
    {
        // Arrange

        // Act
        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromOpenIdUrl("https://helloworld.com")
            .WithAudience("hello")
            .WithExpiryDate()
            .WithIssuer("world")
            .Build();

        // Assert
        Assert.AreEqual("https://helloworld.com", jwtValidatorOptions.OpenIdUrl);
        Assert.AreEqual("hello", jwtValidatorOptions.Audience);
        Assert.IsTrue(jwtValidatorOptions.ExpiryDate);
        Assert.AreEqual("world", jwtValidatorOptions.Issuer);
    }
}