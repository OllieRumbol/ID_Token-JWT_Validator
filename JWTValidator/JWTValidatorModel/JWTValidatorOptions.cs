using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTValidatorModel
{
    public class JWTValidatorOptions
    {
        public String Secret { get; set; } = "";

        public String OpenIdUrl { get; set; } = "";

        public String Issuer { get; set; } = "";

        public String Audience { get; set; } = "";

        public Boolean ExpiryDate { get; set; }
    }
}
