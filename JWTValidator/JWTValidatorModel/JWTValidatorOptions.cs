using Newtonsoft.Json.Linq;

namespace JWTValidatorModel
{
    public class JWTValidatorOptions
    {
        public String Secret { get; set; } = "";

        public String OpenIdUrl { get; set; } = "";

        public String Issuer { get; set; } = "";

        public String Audience { get; set; } = "";

        public Boolean ExpiryDate { get; set; }

        public String Print()
        {
            return JObject.FromObject(new
            {
                Secret = Secret,
                OpenIdUrl = OpenIdUrl,
                Issuer = Issuer,    
                Audience = Audience,
                ValidateExpiryDate = ExpiryDate,
            }).ToString();
        }
    }
}