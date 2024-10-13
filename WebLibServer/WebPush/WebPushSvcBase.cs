using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MoreLinq;
using WebLibServer.DI;
using WebLibServer.Metrics;
using WebLibServer.WebSys.DI;

namespace WebLibServer.WebPush;

public abstract class WebPushSvcBase(ILogger<WebPushSvcBase> logger)
{
    public async Task PushToDevices<T>(List<T> payloads) where T : WebPushPayloadBase
    {
        if (!payloads.Any()) return;

        var messages = FcmUtils.ConvertToFcmMessages(payloads);

        try
        {
            logger.LogInformation("{Event}: Sending {MsgCount} many web push notifications", BaseAppEvent.NotifsWebPush,
                messages.Count);

            var response = await FirebaseMessaging.DefaultInstance.SendEachAsync(messages);
            logger.LogInformation("WEB_PUSH_RESULT: success_count {SCount}, failure_count: {FCount}",
                response.SuccessCount,
                response.FailureCount);
            foreach (var sendResponse in response.Responses)
                if (!sendResponse.IsSuccess)
                    logger.LogError(sendResponse.Exception,
                        "WEB_PUSH_FAIL: MessagingErrorCode: {ErrCd}, MessageId: {MsgId}",
                        sendResponse.Exception.MessagingErrorCode, sendResponse.MessageId);

            CleanUpFailedTokens(payloads, response.Responses);
        }
        catch (FirebaseException ex)
        {
            logger.LogError("FirebaseException is thrown {Msg}", ex.Message);
            // TODO Parse Firebase exception and remove expired tokens
            // using var cx = new GlobalDbCx();
            // cx.PushDevices.Attach(device);
            // cx.PushDevices.Remove(device);
            // cx.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError("ERROR sending web push payload: {Msg}", e.Message);
        }
    }

    private void CleanUpFailedTokens<T>(List<T> payloads, IReadOnlyList<SendResponse> responses) where T : WebPushPayloadBase
    {
        var pushDeviceIdsToRemove = new List<int>();

        for (var i = 0; i < responses.Count; i++)
            if (!responses[i].IsSuccess)
                pushDeviceIdsToRemove.Add(payloads[i].PushDevice.Id);

        if (pushDeviceIdsToRemove.Count == 0) return;

        logger.LogInformation("WEB_PUSH_CLEANUP: pushDeviceIdsToRemove: {Ids}",
            pushDeviceIdsToRemove.ToDelimitedString(","));
        CleanupFailedPushDeviceIdsInDb(pushDeviceIdsToRemove);
    }

    protected abstract void CleanupFailedPushDeviceIdsInDb(List<int> pushDeviceIdsToRemove);
}