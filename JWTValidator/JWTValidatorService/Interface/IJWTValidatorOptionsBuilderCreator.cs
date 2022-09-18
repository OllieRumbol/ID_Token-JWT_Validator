using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTValidatorService.Interface
{
    public interface IJWTValidatorOptionsBuilderCreator
    {
        IJWTValidatorOptionsBuilderFinisher WithSigningKeyFromSecret(String secret);

        IJWTValidatorOptionsBuilderFinisher WithSigningKeyFromOpenIdUrl(String openIdUrl);
    }
}
