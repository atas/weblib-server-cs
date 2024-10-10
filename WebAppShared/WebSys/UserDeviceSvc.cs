using DeviceDetectorNET;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebAppShared.WebSys.DI;

namespace WebAppShared.WebSys;

[Service, UsedImplicitly]
public class UserDeviceSvc(IHttpContextAccessor httpContextAccessor)
{
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext;

    private UserDeviceInfo _userDeviceInfo;

    public UserDeviceInfo Info => _userDeviceInfo ?? GetInfoJson();

    public string GetUiJson()
    {
        return JsonConvert.SerializeObject(new
        {
            isBot = Info.IsBot
        });
    }

    private UserDeviceInfo GetInfoJson()
    {
        var dd = new DeviceDetector(_httpContext.Request.Headers.UserAgent);

        // OPTIONAL: Set caching method
        // By default static cache is used, which works best within one php process (memory array caching)
        // To cache across requests use caching in files or memcache
        // add using DeviceDetectorNET.Cache;
        // dd.SetCache(new DictionaryCache());

        // OPTIONAL: If called, GetBot() will only return true if a bot was detected  (speeds up detection a bit)
        // dd.DiscardBotInformation();

        dd.Parse();

        _userDeviceInfo = new UserDeviceInfo
        {
            IsBot = dd.IsBot()
        };

        return _userDeviceInfo;
    }
}