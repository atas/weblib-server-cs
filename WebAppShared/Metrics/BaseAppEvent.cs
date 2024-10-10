using WebAppShared.Types;

namespace WebAppShared.Metrics;

public class BaseAppEvent(string value) : EnumClass<string>(value)
{
    public static readonly BaseAppEvent UserException = new("UserException");
    public static readonly BaseAppEvent CriticalException = new("CriticalException");
    public static readonly BaseAppEvent NotifsWebPush = new("NotifsWebPush");
}
