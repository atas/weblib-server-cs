namespace WebLibServer.DI;

public interface IFactory<out T>
{
	T GetInstance();
}
