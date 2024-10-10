using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebAppShared.Internal;
using WebAppShared.Utils;

namespace WebAppShared.Videos;

public class Ffmpeg(string workDir = null)
{
    private readonly ILogger _logger = Defaults.LoggerFactory.CreateLogger<Ffmpeg>();

    private OrderedDictionary _args = new()
    {
        // {"movflags", "faststart"}
    };

    private string _destinationPath;

    public async Task Exec()
    {
        string arguments = String.Join(' ', _args.Cast<DictionaryEntry>().Select(x => $"-{x.Key} {x.Value}")) + $" {_destinationPath}";
        
        await ExecuteCmd.RunAsync("ffmpeg", arguments, _logger, workDir);
    }

    public async Task ExecAsync()
    {
        await Task.Run(Exec);
    }

    public Ffmpeg Dest(string outputFilePath)
    {
        _destinationPath = outputFilePath;
        return this;
    }

    public Ffmpeg Arg(string key, string value, int? index = null)
    {
        if (index != null)
        {
            _args.Insert(index.Value, key, value);
        }
        else
        {
            _args.Add(key, value);
        }

        return this;
    }
}