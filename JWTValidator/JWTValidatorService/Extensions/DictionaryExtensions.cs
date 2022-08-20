using System.Text;

namespace JWTValidatorService.Extensions;
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
}