using System;

namespace Riql.Transpiler;

public static class TypeExtensions
{
    public static Type? FindNullableValueType(this Type type)
    {
        type = type ?? throw new ArgumentNullException(nameof(type));
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
        {
            return null;
        }

        return type.GetGenericArguments()[0];
    }
}