using JWTValidatorModel;
using JWTValidatorService.Interface;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTValidatorService
{
    public class JWTValidator : IJWTValidator
    {
        public Boolean TryJWTValidation(String jwt, JWTValidatorOptions options, out Dictionary<String, List<String>> result)
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            TokenValidationParameters validationParameters = GetTokenValidationParameters(options);

            ClaimsPrincipal claimsPrincipal;
            try
            {
                claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(jwt, validationParameters, out SecurityToken token);
            }
            catch
            {
                result = new Dictionary<string, List<string>>();
                return false;
            }

            result = GetDictionaryOfClaims(claimsPrincipal.Claims);
            return true;
        }

        private Dictionary<string, List<string>> GetDictionaryOfClaims(IEnumerable<Claim> claims)
        {
            Dictionary<String, List<String>>  result = new Dictionary<String, List<String>>();

            foreach (Claim claim in claims)
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

            return result;
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
    }
}