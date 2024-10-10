using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using WebAppShared.Internal;
using WebAppShared.Utils;
using WebAppShared.WebSys.DI;

namespace WebAppShared.Disk;

/// <summary>
/// Creates a temporary directory that is deleted when the object is disposed.
/// </summary>
/// <param name="tmpDir"></param>
[Service, UsedImplicitly]
public class ScopedTempPath(string tmpDir = "/tmp") : IDisposable
{
    private readonly ILogger _logger = Defaults.LoggerFactory.CreateLogger<ScopedTempPath>();

    private volatile string _path;

    private readonly object _lock = new();

    public string Get()
    {
        if (_path != null) return _path;

        lock (_lock)
        {
            if (_path != null) return _path;

            var pathLength = 4;
            do
            {
                var fullPath = Path.Join(tmpDir, StringUtils.RandomString(Math.Min(pathLength++, 10)));
                if (Directory.Exists(fullPath)) continue;
                _path = fullPath;
                break;
            } while (true);
        }

        Directory.CreateDirectory(_path);

        return _path;
    }

    public void Dispose()
    {
        _logger.LogDebug("Disposing ScopedTempPath {Path}", _path);
        if (Directory.Exists(_path))
        {
            _logger.LogInformation("Deleting ScopedTempPath {Path}", _path);
            Directory.Delete(_path, true);
        }
    }
    
    public static implicit operator string(ScopedTempPath scopedTempPath) => scopedTempPath.Get();
    
    public override string ToString() => Get();

    ~ScopedTempPath()
    {
        _logger.LogDebug("Destructing ScopedTempPath {Path}", _path);
        Dispose();
    }
}