using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebLibServer.WebSys.MvcFilters;

public class CustomExceptionErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(new
        {
            error = this
        }, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    }
}