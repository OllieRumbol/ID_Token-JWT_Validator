# JWT Validator

## Introduction 
Hello and welcome to the "JWT Validator" C# library. The goal of this library is to simplify the validation of a JWT in C# and abstract away the complexities of the following objects 
- TokenValidationParameters
- JwtSecurityTokenHandler
- ClaimsPrincipal

The biggest use case of this library is to validate "ID Tokens" from the OAuth login flow. At a high level there are 3 key parts to this library
- Builder design pattern to construct what parts of the JWT need validating and how they should be validated
- A validator service with methods that outputs whether the JWT is valid or not as well as a dictionary of what JWT contains. Services to validate if the JWT has expired
- Dictionary extension methods that helps developers handle the dictionary outputted from the above step

## Requirements
- .NET 10. Version 2.0.0 targets `net10.0` only.
- The package depends on version 8 of the Microsoft IdentityModel packages.

## Secret length
When a token is signed with a secret (HMAC), the secret must be at least as long as the algorithm needs. The length is counted in UTF-8 bytes. A shorter secret fails validation: `ValidateJWT` throws and `TryValidateJWT` returns `false`. IdentityModel reports it as `SecurityTokenSignatureKeyNotFoundException`, not as an error about the length, so check the secret length first when you see this exception.

| Algorithm | Minimum secret length |
| --- | --- |
| HS256 | 32 bytes |
| HS384 | 48 bytes |
| HS512 | 64 bytes |

## Validation options
Start with `JWTValidatorOptionsBuilderCreator.Create()`, then choose where the signing key comes from:
- `WithSigningKeyFromSecret(secret)` for a token signed with a secret.
- `WithSigningKeyFromOpenIdUrl(url)` for a token from an OpenID Connect provider. The library downloads the signing keys from the provider's configuration URL, for example `https://accounts.google.com/.well-known/openid-configuration`.

Then add the checks you need, and call `Build()`:
- `WithIssuer(issuer)` checks who issued the token.
- `WithAudience(audience)` checks who the token is for.
- `WithExpiryDate()` checks that the token has not expired and is already valid. IdentityModel allows a clock skew of 5 minutes, so a token stays valid for up to 5 minutes after its expiry time.

## Example
```csharp
using JWTValidatorService.Builder;
using JWTValidatorService.Extensions;
using JWTValidatorService.Validator;

String jwt = "eyJhbGciOi.....";

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
