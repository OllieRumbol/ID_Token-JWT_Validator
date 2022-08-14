using JWTValidatorModel;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTValidatorService
{
    public class ValidateJWT
    {
        public static Boolean TryJWTValidation(String jwt, JWTValidatorOptions options, out Dictionary<String, List<String>> result)
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            TokenValidationParameters validationParameters = GetTokenValidationParameters(options);

            ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(jwt, validationParameters, out SecurityToken token);

            result = new Dictionary<String, List<String>>();

            foreach (Claim claim in claimsPrincipal.Claims)
            {
                if (result.ContainsKey(claim.Type))
                {
                    result[claim.Type].Add(claim.Value);
                }
                else
                {
                    result.Add(claim.Type, new List<String>
                    {
                        claim.Value
                    });
                }
            }

            return true;
        }

        private static TokenValidationParameters GetTokenValidationParameters(JWTValidatorOptions options)
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
    }
}