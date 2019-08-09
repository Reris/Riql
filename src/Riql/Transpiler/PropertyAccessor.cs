using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Riql.Transpiler
{
    public class PropertyAccessor
    {
        [NotNull]
        private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> JsonPropertyInfos =
            new ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>();

        [NotNull]
        private readonly NamingStrategy _namingStrategy;

        public PropertyAccessor()
            : this(new DefaultNamingStrategy())
        {
        }

        public PropertyAccessor([NotNull] NamingStrategy namingStrategy)
        {
            this._namingStrategy = namingStrategy ?? throw new ArgumentNullException(nameof(namingStrategy));
        }


        private Dictionary<string, PropertyInfo> MapProperties(Type type)
        {
            var result = new Dictionary<string, PropertyInfo>();
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var ignore = property.GetCustomAttribute<JsonIgnoreAttribute>();
                if (ignore == null)
                {
                    var jsonPropertyName = this.GetJsonPropertyName(property);
                    result.Add(jsonPropertyName, property);
                }
            }

            return result;
        }


        private string GetJsonPropertyName(MemberInfo propertyInfo)
        {
            var propertyName = propertyInfo.Name;
            var attribute = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
            if (attribute != null)
            {
                return attribute.PropertyName;
            }

            return this._namingStrategy.GetPropertyName(propertyName, false);
        }

        [CanBeNull]
        public PropertyInfo FindProperty([NotNull] Type type, [NotNull] string name)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            name = name ?? throw new ArgumentNullException(nameof(name));

            var properties = PropertyAccessor.JsonPropertyInfos.GetOrAdd(type, t => this.MapProperties(type));
            properties.TryGetValue(name, out var result);
            return result;
        }


        [NotNull]
        public ExpressionPath ParsePath([NotNull] ParameterExpression parameter, [NotNull] string propertyPath)
        {
            parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            propertyPath = propertyPath ?? throw new ArgumentNullException(nameof(propertyPath));

            Expression pathExpression = parameter;
            var expressionSteps = new List<MemberExpression>();
            foreach (var property in propertyPath.Split('.'))
            {
                MemberExpression currentPath;
                var nullableType = pathExpression.Type.FindNullableValueType();
                if (nullableType != null)
                {
                    pathExpression = currentPath = Expression.Property(pathExpression, nameof(Nullable<int>.Value));
                    expressionSteps.Add(currentPath);
                }

                pathExpression = currentPath = this.ParseProperty(pathExpression, property)
                                               ?? throw new PropertyNotFoundException($"There is no property '{property}' in {propertyPath}.");
                expressionSteps.Add(currentPath);
            }

            return new ExpressionPath(pathExpression, expressionSteps);
        }

        [CanBeNull]
        private MemberExpression ParseProperty([NotNull] Expression current, [NotNull] string property)
        {
            var propertyInfo = this.FindProperty(current.Type, property);
            return propertyInfo == null
                       ? null
                       : Expression.Property(current, propertyInfo);
        }
    }
}