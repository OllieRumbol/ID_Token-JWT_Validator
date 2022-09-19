using JWTValidatorService.Builder;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JWTValidatorTests;

[TestClass]
public class BuilderTests
{
    [TestMethod]
    public void BasicBuilderSecretTests()
    {
        //Associate

        //Act
        JWTValidatorOptions jWTValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret("abc")
            .Build();

        //Assert
        Assert.AreEqual("abc", jWTValidatorOptions.Secret);
    }

    [TestMethod]
    public void BasicBuilderUrlTests()
    {
        //Associate

        //Act
        JWTValidatorOptions jWTValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromOpenIdUrl("https://helloworld.com")
            .Build();

        //Assert
        Assert.AreEqual("https://helloworld.com", jWTValidatorOptions.OpenIdUrl);
    }

    [TestMethod]
    public void ComplexBuilderTests()
    {
        //Associate

        //Act
        JWTValidatorOptions jWTValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromOpenIdUrl("https://helloworld.com")
            .WithAudience("hello")
            .WithExpiryDate()
            .WithIssuer("world")
            .Build();

        //Assert
        Assert.AreEqual("https://helloworld.com", jWTValidatorOptions.OpenIdUrl);
        Assert.AreEqual("", jWTValidatorOptions.Audience);
        Assert.IsTrue(jWTValidatorOptions.ExpiryDate);
        Assert.AreEqual("", jWTValidatorOptions.Issuer);
    }
}