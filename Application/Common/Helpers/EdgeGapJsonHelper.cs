using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Common.Helpers;

public static class EdgeGapJsonHelper<T>
{
    public static JsonContent Create(T sData)
    {
        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true
        };

        return JsonContent.Create(sData, options: serializeOptions);
    }

    public static T Deserialize(string data)
    {
        DefaultContractResolver contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };

        return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings() { ContractResolver = contractResolver });
    }
}
