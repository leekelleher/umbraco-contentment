using System.Text.Json.Nodes;

namespace Umbraco.Extensions;

internal static class SystemTextJsonExtensions
{
    internal static Dictionary<string, object> ToDictionary(this JsonNode? node)
    {
        if (node is not JsonObject nodeObject)
        {
            return new Dictionary<string, object>();
        }

        var dictionary = new Dictionary<string, object>();

        foreach (var keyPair in nodeObject)
        {
            if (keyPair.Value is not null)
            {
                dictionary[keyPair.Key] = keyPair.Value;
            }
        }

        return dictionary;
    }
}
