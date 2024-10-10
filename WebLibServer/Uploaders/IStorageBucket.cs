namespace WebLibServer.Uploaders;

public interface IStorageBucket
{
	string Videos { get; }
	string Photos { get; }
	string Sprites { get; }

	public string MediaHost { get; }
}
