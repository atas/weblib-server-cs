using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using WebAppShared.Internal;
using Xunit.Abstractions;

namespace WebAppSharedTest.TestLibs;

public static class TestGlobals
{
	public static void Initialize(ITestOutputHelper outputHelper)
	{
		var loggerProvider = new TestOutputLoggerProvider(outputHelper, new LoggingConfig()
		{
			LogLevel = LogLevel.Debug
		});

		WebAppSharedDefaults.SetLoggerFactory(new LoggerFactory(new List<ILoggerProvider> { loggerProvider }));
	}
}
