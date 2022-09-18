namespace JWTValidatorService.Builder
{
    public interface IJWTValidatorOptionsBuilderCreator
    {
        IJWTValidatorOptionsBuilderFinisher WithSigningKeyFromSecret(string secret);

        IJWTValidatorOptionsBuilderFinisher WithSigningKeyFromOpenIdUrl(string openIdUrl);
    }
}