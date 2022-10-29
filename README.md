# JWT Validator

## Introduction 
Hello and welcome to the "JWT Validator" C# library. The goal of this library is to simplify the validation of a JWT in C# and abstract away the complexities of the following objects 
- TokenValidationParameters
- JwtSecurityTokenHandler
- ClaimsPrincipal

The biggest use case of this library is to validate "ID Tokens" from the OAuth login flow. At a high level there are 3 key parts to this library
- Builder design pattern to constrct what parts of the JWT need validating and how they should be validated
- A "TryValidateJWT" method that returns whether the JWT is valid or not as well as a dictionary of what JWT contains
- Dictionary extension methods that helps developers handle the dictionary outputted from the above step
  
## Examples
```
String JWT = "eyJhbGciOi.....";

JWTValidatorOptions jWTValidatorOptions = JWTValidatorOptionsBuilder
    .Create()
    .WithSigningKeyFromSecret("serguhvalhoier.....")
    .WithExpiryDate()
    .Build();

if (new JWTValidator().TryValidateJWT(JWT, jWTValidatorOptions, out Dictionary<String, List<String>> result) == false)
{
    Console.WriteLine("Invalid JWT");
}

Boolean hasMemberRole = result.DictionaryContainsKeyAndValue("role", "MEMBER");
```