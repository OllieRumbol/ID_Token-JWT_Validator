using JWTValidatorModel;
using JWTValidatorService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JWTValidatorTests
{
    [TestClass]
    public  class BuilderTests
    {
        [TestMethod]
        public void BasicBuilderSecretTests()
        {
            //Associate

            //Act
            JWTValidatorOptions jWTValidatorOptions = JWTValidatorOptionsBuilder
                .Create()
                .WithSigningKeyFromSecret("abc")
                .WithIssuer("a")
                .WithExpiryDate()
                .Build();

            //Assert
            Assert.AreEqual("abc", jWTValidatorOptions.Secret);
        }
    }
}