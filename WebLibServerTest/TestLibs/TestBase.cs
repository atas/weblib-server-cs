﻿using Xunit.Abstractions;

namespace WebLibServerTest.TestLibs;

public class TestBase
{
	protected readonly ITestOutputHelper Output;

	public TestBase(ITestOutputHelper output)
	{
		Output = output;
		TestGlobals.Initialize(Output);
	}
}
