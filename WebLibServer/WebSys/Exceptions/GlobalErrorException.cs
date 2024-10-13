using JetBrains.Annotations;

namespace WebLibServer.WebSys.Exceptions;

[UsedImplicitly]
public class GlobalErrorException(string msg) : Exception(msg)
{
    public string ViewName => "/Views/Errors/GlobalErrorException.cshtml";

    public ErrorViewModel GetModel()
    {
        return new ErrorViewModel
        {
            Message = Message
        };
    }
}