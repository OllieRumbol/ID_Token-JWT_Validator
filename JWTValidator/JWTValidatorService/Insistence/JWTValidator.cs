﻿using JWTValidatorModel;
using JWTValidatorService.Interface;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTValidatorService;

public class JWTValidator : IJWTValidator
{
    public Boolean TryValidateJWT(String jwt, JWTValidatorOptions options, out Dictionary<String, List<String>> result)
    {

        try
        {
            result = ValidateJWT(jwt, options);
            return true;
        }
        catch (SecurityTokenValidationException ex)
        {
            result = new Dictionary<string, List<string>>();
            return false;
        }
        catch (Exception ex)
        {
            result = new Dictionary<string, List<string>>();
            return false;
        }
    }

    public Dictionary<String, List<String>> ValidateJWT(String jwt, JWTValidatorOptions options)
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        TokenValidationParameters validationParameters = GetTokenValidationParameters(options);

        ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(jwt, validationParameters, out SecurityToken token);

        return GetDictionaryOfClaims(claimsPrincipal.Claims);
    }

    private TokenValidationParameters GetTokenValidationParameters(JWTValidatorOptions options)
    {
        TokenValidationParameters validationParameters = new TokenValidationParameters();

        //
        if (String.IsNullOrEmpty(options.Secret) == false)
        {
            validationParameters.IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));
        }
        else
        {
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(options.OpenIdUrl, new OpenIdConnectConfigurationRetriever());
            OpenIdConnectConfiguration openIdConfig = configurationManager.GetConfigurationAsync(System.Threading.CancellationToken.None).Result;
            validationParameters.IssuerSigningKeys = openIdConfig.SigningKeys;
        }

        //Issuer (who made the JWT)
        if (String.IsNullOrEmpty(options.Issuer))
        {
            validationParameters.ValidateIssuer = false;
        }
        else
        {
            validationParameters.ValidIssuer = options.Issuer;
        }

        if (String.IsNullOrEmpty(options.Audience))
        {
            validationParameters.ValidateAudience = false;
        }
        else
        {
            validationParameters.ValidAudience = options.Audience;
        }

        validationParameters.ValidateLifetime = options.ExpiryDate;

        return validationParameters;
    }

    private Dictionary<String, List<String>> GetDictionaryOfClaims(IEnumerable<Claim> claims)
    {
        return claims
            .GroupBy(a => a.Type, a => a.Value)
            .ToDictionary(a => a.Key, a => a.ToList());
    }
}