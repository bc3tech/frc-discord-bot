namespace Common.Extensions;
using System.Text.Json;

public static class JsonElementExtensions
{
    public static bool TryGetPropertyAnywhere(this JsonElement elt, string propertyName, out IReadOnlyCollection<JsonElement>? found)
    {
        List<JsonElement> foundSoFar = [];
        FindAllPropertiesWithName(elt, propertyName, foundSoFar);
        found = foundSoFar;
        return foundSoFar.Count > 0;
    }

    private static void FindAllPropertiesWithName(this JsonElement elt, string propertyName, IList<JsonElement> foundSoFar)
    {
        if (elt.ValueKind is JsonValueKind.Object)
        {
            foreach (var property in elt.EnumerateObject())
            {
                if (property.Name == propertyName)
                {
                    foundSoFar.Add(property.Value);
                }
                else
                {
                    FindAllPropertiesWithName(property.Value, propertyName, foundSoFar);
                }
            }
        }
        else if (elt.ValueKind is JsonValueKind.Array)
        {
            foreach (var item in elt.EnumerateArray())
            {
                FindAllPropertiesWithName(item, propertyName, foundSoFar);
            }
        }
    }
}
