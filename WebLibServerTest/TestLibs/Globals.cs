using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using WebLibServer.Internal;
using Xunit.Abstractions;

namespace WebLibServerTest.TestLibs;

public static class TestGlobals
{
	public static void Initialize(ITestOutputHelper outputHelper)
	{
		var loggerProvider = new TestOutputLoggerProvider(outputHelper, new LoggingConfig()
		{
			LogLevel = LogLevel.Debug
		});

		WebLibServerDefaults.SetLoggerFactory(new LoggerFactory(new List<ILoggerProvider> { loggerProvider }));
	}
}
