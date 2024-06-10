using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace pandx.Wheel.Extensions;

public static class JsonExtensions
{
    public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = camelCase ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver()
        };

        if (indented)
        {
            settings.Formatting = Formatting.Indented;
        }

        return ToJsonString(obj, settings);
    }

    public static string ToJsonString(this object obj, JsonSerializerSettings settings)
    {
        return JsonConvert.SerializeObject(obj, settings);
    }
}