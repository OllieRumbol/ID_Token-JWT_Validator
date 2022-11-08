using JWTValidatorService.Builder;
using JWTValidatorService.Extensions;
using JWTValidatorService.Validator;
using JWTValidatorTests.Helpers.Builder;
using JWTValidatorTests.Helpers.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTValidatorTests;

[TestClass]
public class FullEndToEndTests
{
    [TestMethod]
    public void FullTest()
    {
        try
        {
            String secret = Guid.NewGuid().ToString();
            JWTOptions jwtOptions = JWTOptionsBuilderCreator
                .Create()
                .WithSecret(secret)
                .WithCliams(new List<KeyValuePair<String, String>>
                {
                    new KeyValuePair<String, String>("role", "member"),
                    new KeyValuePair<String, String>("access", "true"),
                    new KeyValuePair<String, String>("hasSubscription", "yes"),
                    new KeyValuePair<String, String>("entitlements", "full")
                })
                .WithExpiryDate(DateTime.UtcNow.AddDays(2))
                .WithAudience("audience")
                .WithIssuer("issuer")
                .Build();

            String jwt = new JWTFactory().GenerateToken(jwtOptions);

            JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
                .Create()
                .WithSigningKeyFromSecret(secret)
                .WithExpiryDate()
                .Build();

            Console.WriteLine(jwtValidatorOptions.Print());


            if (new JWTValidator().TryValidateJWT(jwt, jwtValidatorOptions, out Dictionary<String, List<String>> result) == false)
            {
                Assert.Fail();
            }

            Console.WriteLine(result.Print());

            Boolean hasMemberRole = result.ContainsKeyAndValue("role", "MEMBER");
            Console.WriteLine(hasMemberRole);

            Boolean hasMemberRole2 = result.ContainsValueInList("MEMBER");
            Console.WriteLine(hasMemberRole2);

            DateTime expiryDate = new JWTExpiryChecker().WhenDoesJWTExpire(jwt, secret);
            Console.WriteLine(expiryDate);

            Boolean isExpired = new JWTExpiryChecker().HasJWTExpired(jwt, secret);
            Console.WriteLine(isExpired);
        }
        catch
        {
            Assert.Fail();
        }
    }
}