namespace WebAppShared.DI;

public interface IFactory<out T>
{
	T GetInstance();
}
