using Microsoft.Extensions.Logging;

namespace WebLibServer.Internal;

public static class WebLibServerDefaults
{
	public static void SetLoggerFactory(ILoggerFactory loggerFactory)
	{
		Defaults.LoggerFactory = loggerFactory;
	}
}
