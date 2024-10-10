using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace WebAppShared.Utils;

public static class ExecuteCmd
{
    public static async Task<string> RunAsync(string proc, string args, ILogger logger, string workDir = null, bool useShellExecute = false)
    {
        logger.LogInformation("Executing {FileName} with arguments: {Arguments}", proc, args);

        var info = new ProcessStartInfo
        {
            FileName = proc,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = useShellExecute,
            CreateNoWindow = true
        };
        if (workDir != null) info.WorkingDirectory = workDir;

        using var process = new Process();
        process.StartInfo = info;
        process.Start();

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorsTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        string output = await outputTask;
        string errors = await errorsTask;
        int exitCode = process.ExitCode;
        
        logger.LogInformation("Executing: {Proc} {Arguments}: Output: {Output}", proc, args, output);

        if (exitCode != 0)
        {
            logger.LogError("Failed executing {Proc} with args {Arguments}: ExitCode: {ExitCode}, Errors: {Errors}",
                proc, args, exitCode, errors);
            throw new Exception("Failed executing " + proc + " with args: " + args + ". ExitCode: " + exitCode +
                                " Errors: " + errors);
        }

        return output;
    }
}