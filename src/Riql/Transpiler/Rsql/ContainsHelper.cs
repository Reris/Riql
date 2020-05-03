using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Riql.Transpiler.Rsql
{
    public static class ContainsHelper
    {
        public static readonly MethodInfo StringContainsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] {typeof(string)})
                                                                 ?? throw new MissingMethodException(nameof(String), nameof(string.Contains));

        public static readonly MethodInfo StringStartsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), new[] {typeof(string)})
                                                                   ?? throw new MissingMethodException(nameof(String), nameof(string.StartsWith));

        public static readonly MethodInfo StringEndsWithMethod = typeof(string).GetMethod(nameof(string.EndsWith), new[] {typeof(string)})
                                                                 ?? throw new MissingMethodException(nameof(String), nameof(string.EndsWith));

        private static readonly ConcurrentDictionary<Type, ListContainsHelper> ListContainsHelpers = new ConcurrentDictionary<Type, ListContainsHelper>();

        public static ListContainsHelper GetOrRegistryContainsMethodInfo(Type type)
        {
            var result = ContainsHelper.ListContainsHelpers.GetOrAdd(type, t => new ListContainsHelper(type));
            return result;
        }


        public class ListContainsHelper
        {
            private static readonly MethodInfo CastMethod = ListContainsHelper.GetCastMethodInfo();

            private readonly Type _elementType;
            private readonly Type _listType;


            public ListContainsHelper(Type type)
            {
                this._elementType = type ?? throw new ArgumentNullException(nameof(type));
                this._listType = typeof(List<>).MakeGenericType(type);
                this.ContainsMethod = this._listType.GetMethod(nameof(List<object>.Contains), new[] {this._elementType})
                                      ?? throw new MissingMethodException(this._listType.Name, nameof(List<object>.Contains));
            }

            public MethodInfo ContainsMethod { get; }

            private static MethodInfo GetCastMethodInfo()
            {
                Expression<Func<IEnumerable<object>, IEnumerable<int>>> stub = a => a.Cast<int>();
                return ((MethodCallExpression) stub.Body).Method.GetGenericMethodDefinition();
            }

            public object Convert(List<object> values)
            {
                var castMethod = ListContainsHelper.CastMethod.MakeGenericMethod(this._elementType);
                var castedValues = castMethod.Invoke(null, new object[] {values});
                var result = Activator.CreateInstance(this._listType, castedValues);
                return result;
            }
        }
    }
}