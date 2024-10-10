using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace WebAppShared.DI;

/// <summary>
/// Factory for getting instances of T from the serviceProvider.
/// </summary>
/// <param name="serviceProvider"></param>
/// <typeparam name="T"></typeparam>
[UsedImplicitly]
public class Factory<T>(IServiceProvider serviceProvider) : IFactory<T>
{
	public T GetInstance()
	{
		return serviceProvider.GetRequiredService<T>();
	}
}
