using JetBrains.Annotations;
using WebLibServer.Types;

namespace WebLibServer.WebPush;

[UsedImplicitly]
public class WebPushTagBase(string name) : EnumClass<string>(name)
{
}