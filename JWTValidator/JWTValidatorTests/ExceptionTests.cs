using JWTValidatorModel;
using JWTValidatorService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JWTValidatorTests;

[TestClass]
public class ExceptionTests
{
    [TestMethod]
    [ExpectedException(typeof(BuildingException))]
    public void InvalidBuilderTest()
    {
        //Assoicate

        //Act
        JWTValidatorOptions jWTValidatorOptions = JWTValidatorOptionsBuilder
            .Create()
            .WithIssuer("a")
            .WithExpiryDate()
            .Build();

        //Assert
    }
}