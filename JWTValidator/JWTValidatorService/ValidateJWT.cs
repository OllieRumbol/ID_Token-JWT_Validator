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
        public static Boolean TryJWTValidation(String jwt, JWTValidatorOptions options, out String result)
        {

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            TokenValidationParameters validationParameters;
            if (String.IsNullOrEmpty(options.Secret) == false)
            {
                validationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = options.ValidateOnExpiryDate,
                };
            }
            else
            {
                IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(options.OpenIdUrl, new OpenIdConnectConfigurationRetriever());
                OpenIdConnectConfiguration openIdConfig = configurationManager.GetConfigurationAsync(System.Threading.CancellationToken.None).Result;

                validationParameters = new TokenValidationParameters
                {
                    IssuerSigningKeys = openIdConfig.SigningKeys,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = options.ValidateOnExpiryDate,
                };
            }

            Microsoft.IdentityModel.Tokens.SecurityToken token;
            ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(jwt, validationParameters, out token);

            String role = claimsPrincipal.Claims
                .Where(n => n.Type == "role")
                .Select(n => n.Value)
                .FirstOrDefault();

            result = null;
            return true;
        }
    }
}
