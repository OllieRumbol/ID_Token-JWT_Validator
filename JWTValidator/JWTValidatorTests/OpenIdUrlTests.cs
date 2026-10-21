using JWTValidatorService.Builder;
using JWTValidatorService.Validator;

using JWTValidatorTests.Helpers.Factory;

using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace JWTValidatorTests;

// The OpenID URL path, tested offline (spec 6.1.2 items 3 and 4). The validator gets a test configuration
// in place of the download. The provider publishes K1 and K2. K3 is a key that the provider does not publish.
[TestClass]
public class OpenIdUrlTests
{
    // .invalid is a reserved domain, so this URL can never reach a real provider.
    private const String OpenIdUrl = "https://openid.example.invalid/.well-known/openid-configuration";

    // exp 1793451600, the expiry in the worked example in spec 6.2.
    private static readonly DateTime ExampleExpiryDate = new DateTime(2026, 10, 31, 13, 0, 0, DateTimeKind.Utc);

    // Generating an RSA key is slow, so the tests share the keys.
    private static readonly Dictionary<String, RSA> RsaKeys = new Dictionary<String, RSA>
    {
        { "K1", RSA.Create(2048) },
        { "K2", RSA.Create(2048) },
        { "K3", RSA.Create(2048) }
    };

    // Plan 7.2 test 15, the OpenID URL rows in spec 6.2.
    [TestMethod]
    [DataRow("K1", true, DisplayName = "Token signed with K1")]
    [DataRow("K2", true, DisplayName = "Token signed with K2, after the provider rotated keys")]
    [DataRow("K3", false, DisplayName = "Token signed with K3, which the provider does not publish")]
    public void TryValidateJWT_OpenIdUrl_AcceptsOnlyTokensSignedWithAPublishedKey(String signingKeyId, Boolean expectedIsValid)
    {
        // Arrange
        String jwt = CreateToken(signingKeyId);

        String? requestedUrl = null;
        JWTValidator jwtValidator = new JWTValidator(openIdUrl =>
        {
            requestedUrl = openIdUrl;
            return CreatePublishedConfiguration();
        });

        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromOpenIdUrl(OpenIdUrl)
            .Build();

        // Act
        Boolean isJWTValid = jwtValidator.TryValidateJWT(jwt, jwtValidatorOptions, out Dictionary<String, List<String>> result);

        // Assert
        Assert.AreEqual(expectedIsValid, isJWTValid);
        Assert.AreEqual(OpenIdUrl, requestedUrl);

        if (expectedIsValid)
        {
            CollectionAssert.AreEqual(new List<String> { "member" }, result["role"]);
        }
        else
        {
            Assert.AreEqual(0, result.Count);
        }
    }

    // Plan 7.2 test 16, the expiry of a token from the OpenID URL path.
    [TestMethod]
    public void WhenDoesJWTExpire_OpenIdUrl_ReturnsExpInUtc()
    {
        // Arrange
        String jwt = CreateToken("K1");
        JWTExpiryChecker jwtExpiryChecker = CreateExpiryChecker(ExampleExpiryDate);

        // Act
        DateTime result = jwtExpiryChecker.WhenDoesJWTExpire(jwt, new Uri(OpenIdUrl));

        // Assert
        Assert.AreEqual(ExampleExpiryDate, result);
        Assert.AreEqual(DateTimeKind.Utc, result.Kind);
    }

    // Plan 7.2 test 16. The current time is one second either side of exp.
    [TestMethod]
    [DataRow(-1, false, DisplayName = "One second before exp")]
    [DataRow(1, true, DisplayName = "One second after exp")]
    public void HasJWTExpired_OpenIdUrl_ComparesCurrentTimeWithExp(Int32 secondsAfterExpiry, Boolean expectedHasExpired)
    {
        // Arrange
        String jwt = CreateToken("K1");
        JWTExpiryChecker jwtExpiryChecker = CreateExpiryChecker(ExampleExpiryDate.AddSeconds(secondsAfterExpiry));

        // Act
        Boolean result = jwtExpiryChecker.HasJWTExpired(jwt, new Uri(OpenIdUrl));

        // Assert
        Assert.AreEqual(expectedHasExpired, result);
    }

    // An RS256 token with the signing key's ID in its header. Lifetime is not validated, so the fixed times never make it invalid.
    private static String CreateToken(String signingKeyId)
    {
        Dictionary<String, Object> claims = new Dictionary<String, Object>
        {
            { "role", "member" }
        };
        DateTime issuedAt = ExampleExpiryDate.AddHours(-2);

        return new JWTFactory().GenerateToken(RsaKeys[signingKeyId], signingKeyId, claims, issuedAt, issuedAt, ExampleExpiryDate);
    }

    // The configuration the provider publishes: the public parts of K1 and K2 only.
    private static IConfigurationManager<OpenIdConnectConfiguration> CreatePublishedConfiguration()
    {
        OpenIdConnectConfiguration configuration = new OpenIdConnectConfiguration();

        foreach (String keyId in new[] { "K1", "K2" })
        {
            RSAParameters publicKey = RsaKeys[keyId].ExportParameters(includePrivateParameters: false);
            configuration.SigningKeys.Add(new RsaSecurityKey(publicKey) { KeyId = keyId });
        }

        return new StaticConfigurationManager<OpenIdConnectConfiguration>(configuration);
    }

    private static JWTExpiryChecker CreateExpiryChecker(DateTime currentUtcTime)
    {
        JWTValidator jwtValidator = new JWTValidator(_ => CreatePublishedConfiguration());
        FakeTimeProvider timeProvider = new FakeTimeProvider(new DateTimeOffset(currentUtcTime));

        return new JWTExpiryChecker(jwtValidator, timeProvider);
    }
}
