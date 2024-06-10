using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace pandx.Wheel.Miscellaneous;

public class SerializerService : ISerializerService
{
    public string Serialize<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() }
            }
        });
    }

    public string Serialize<T>(T obj, Type type)
    {
        return JsonConvert.SerializeObject(obj, type, new JsonSerializerSettings());
    }

    public T? Deserialize<T>(string str)
    {
        return JsonConvert.DeserializeObject<T>(str);
    }
}