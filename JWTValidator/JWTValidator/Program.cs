using JWTValidatorService;

JWTValidatorOptions jWTValidatorOptions = JWTValidatorOptionsBuilder
    .Create()
    .WithSigningKeyFromSecret("abc")
    .WithValidateOnExpiryDate()
    .Build();

String JWT = "";
if(ValidateJWT.TryJWTValidation(JWT, jWTValidatorOptions, out String result) == false)
{

}







