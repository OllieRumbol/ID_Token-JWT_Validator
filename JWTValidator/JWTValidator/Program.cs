using JWTValidatorService.Builder;
using JWTValidatorService.Validator;
using JWTValidatorService.Extensions;
using Microsoft.IdentityModel.Logging;

// This app is a local debugging tool and is never published.
// Showing personal data in IdentityModel error messages shows why a token fails validation.
IdentityModelEventSource.ShowPII = true;

Console.WriteLine("Enter the JWT");
String jwt = Console.ReadLine() ?? "";
Console.WriteLine();

Console.WriteLine("Enter the OpenID URL");
String openIdUrl = Console.ReadLine() ?? "";
Console.WriteLine();

if (String.IsNullOrWhiteSpace(jwt) || String.IsNullOrWhiteSpace(openIdUrl))
{
    Console.WriteLine("Enter both a JWT and an OpenID URL");
    return;
}

JWTValidatorOptions jwtValidatorOptions = JWTValidatorOptionsBuilderCreator
    .Create()
    .WithSigningKeyFromOpenIdUrl(openIdUrl)
    //.withexpirydate()
    .Build();

Console.WriteLine("JWT properties to be validated");
Console.WriteLine(jwtValidatorOptions.Print());
Console.WriteLine();

Dictionary<String, List<String>> result;
try
{
    result = new JWTValidator().ValidateJWT(jwt, jwtValidatorOptions);
}
catch (Exception exception)
{
    Console.WriteLine("Invalid JWT");

    // The real reason is often in an inner exception, for example when the OpenID configuration cannot be downloaded.
    Exception? error = exception;
    while (error is not null)
    {
        Console.WriteLine(error.Message);
        error = error.InnerException;
    }

    return;
}

Console.WriteLine("JWT claims");
Console.WriteLine(result.Print());
Console.WriteLine();

Boolean hasMemberRole = result.ContainsKeyAndValue("role", "MEMBER");
Console.WriteLine("Does the JWT contain a claim 'role' with value of 'member'");
Console.WriteLine(hasMemberRole);
Console.WriteLine();

Boolean hasMemberRole2 = result.ContainsValueInList("MEMBER");
Console.WriteLine("Does JWT contain a value of 'member'");
Console.WriteLine(hasMemberRole2);
Console.WriteLine();

DateTime expiryDate = new JWTExpiryChecker().WhenDoesJWTExpire(jwt, new Uri(openIdUrl));
Console.WriteLine("When does the JWT Expire");
Console.WriteLine(expiryDate);