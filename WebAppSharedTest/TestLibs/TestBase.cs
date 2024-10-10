using Xunit.Abstractions;

namespace WebAppSharedTest.TestLibs;

public class TestBase
{
	protected readonly ITestOutputHelper Output;

	public TestBase(ITestOutputHelper output)
	{
		Output = output;
		TestGlobals.Initialize(Output);
	}
}
