using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using WebLibServer.Contexts;
using WebLibServer.Metrics;
using WebLibServer.WebSys.DI;

namespace WebLibServer.SharedLogic.Recaptcha;

[Service, UsedImplicitly]
public class RecaptchaValidator(
    IConnectionCx connectionCx,
    IRecaptchaConfig recaptchaConfig,
    IMetricsSvc metricsSvc,
    IHttpClientFactory httpClientFactory)
{
    private const string RecaptchaValidateUrl = "https://www.google.com/recaptcha/api/siteverify";

    public async Task<bool> Validate(string captchaResponse)
    {
        if (string.IsNullOrEmpty(captchaResponse)) return false;

        var recaptchaSecret = recaptchaConfig.Secret;

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("response", captchaResponse),
            new KeyValuePair<string, string>("secret", recaptchaSecret),
            new KeyValuePair<string, string>("remoteip", connectionCx.IpAddress)
        });

        var result = await httpClientFactory.CreateClient().PostAsync(RecaptchaValidateUrl, content);

        var resultBody = await result.Content.ReadAsStringAsync();
        var resultJson = JObject.Parse(resultBody);

        if (resultJson["success"] != null && resultJson["success"].ToObject<bool>())
        {
            metricsSvc.Collect("CaptchaSuccess", 1);
            return true;
        }

        metricsSvc.Collect("CaptchaFail", 1);

        return false;
    }
}