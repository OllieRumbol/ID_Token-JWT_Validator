using JWTValidatorService.Builder;

namespace JWTValidatorService.Validator;

public class JWTExpiryChecker : IJWTExpiryChecker
{
    private IJWTValidator JwtValidator;

    public JWTExpiryChecker() => JwtValidator = new JWTValidator();

    public JWTExpiryChecker(IJWTValidator jwtValidator) => JwtValidator = jwtValidator;

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
        
        return expiryDate < DateTime.UtcNow;
    }

    public Boolean HasJWTExpired(String jwt, Uri openIdUrl)
    {
        if (String.IsNullOrEmpty(jwt))
        {
            throw new ArgumentNullException(nameof(jwt));
        }

        DateTime expiryDate = WhenDoesJWTExpire(jwt, openIdUrl);

        return expiryDate < DateTime.UtcNow;
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

        if (result.TryGetValue("exp", out List<String> expiryDate) == false)
        {
            throw new Exception("Jwt does not contain an expiry");
        }

        if(expiryDate is null)
        {
            throw new Exception("Invalid JWT exipry");
        }

        Double seconds = Double.Parse(expiryDate.First());
        DateTime expirationDateTime = new DateTime(1970, 1, 1, 0, 0, 0);
        expirationDateTime = expirationDateTime.AddSeconds(seconds).ToLocalTime();

        return expirationDateTime;
    }
}