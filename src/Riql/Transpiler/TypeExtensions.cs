using System;
using JetBrains.Annotations;

namespace Riql.Transpiler
{
    public static class TypeExtensions
    {
        [CanBeNull]
        public static Type FindNullableValueType([NotNull] this Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                return null;
            }

            return type.GetGenericArguments()[0];
        }
    }
}