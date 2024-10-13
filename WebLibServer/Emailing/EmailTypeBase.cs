using WebLibServer.Types;

namespace WebLibServer.Emailing;

public abstract class EmailTypeBase(string value) : EnumClass<string>(value)
{
}
