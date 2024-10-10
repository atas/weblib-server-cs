using JetBrains.Annotations;
using WebAppShared.Types;

namespace WebAppShared.WebPush;

[UsedImplicitly]
public class WebPushTagBase(string name) : EnumClass<string>(name)
{
}