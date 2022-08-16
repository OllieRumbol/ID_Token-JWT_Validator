using JWTValidatorModel;

namespace JWTValidatorService.Interface
{
    public  interface IJWTValidator
    {
        Boolean TryJWTValidation(String jwt, JWTValidatorOptions options, out Dictionary<String, List<String>> result);
    }
}