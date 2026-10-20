using JWTValidatorService.Builder;
using JWTValidatorService.Validator;

using JWTValidatorTests.Helpers.Builder;
using JWTValidatorTests.Helpers.Factory;

using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace JWTValidatorTests;

[TestClass]
public class JWTExpiryCheckerTests
{
    // exp 1793451600, the expiry in the worked example in spec 6.2.
    private static readonly DateTime ExampleExpiryDate = new DateTime(2026, 10, 31, 13, 0, 0, DateTimeKind.Utc);

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
        Assert.AreEqual(DateTimeKind.Utc, result.Kind);
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
        Assert.AreEqual(DateTimeKind.Utc, expiryDateResult.Kind);
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

    // Plan 7.2 test 1, the expiry example in spec 6.2. DateTime equality ignores Kind, so Kind is checked on its own.
    [TestMethod]
    public void WhenDoesJWTExpire_FixedExpiry_ReturnsExpiryInUtc()
    {
        // Arrange
        String secret = Guid.NewGuid().ToString();
        String jwt = CreateToken(secret, ExampleExpiryDate);
        JWTExpiryChecker jwtExpiryChecker = new JWTExpiryChecker();

        // Act
        DateTime result = jwtExpiryChecker.WhenDoesJWTExpire(jwt, secret);

        // Assert
        Assert.AreEqual(ExampleExpiryDate, result);
        Assert.AreEqual(DateTimeKind.Utc, result.Kind);
    }

    // Plan 7.2 test 2, every row of the expiry table in spec 6.2. The fake clock sets the current UTC time
    // and its own local time zone. The library reads neither the fake clock's zone nor the machine's zone,
    // so the Kind assertion and the CI run in Asia/Kolkata (plan 7.1) are what catch a local-time defect.
    [TestMethod]
    [DataRow("UTC", "2026-10-31T12:59:59Z", false, DisplayName = "UTC, one second before expiry")]
    [DataRow("Europe/London", "2026-10-31T13:00:01Z", true, DisplayName = "Europe/London, one second after expiry")]
    [DataRow("Asia/Kolkata", "2026-10-31T12:30:00Z", false, DisplayName = "Asia/Kolkata, 30 minutes before expiry")]
    [DataRow("America/New_York", "2026-10-31T13:30:00Z", true, DisplayName = "America/New_York, 30 minutes after expiry")]
    [DataRow("UTC", "2026-10-31T13:00:00Z", false, DisplayName = "Any time zone, exactly at expiry")]
    public void HasJWTExpired_ExpiryTableRow_ReturnsExpectedResult(String timeZoneId, String currentUtcTime, Boolean expectedHasExpired)
    {
        // Arrange
        String secret = Guid.NewGuid().ToString();
        String jwt = CreateToken(secret, ExampleExpiryDate);

        FakeTimeProvider timeProvider = new FakeTimeProvider(ParseUtc(currentUtcTime));
        timeProvider.SetLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
        JWTExpiryChecker jwtExpiryChecker = new JWTExpiryChecker(new JWTValidator(), timeProvider);

        // Act
        Boolean hasExpired = jwtExpiryChecker.HasJWTExpired(jwt, secret);
        DateTime expiryDate = jwtExpiryChecker.WhenDoesJWTExpire(jwt, secret);

        // Assert
        Assert.AreEqual(expectedHasExpired, hasExpired);
        Assert.AreEqual(ExampleExpiryDate, expiryDate);
        Assert.AreEqual(DateTimeKind.Utc, expiryDate.Kind);
    }

    // Plan 7.2 test 3, spec 6.1.3 items 3 and 6. The expiry methods check the signature, not the lifetime,
    // so they work for an expired token. The fake clock is the day after the first row's expiry. It is a fixed day
    // in the past, so both tokens have also expired on the real clock. Then both rows fail if the expiry methods
    // start to check the lifetime, which IdentityModel does against the real clock.
    [TestMethod]
    [DataRow("2025-06-01T13:00:00Z", DisplayName = "Expired yesterday")]
    [DataRow("1971-01-01T00:00:00Z", DisplayName = "Expired in 1971")]
    public void HasJWTExpired_ExpiryInThePast_ReturnsTrue(String expiryUtcTime)
    {
        // Arrange
        String secret = Guid.NewGuid().ToString();
        DateTime expiryDate = ParseUtc(expiryUtcTime).UtcDateTime;
        String jwt = CreateToken(secret, expiryDate);

        FakeTimeProvider timeProvider = new FakeTimeProvider(ParseUtc("2025-06-02T13:00:00Z"));
        JWTExpiryChecker jwtExpiryChecker = new JWTExpiryChecker(new JWTValidator(), timeProvider);

        // Act
        Boolean hasExpired = jwtExpiryChecker.HasJWTExpired(jwt, secret);
        DateTime result = jwtExpiryChecker.WhenDoesJWTExpire(jwt, secret);

        // Assert
        Assert.IsTrue(hasExpired);
        Assert.AreEqual(expiryDate, result);
        Assert.AreEqual(DateTimeKind.Utc, result.Kind);
    }

    // Plan 7.2 test 4, spec 6.1.3 items 4 and 5. The token handler cannot create these tokens, so they are signed by hand.
    // 253402300800 is 1 January 10000, one second later than the last time a DateTime can hold.
    [TestMethod]
    [DataRow("""{"role":"member"}""", DisplayName = "No exp")]
    [DataRow("""{"role":"member","exp":253402300800}""", DisplayName = "exp on 1 January 10000")]
    public void ExpiryMethods_ExpiryMissingOrOutOfRange_ThrowOrReturnFalse(String payload)
    {
        // Arrange
        String secret = Guid.NewGuid().ToString();
        String jwt = new JWTFactory().SignToken("""{"alg":"HS256","typ":"JWT"}""", payload, secret);
        JWTExpiryChecker jwtExpiryChecker = new JWTExpiryChecker();

        // The signature is valid, so the methods below fail because of exp, not because of the signature.
        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret(secret)
            .Build();
        Assert.IsTrue(new JWTValidator().TryValidateJWT(jwt, jwtValidatorOptions, out _));

        // Act and Assert
        Assert.Throws<Exception>(() => jwtExpiryChecker.HasJWTExpired(jwt, secret));
        Assert.Throws<Exception>(() => jwtExpiryChecker.WhenDoesJWTExpire(jwt, secret));
        Assert.IsFalse(jwtExpiryChecker.TryHasJWTExpired(jwt, secret, out _));
        Assert.IsFalse(jwtExpiryChecker.TryGetWhenDoesJWTExpire(jwt, secret, out _));
    }

    // The library does not check the lifetime, so "not before" and "issued at" only need to be earlier than the expiry.
    private static String CreateToken(String secret, DateTime expiryDate)
    {
        DateTime issuedAt = expiryDate.AddDays(-1);
        JWTOptions jwtOptions = JWTOptionsBuilderCreator
            .Create()
            .WithSecret(secret)
            .WithIssuedAt(issuedAt)
            .WithNotBefore(issuedAt)
            .WithExpiryDate(expiryDate)
            .Build();

        return new JWTFactory().GenerateToken(jwtOptions);
    }

    private static DateTimeOffset ParseUtc(String utcTime)
    {
        return DateTimeOffset.Parse(utcTime, CultureInfo.InvariantCulture);
    }
}