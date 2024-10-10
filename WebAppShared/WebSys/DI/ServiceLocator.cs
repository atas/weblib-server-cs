using Microsoft.Extensions.DependencyInjection;

namespace WebAppShared.WebSys.DI;

public class ServiceLocator
{
    private static IServiceProvider _serviceProvider;
    private readonly IServiceProvider _currentServiceProvider;

    public ServiceLocator(IServiceProvider currentServiceProvider)
    {
        _currentServiceProvider = currentServiceProvider;
        _serviceProvider = currentServiceProvider;
    }

    public static ServiceLocator Current => new(_serviceProvider);

    public static void SetLocatorProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///     Example usage:
    ///     using var _ = ServiceLocator.Build(s =>
    ///     {
    ///     s.AddSingleton(mockBucket.Object);
    ///     s.AddSingleton(mockLazyConfig);
    ///     });
    /// </summary>
    public static ServiceProvider Build(Action<ServiceCollection> fn)
    {
        var services = new ServiceCollection();
        fn(services);
        var provider = services.BuildServiceProvider();
        SetLocatorProvider(services.BuildServiceProvider());

        return provider;
    }

    public object GetInstance(Type serviceType)
    {
        return _currentServiceProvider.GetRequiredService(serviceType);
    }

    public TService GetInstance<TService>()
    {
        return _currentServiceProvider.GetRequiredService<TService>();
    }

    public static TService Get<TService>()
    {
        return _serviceProvider.GetRequiredService<TService>();
    }
}