# JWT Validator

[![CI](https://github.com/OllieRumbol/ID_Token-JWT_Validator/actions/workflows/ci.yml/badge.svg)](https://github.com/OllieRumbol/ID_Token-JWT_Validator/actions/workflows/ci.yml)

JWT Validator is a C# library that makes it simple to validate a JWT. Its main use is to validate ID tokens from an OpenID Connect sign-in. It hides the complexity of these IdentityModel types:
- `TokenValidationParameters`
- `JwtSecurityTokenHandler`
- `ClaimsPrincipal`

The library has these parts:
- An options builder, which sets what to validate and where the signing keys come from.
- A validator, which says whether the JWT is valid and returns its claims as a dictionary.
- An expiry checker, which says when the JWT expires and whether it has expired.
- Extension methods for the claims dictionary.

## Installation
```
dotnet add package JWTValidator
```
To install a pre-release version, such as a 2.0.0 beta, add `--prerelease`.

## Requirements
- .NET 10. Version 2.0.0 targets `net10.0` only.
- The package depends on version 8 of the Microsoft IdentityModel packages.

## Upgrading from 1.0.3
Version 2.0.0 has breaking changes. Read the release notes on the [package page](https://www.nuget.org/packages/JWTValidator) before you upgrade.

## Validation options
Start with `JWTValidatorOptionsBuilderCreator.Create()`, then choose where the signing key comes from:
- `WithSigningKeyFromSecret(secret)` for a token signed with a secret.
- `WithSigningKeyFromOpenIdUrl(url)` for a token from an OpenID Connect provider. The library downloads the signing keys from the provider's configuration URL, for example `https://accounts.google.com/.well-known/openid-configuration`.

Then add the checks you need, and call `Build()`:
- `WithIssuer(issuer)` checks who issued the token.
- `WithAudience(audience)` checks who the token is for.
- `WithExpiryDate()` checks that the token has not expired and is already valid. IdentityModel allows a clock skew of 5 minutes, so a token stays valid for up to 5 minutes after its expiry time.

## Secret length
When a token is signed with a secret (HMAC), the secret must be at least as long as the algorithm needs. The length is counted in UTF-8 bytes. A shorter secret fails validation: `ValidateJWT` throws and `TryValidateJWT` returns `false`. IdentityModel reports it as `SecurityTokenSignatureKeyNotFoundException`, not as an error about the length, so check the secret length first when you see this exception.

| Algorithm | Minimum secret length |
| --- | --- |
| HS256 | 32 bytes |
| HS384 | 48 bytes |
| HS512 | 64 bytes |

## ValidateJWT or TryValidateJWT
- `TryValidateJWT` returns `false` when the token is not valid. Use it when you only need a yes or no.
- `ValidateJWT` throws when the token is not valid. The exception message says why, which helps when you debug.

IdentityModel hides personal data in its messages. To see it while you debug, set `IdentityModelEventSource.ShowPII = true` in your own application. The library does not turn it on. The token itself stays hidden unless you also set `IdentityModelEventSource.LogCompleteSecurityArtifact = true`.

The expiry checker has two pairs of methods, one that throws and one that returns `false`:
- `WhenDoesJWTExpire` and `TryGetWhenDoesJWTExpire` give the expiry time in UTC.
- `HasJWTExpired` and `TryHasJWTExpired` compare the expiry time with the current UTC time.

## Claims
The validator returns the claims as `Dictionary<String, List<String>>`:
- The key is the claim name as it is in the token, for example `sub` or `email`.
- Every value is a string. A claim with an array holds one string for each item.
- Numbers and booleans become text, for example `"42"` and `"true"`.
- A nested object becomes one JSON string, for example `{"city":"London","floor":3}`.
- A JSON `null` becomes an empty string.
- The times `exp`, `nbf` and `iat` are Unix seconds, for example `"1793451600"`.

## Example: a token signed with a secret
```csharp
using JWTValidatorService.Builder;
using JWTValidatorService.Extensions;
using JWTValidatorService.Validator;

Console.WriteLine("Enter the JWT");
String jwt = Console.ReadLine() ?? "";

// Read the secret from configuration in a real application. HS256 needs at least 32 bytes.
String secret = "a-secret-of-at-least-32-bytes-for-hs256";

JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
    .Create()
    .WithSigningKeyFromSecret(secret)
    .WithExpiryDate()
    .Build();

if (new JWTValidator().TryValidateJWT(jwt, jwtValidatorOptions, out Dictionary<String, List<String>> claims) == false)
{
    Console.WriteLine("Invalid JWT");
    return;
}

Console.WriteLine(claims.Print());

Boolean hasMemberRole = claims.ContainsKeyAndValue("role", "member");
Boolean hasMemberValue = claims.ContainsValueInList("member");

// The expiry is returned in UTC.
if (new JWTExpiryChecker().TryGetWhenDoesJWTExpire(jwt, secret, out DateTime expiryDate))
{
    Console.WriteLine($"The JWT expires at {expiryDate:u}");
}
```

## Example: an ID token from an OpenID Connect provider
```csharp
using JWTValidatorService.Builder;
using JWTValidatorService.Validator;

Console.WriteLine("Enter the ID token");
String idToken = Console.ReadLine() ?? "";

// Use your own client ID as the audience.
// WithIssuer takes one value. Google can also issue tokens with the issuer "accounts.google.com", without "https://".
JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
    .Create()
    .WithSigningKeyFromOpenIdUrl("https://accounts.google.com/.well-known/openid-configuration")
    .WithIssuer("https://accounts.google.com")
    .WithAudience("your-client-id.apps.googleusercontent.com")
    .WithExpiryDate()
    .Build();

try
{
    Dictionary<String, List<String>> claims = new JWTValidator().ValidateJWT(idToken, jwtValidatorOptions);
    Console.WriteLine($"Signed in as {claims["sub"][0]}");
}
catch (Exception exception)
{
    // ValidateJWT throws when the token is not valid. A failed download is wrapped
    // in an AggregateException, so print the message of the exception inside it, which says why.
    Console.WriteLine(exception.GetBaseException().Message);
}
```

## Known limitations
- Each validation clears `JwtSecurityTokenHandler.DefaultInboundClaimTypeMap`. This keeps claim names as they are in the token. The map is shared by the whole process, so it also changes the claim names for other code in your application that uses `JwtSecurityTokenHandler`. The clear is not thread-safe, so other code can fail if it reads the map at the same moment.
- With an OpenID URL, each call downloads the OpenID configuration again and waits for it synchronously. The expiry checker validates the token again, so it downloads the configuration again too.

## Licence
MIT. See [LICENSE.txt](https://github.com/OllieRumbol/ID_Token-JWT_Validator/blob/main/LICENSE.txt).
