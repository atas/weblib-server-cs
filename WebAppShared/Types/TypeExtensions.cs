using System;
using System.Linq;

namespace WebAppShared.Types;

public static class TypeExtensions
{
    public static bool HasGenericInterface(this Type type, Type genericInterfaceType)
    {
        return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterfaceType);
    }
}