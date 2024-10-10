using Amazon.Runtime;
using Microsoft.Extensions.Logging;

namespace WebAppShared.Internal;

internal static class Defaults
{
	public static ILoggerFactory LoggerFactory { get; set; }
	public static HttpClientFactory HttpClientFactory { get; set; }
}
