using WebAppShared.Utils;

namespace WebAppSharedTest.Utils;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;


public class ExecuteCmdTests
{
    [Fact]
    public async Task RunAsync_WithEchoCommand_ReturnsCorrectOutput()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        string expectedOutput = "Hello World";

        // Act
        string result = await ExecuteCmd.RunAsync("/bin/echo", expectedOutput, loggerMock.Object);

        // Assert
        Assert.Equal(expectedOutput + "\n", result);  // echo command adds a newline
    }
    
    [Fact]
    public async Task RunAsync_WithExitCommand_ThrowsException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => ExecuteCmd.RunAsync("/bin/bash", "-c \"exit 1\"", loggerMock.Object));
    }

    [Fact]
    public async Task RunAsync_WithCatCommand_ReturnsFileContents()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        string testFilePath = "/tmp/testfile.txt";
        await File.WriteAllTextAsync(testFilePath, "Test content");

        // Act
        string output = await ExecuteCmd.RunAsync("cat", testFilePath, loggerMock.Object);

        // Assert
        Assert.Equal("Test content", output.Trim());
    }

    
    [Fact]
    public async Task RunAsync_SpecifiesWorkingDirectory_CommandExecutesInDirectory()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        string workDir = "/usr";

        // Act
        string output = await ExecuteCmd.RunAsync("pwd", "", loggerMock.Object, workDir);

        // Assert
        Assert.Equal(workDir, output.Trim());
    }

    
}