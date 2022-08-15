using System.Text;

namespace JWTValidatorService.Extensions
{
    public static class DictionaryExtensions
    {
        public static String Print(this Dictionary<String, List<String>> dictionary)
        {
            StringBuilder bobTheBuilder = new StringBuilder();

            foreach (KeyValuePair<String, List<String>> kvp in dictionary)
            {
                bobTheBuilder.Append($"{kvp.Key}: {String.Join(", ", kvp.Value)} \n");
            }

            return bobTheBuilder.ToString();
        }

        public static Boolean DictionaryContainsKeyAndValue(this Dictionary<String, List<String>> dictionary, String key, String value)
        {
            return dictionary
                .FirstOrDefault(x => x.Key == key)
                .Value
                .Any(x => x == value);
        }
    }
}