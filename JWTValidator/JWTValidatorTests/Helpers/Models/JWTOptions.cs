using System;
using System.Collections.Generic;

namespace JWTValidatorTests.Helpers.Models;

public class JWTOptions
{
    public String Secret { get; set; }

    public String Issuer { get; set; }

    public String Audience { get; set; }

    public DateTime ExpiryDate { get; set; }

    public List<KeyValuePair<String, String>> Claims { get; set; }
}