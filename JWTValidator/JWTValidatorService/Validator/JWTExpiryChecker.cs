using JWTValidatorService.Builder;

namespace JWTValidatorService.Validator;

public class JWTExpiryChecker : IJWTExpiryChecker
{
    private readonly IJWTValidator JwtValidator;
    private readonly TimeProvider TimeProvider;

    public JWTExpiryChecker() : this(new JWTValidator(), TimeProvider.System) { }

    public JWTExpiryChecker(IJWTValidator jwtValidator) : this(jwtValidator, TimeProvider.System) { }

    // Internal so that tests can control the current time without a change to the public API.
    internal JWTExpiryChecker(IJWTValidator jwtValidator, TimeProvider timeProvider)
    {
        JwtValidator = jwtValidator;
        TimeProvider = timeProvider;
    }

    public Boolean HasJWTExpired(String jwt, String signingKey)
    {
        if (String.IsNullOrEmpty(jwt))
        {
            throw new ArgumentNullException(nameof(jwt));
        }

        if (String.IsNullOrEmpty(signingKey))
        {
            throw new ArgumentException(nameof(signingKey));
        }

        DateTime expiryDate = WhenDoesJWTExpire(jwt, signingKey);

        return expiryDate < TimeProvider.GetUtcNow().UtcDateTime;
    }

    public Boolean HasJWTExpired(String jwt, Uri openIdUrl)
    {
        if (String.IsNullOrEmpty(jwt))
        {
            throw new ArgumentNullException(nameof(jwt));
        }

        DateTime expiryDate = WhenDoesJWTExpire(jwt, openIdUrl);

        return expiryDate < TimeProvider.GetUtcNow().UtcDateTime;
    }

    public Boolean TryHasJWTExpired(String jwt, String signingKey, out Boolean isExpired)
    {
        try
        {
            isExpired = HasJWTExpired(jwt, signingKey);
            return true;
        }
        catch
        {
            isExpired = false;
            return false;
        }
    }

    public Boolean TryHasJWTExpired(String jwt, Uri openIdUrl, out Boolean isExpired)
    {
        try
        {
            isExpired = HasJWTExpired(jwt, openIdUrl);
            return true;
        }
        catch
        {
            isExpired = false;
            return false;
        }
    }

    public DateTime WhenDoesJWTExpire(String jwt, String signingKey)
    {
        if (String.IsNullOrEmpty(jwt))
        {
            throw new ArgumentException(nameof(jwt));
        }

        if (String.IsNullOrEmpty(signingKey))
        {
            throw new ArgumentException(nameof(signingKey));
        }

        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromSecret(signingKey)
            .Build();

        DateTime expiryDate = WhenDoesJWTExpire(jwt, jwtValidatorOptions);

        return expiryDate;
    }

    public DateTime WhenDoesJWTExpire(String jwt, Uri openIdUrl)
    {
        if (String.IsNullOrEmpty(jwt))
        {
            throw new ArgumentException(nameof(jwt));
        }

        JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
            .Create()
            .WithSigningKeyFromOpenIdUrl(openIdUrl.ToString())
            .Build();

        DateTime expiryDate = WhenDoesJWTExpire(jwt, jwtValidatorOptions);

        return expiryDate;
    }

    public Boolean TryGetWhenDoesJWTExpire(String jwt, String signingKey, out DateTime expiryDate)
    {
        try
        {
            expiryDate = WhenDoesJWTExpire(jwt, signingKey);
            return true;
        }
        catch
        {
            expiryDate = DateTime.MinValue;
            return false;
        }
    }

    public Boolean TryGetWhenDoesJWTExpire(String jwt, Uri openIdUrl, out DateTime expiryDate)
    {
        try
        {
            expiryDate = WhenDoesJWTExpire(jwt, openIdUrl);
            return true;
        }
        catch
        {
            expiryDate = DateTime.MinValue;
            return false;
        }
    }

    private DateTime WhenDoesJWTExpire(String jwt, JWTValidatorOptions jwtValidatorOptions)
    {
        if(JwtValidator.TryValidateJWT(jwt, jwtValidatorOptions, out Dictionary<String, List<String>> result) == false)
        {
            throw new Exception("Unable to validate JWT");
        }

        if (result.TryGetValue("exp", out List<String>? expiryDate) == false)
        {
            throw new Exception("Jwt does not contain an expiry");
        }

        if(expiryDate is null)
        {
            throw new Exception("Invalid JWT exipry");
        }

        Double seconds = Double.Parse(expiryDate.First());
        // UnixEpoch is UTC, so the result is UTC. An exp later than year 9999 makes AddSeconds throw.
        DateTime expirationDateTime = DateTime.UnixEpoch.AddSeconds(seconds);

        return expirationDateTime;
    }
}