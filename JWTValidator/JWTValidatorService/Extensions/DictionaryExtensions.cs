using System.Security.Claims;
using System.Text;

namespace JWTValidatorService.Extensions
{
    public static class DictionaryExtensions
    {
        public static String Print<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary)
        {
            StringBuilder bobTheBuilder = new StringBuilder();

            foreach (KeyValuePair<TKey, List<TValue>> kvp in dictionary)
            {
                bobTheBuilder.Append($"{kvp.Key}: {String.Join(", ", kvp.Value)} \n");
            }

            return bobTheBuilder.ToString();
        }

        public static Boolean DictionaryContainsKeyAndValue<TKey, TValue, T>(this Dictionary<TKey, List<TValue>> dictionary, T key, T value)
        {
            return dictionary
                .FirstOrDefault(x => x.Key.Equals(key))
                .Value
                .Any(x => x.Equals(value));
        }

        public static Dictionary<String, List<String>> ToDictionaryWithoutDuplicates(this IEnumerable<Claim> claims)
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
    }
}