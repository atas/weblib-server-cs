using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace WebLibServer.WebSys.DI;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
public class ServiceAttribute(ServiceAttribute.Scopes scope = ServiceAttribute.Scopes.Transient)
    : Attribute
{
    public enum Scopes
    {
        Transient,
        Scoped,
        Singleton
    }

    public readonly Scopes Scope = scope;

    public Type InterfaceToBind { get; set; } = null;

    public static void RegisterServices(Assembly assembly, IServiceCollection serviceCollection)
    {
        foreach (var type in assembly.GetTypes())
        {
            var serviceAttr = (ServiceAttribute)type
                .GetCustomAttributes(typeof(ServiceAttribute), true)
                .FirstOrDefault();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (serviceAttr == null) continue;

            var interfaceToBind = serviceAttr.InterfaceToBind ?? type;

            if (serviceAttr.Scope == ServiceAttribute.Scopes.Scoped)
                serviceCollection.AddScoped(interfaceToBind, type);
            else if (serviceAttr.Scope == ServiceAttribute.Scopes.Transient)
                serviceCollection.AddTransient(interfaceToBind, type);
            else if (serviceAttr.Scope == ServiceAttribute.Scopes.Transient)
                serviceCollection.AddSingleton(interfaceToBind, type);
        }
    }
}