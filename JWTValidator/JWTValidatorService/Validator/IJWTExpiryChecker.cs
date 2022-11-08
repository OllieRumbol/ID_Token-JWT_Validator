namespace JWTValidatorService.Validator;

internal interface IJWTExpiryChecker
{
    Boolean HasJWTExpired(String jwt, String signingKey);

    Boolean HasJWTExpired(String jwt, Uri openIdUrl);

    Boolean TryHasJWTExpired(String jwt, String signingKey, out Boolean isExpired);

    Boolean TryHasJWTExpired(String jwt, Uri openIdUrl, out Boolean isExpired);

    DateTime WhenDoesJWTExpire(String jwt, String signingKey);

    DateTime WhenDoesJWTExpire(String jwt, Uri openIdUrl);

    Boolean TryGetWhenDoesJWTExpire(String jwt, String signingKey, out DateTime expiryDate);

    Boolean TryGetWhenDoesJWTExpire(String jwt, Uri openIdUrl, out DateTime expiryDate);
}
