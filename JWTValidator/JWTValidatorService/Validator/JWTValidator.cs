using JWTValidatorService.Builder;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTValidatorService.Validator;

public class JWTValidator : IJWTValidator
{
    public Boolean TryValidateJWT(String jwt, JWTValidatorOptions options, out Dictionary<String, List<String>> result)
    {
        try
        {
            result = ValidateJWT(jwt, options);
            return true;
        }
        catch (Exception)
        {
            result = new Dictionary<String, List<String>>();
            return false;
        }
    }

    public Dictionary<String, List<String>> ValidateJWT(String jwt, JWTValidatorOptions options)
    {
        if (String.IsNullOrEmpty(jwt))
        {
            throw new ArgumentException(nameof(jwt));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        TokenValidationParameters validationParameters = GetTokenValidationParameters(options);

        ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(jwt, validationParameters, out _);

        return GetDictionaryOfClaims(claimsPrincipal.Claims);
    }

    private TokenValidationParameters GetTokenValidationParameters(JWTValidatorOptions options)
    {
        if (String.IsNullOrEmpty(options.Secret) && String.IsNullOrEmpty(options.OpenIdUrl))
        {
            throw new ArgumentException("Must provide a secret or open id url");
        }

        TokenValidationParameters validationParameters = new TokenValidationParameters();

        //Issuer Signing Keys
        if (String.IsNullOrEmpty(options.Secret) == false)
        {
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));
        }
        else
        {
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(options.OpenIdUrl, new OpenIdConnectConfigurationRetriever());
            OpenIdConnectConfiguration openIdConfig = configurationManager.GetConfigurationAsync(CancellationToken.None).Result;
            validationParameters.IssuerSigningKeys = openIdConfig.SigningKeys;
        }

        // Issuer identifies who made the JWT
        if (String.IsNullOrEmpty(options.Issuer))
        {
            validationParameters.ValidateIssuer = false;
        }
        else
        {
            validationParameters.ValidateIssuer = true;
            validationParameters.ValidIssuer = options.Issuer;
        }

        // Audience identifies the recipients that the JWT
        if (String.IsNullOrEmpty(options.Audience))
        {
            validationParameters.ValidateAudience = false;
        }
        else
        {
            validationParameters.ValidateAudience = true;
            validationParameters.ValidAudience = options.Audience;
        }

        // Validate the expiry date of the JWT
        validationParameters.ValidateLifetime = options.ExpiryDate;

        return validationParameters;
    }

    private Dictionary<String, List<String>> GetDictionaryOfClaims(IEnumerable<Claim> claims)
    {
        return claims
            .GroupBy(claim => claim.Type, claim => claim.Value)
            .ToDictionary(claim => claim.Key, claim => claim.ToList());
    }
}