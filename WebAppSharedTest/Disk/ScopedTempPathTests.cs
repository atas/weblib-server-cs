using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Moq;
using WebAppShared.Disk;
using WebAppSharedTest.TestLibs;
using Xunit.Abstractions;

namespace WebAppSharedTest.Disk;

public class ScopedTempPathTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void Get_ReturnsSamePathOnMultipleCalls()
    {
        using var scopedTempPath = new ScopedTempPath();
        var path1 = scopedTempPath.Get();
        var path2 = scopedTempPath.Get();

        Assert.Equal(path1, path2);
    }

    [Fact]
    public void Get_CreatesDirectory()
    {
        using var scopedTempPath = new ScopedTempPath();
        var path = scopedTempPath.Get();

        Assert.True(Directory.Exists(path));
    }
    
    [Fact]
    public void Dispose_DeletesDirectory()
    {
        var scopedTempPath = new ScopedTempPath();
        var path = scopedTempPath.Get();
        scopedTempPath.Dispose();

        Assert.False(Directory.Exists(path));
    }
    
    [Fact]
    public void Get_IsThreadSafe()
    {
        using var scopedTempPath = new ScopedTempPath();
        var paths = new ConcurrentBag<string>();

        Parallel.For(0, 100, _ =>
        {
            paths.Add(scopedTempPath.Get());
        });

        Assert.Single(paths.Distinct());
    }

    
}