using JetBrains.Annotations;

namespace WebLibServer.WebPush;

[UsedImplicitly]
public class WebPushPayloadBase
{
    public WebPushPayloadBase()
    {
    }

    public WebPushPayloadBase(WebPushPayloadBase o)
    {
        Title = o.Title;
        Body = o.Body;
        Url = o.Url;
        Icon = o.Icon;
        Tag = o.Tag;
        PushDevice = o.PushDevice;
    }

    public string Title { get; set; }
    public string Body { get; set; }
    public string Url { get; set; }
    public string Icon { get; set; }

    public WebPushTagBase Tag { get; set; }

    public IPushDevice PushDevice { get; set; }

    public Dictionary<string, string> Data { get; set; }
}