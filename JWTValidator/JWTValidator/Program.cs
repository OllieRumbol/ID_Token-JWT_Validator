using JWTValidatorService;

JWTValidatorOptions foo = JWTValidatorOptionsBuilder
    .Create()
    .WithSigningKeyFromSecret("abc")
    .WithValidateOnExpiryDate(false)
    .Build();


