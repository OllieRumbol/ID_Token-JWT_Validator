using JWTValidatorModel;
using JWTValidatorService;

JWTValidatorOptions jWTValidatorOptions = JWTValidatorOptionsBuilder
    .Create()
    .WithSigningKeyFromSecret("abc")
    .WithIssuer("a")
    .WithExpiryDate()
    .Build();

String JWT = "";
if(ValidateJWT.TryJWTValidation(JWT, jWTValidatorOptions, out String result) == false)
{

}

