using Amazon.S3.Model;
using WebAppShared.Types;

namespace WebAppShared.Uploaders;

public class StorageBucketBase(IStorageConfig storageConfig)
{
    public string Videos => GetBucket("videos");
    public string Photos => GetBucket("photos");

    public string MediaHost => storageConfig.MediaHost;

    protected string GetBucket(string name)
    {
        var prefix = storageConfig.BucketNamePrefix ?? "";
        var suffix = storageConfig.BucketNameSuffix ?? "";
        return $"{prefix}{name}{suffix}";
    }
}