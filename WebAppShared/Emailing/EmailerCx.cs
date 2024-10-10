using JetBrains.Annotations;
using WebAppShared.Metrics;
using WebAppShared.MVC;

namespace WebAppShared.Emailing;

[UsedImplicitly]
public class EmailerCx
{
    public IViewRenderService ViewRenderService { get; set; }
    public EmailerConfigBase Config { get; set; }
    public IMetricsSvc MetricsSvc { get; set; }

    /// <summary>
    ///  Used to determine which EmailTemplate to use and construct unsubscribe links
    ///  ViewRender template will be used at EmailTemplates/{EmailType}Email (without .cshtml)
    /// </summary>
    public EmailTypeBase EmailType { get; set; }

    public bool HasListUnsubscribeHeader { get; set; }

    /// <summary>
    ///     Used to add the List-Unsubscribe header to emails
    /// </summary>
    public string UserSecret { get; private set; }
    
    /// <summary>
    ///     Adds the unsubscribe email headers to the email
    /// </summary>
    /// <param name="userSecret">Used to add the List-Unsubscribe header to emails</param>
    public void AddListUnsubscribeHeader(string userSecret)
    {
        HasListUnsubscribeHeader = true;
        UserSecret = userSecret;
    }

    public EmailerCx Clone()
    {
        return (EmailerCx)MemberwiseClone();
    }
}