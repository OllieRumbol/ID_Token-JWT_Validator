namespace JWTValidatorService.Builder;

public interface IJWTValidatorOptionsBuilderCreator
{
    IJWTValidatorOptionsBuilderFinisher WithSigningKeyFromSecret(String secret);

    IJWTValidatorOptionsBuilderFinisher WithSigningKeyFromOpenIdUrl(String openIdUrl);
}