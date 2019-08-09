using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace Riql.Tests
{
    public static class EnumerableExtensions
    {
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static T Random<T>([NotNull] this IEnumerable<T> employees)
        {
            return employees.Skip(new Random().Next(employees.Count())).First();
        }
    }
}