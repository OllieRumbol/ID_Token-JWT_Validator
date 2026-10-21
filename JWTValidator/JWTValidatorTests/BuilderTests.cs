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

    // Plan 7.2 test 9. The builder does not check the secret length. Validation rejects a short secret (spec 6.1.6).
    [TestMethod]
    public void WithSigningKeyFromSecret_SecretOf31Bytes_IsAccepted()
    {
        // Arrange
        String secret = new String('k', 31);

        // Act
        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret(secret)
            .Build();

        // Assert
        Assert.AreEqual(secret, jwtValidatorOptions.Secret);
    }

    // Plan 7.2 test 9. The builder still rejects an empty secret, as in version 1.0.3.
    [TestMethod]
    public void WithSigningKeyFromSecret_EmptySecret_ThrowsArgumentNullException()
    {
        // Arrange
        JWTValidatorOptionsBuilderCreator jwtValidatorOptionsBuilderCreator = JWTValidatorOptionsBuilderCreator.Create();

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => jwtValidatorOptionsBuilderCreator.WithSigningKeyFromSecret(String.Empty));
    }
}