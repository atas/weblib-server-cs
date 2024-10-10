using Microsoft.Extensions.Logging;

namespace WebAppShared.Internal;

public static class WebAppSharedDefaults
{
	public static void SetLoggerFactory(ILoggerFactory loggerFactory)
	{
		Defaults.LoggerFactory = loggerFactory;
	}
}
