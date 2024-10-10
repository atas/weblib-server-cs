using WebLibServer.Exceptions;
using WebLibServer.Types;

namespace WebLibServerTest.Types;

public class SetOnceTests
{
    [Fact]
    public void TestSetOnce()
    {
        var setOnce = new SetOnce<string>
        {
            Value = "Hello"
        };

        Assert.Equal("Hello", setOnce.Value);
        Assert.Equal("Hello", setOnce);
        Assert.Throws<AlreadySetException>(() => setOnce.Value = "World");
        Assert.Equal("Hello", setOnce);
    }

    [Fact]
    public void TestSetOnceWithoutObjectInitializer()
    {
        var setOnce = new SetOnce<string>();

        Assert.Null(setOnce.Value);
        Assert.Null((string)setOnce);

        setOnce.Value = "Hello";
        Assert.Equal("Hello", setOnce.Value);
        Assert.Equal("Hello", setOnce);

        Assert.Throws<AlreadySetException>(() => setOnce.Value = "World");
        Assert.Equal("Hello", setOnce);
    }
}