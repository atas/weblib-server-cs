using FirebaseAdmin.Messaging;
using WebLibServer.Extensions;

namespace WebLibServer.WebPush;

public static class FcmUtils
{
    public static List<Message> ConvertToFcmMessages(IEnumerable<WebPushPayloadBase> payloads)
    {
        return payloads.Select(p => new Message
        {
            Token = p.PushDevice.Token,
            Webpush = new WebpushConfig
            {
                Notification = new WebpushNotification
                {
                    Title = p.Title,
                    Body = p.Body,
                    Icon = p.Icon,
                    Tag = p.Tag.ToString(),
                    Data = new Dictionary<string, string>
                    {
                        { "url", p.Url },
                    }.Merge(p.Data)
                }
            }
        }).ToList();
    }
}