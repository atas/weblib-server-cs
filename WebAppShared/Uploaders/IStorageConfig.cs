namespace WebAppShared.Uploaders;

public interface IStorageConfig
{
    string MediaHost { get; }
    string BucketNamePrefix { get; }
    string BucketNameSuffix { get; }
}