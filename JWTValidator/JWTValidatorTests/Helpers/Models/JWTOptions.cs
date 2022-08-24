using System;
using System.Collections.Generic;

namespace JWTValidatorTests.Helpers.Models
{
    public class JWTOptions
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public DateTime ExpiryDate { get; set; }

        public List<KeyValuePair<String, String>> Claims { get; set; }
    }
}