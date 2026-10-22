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
    private readonly Func<String, IConfigurationManager<OpenIdConnectConfiguration>> ConfigurationManagerFactory;

    public JWTValidator() : this(CreateConfigurationManager) { }

    // Internal so that tests can supply the OpenID configuration without a download and without a change to the public API.
    internal JWTValidator(Func<String, IConfigurationManager<OpenIdConnectConfiguration>> configurationManagerFactory)
    {
        ConfigurationManagerFactory = configurationManagerFactory;
    }

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
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = ConfigurationManagerFactory(options.OpenIdUrl);
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

    // Downloads the OpenID configuration, with its signing keys, from the URL.
    private static IConfigurationManager<OpenIdConnectConfiguration> CreateConfigurationManager(String openIdUrl)
    {
        return new ConfigurationManager<OpenIdConnectConfiguration>(openIdUrl, new OpenIdConnectConfigurationRetriever());
    }

    private Dictionary<String, List<String>> GetDictionaryOfClaims(IEnumerable<Claim> claims)
    {
        return claims
            .GroupBy(claim => claim.Type, claim => claim.Value)
            .ToDictionary(claim => claim.Key, claim => claim.ToList());
    }
}