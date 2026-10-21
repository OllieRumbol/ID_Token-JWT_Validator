using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JWTValidatorService.Extensions;

public static class DictionaryExtensions
{
    public static String Print<TKey, TValue>(this Dictionary<TKey, List<TValue>>? dictionary)
    {
        if (dictionary.IsEmpty())
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

    public static Boolean ContainsKeyAndValue<TKey, TValue, T>(this Dictionary<TKey, List<TValue>>? dictionary, T key, T value)
    {
        if (dictionary.IsEmpty())
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

    public static Boolean ContainsValueInList<TKey, TValue, T>(this Dictionary<TKey, List<TValue>>? dictionary, T value )
    {
        if(dictionary.IsEmpty())
        {
            return false;
        }


        foreach (List<TValue> values in dictionary.Values)
        {
            Boolean result = values.Any(v => v.Equals(value));

            if(result == true)
            {
                return true;
            }
        }

        return false;
    }

    // NotNullWhen(false) tells the compiler that the dictionary is not null after a false result.
    public static Boolean IsEmpty<TKey, TValue>([NotNullWhen(false)] this Dictionary<TKey, List<TValue>>? dictionary)
    {
        return dictionary is null || dictionary.Count == 0;
    }
}