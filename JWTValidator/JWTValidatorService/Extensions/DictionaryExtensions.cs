using System.Text;
using System.Linq;

namespace JWTValidatorService.Extensions;
public static class DictionaryExtensions
{
    public static String Print<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary)
    {
        if (dictionary is null || dictionary.Count == 0)
        {
            return String.Empty;
        }

        StringBuilder bobTheBuilder = new StringBuilder();

        foreach (KeyValuePair<TKey, List<TValue>> kvp in dictionary)
        {
            bobTheBuilder.Append($"{kvp.Key}: {String.Join(", ", kvp.Value)} \n");
        }

        return bobTheBuilder.ToString();
    }

    public static Boolean DictionaryContainsKeyAndValue<TKey, TValue, T>(this Dictionary<TKey, List<TValue>> dictionary, T key, T value)
    {
        if(dictionary is null || dictionary.Count == 0)
        {
            return false;
        }

        KeyValuePair<TKey, List<TValue>> result = dictionary.FirstOrDefault(x => x.Key.Equals(key));
        if(result.Key is null || result.Value is null)
        {
            return false;
        }

        return result.Value.Any(x => x.Equals(value));
    }
}