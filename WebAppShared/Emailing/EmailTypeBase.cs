using WebAppShared.Types;

namespace WebAppShared.Emailing;

public abstract class EmailTypeBase(string value) : EnumClass<string>(value)
{
}
