using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb.Oracle.BulkOperations
{
    /// <summary>
    /// Compiles and caches fast (expression-tree-based) property-value getters, used to pull each row's
    /// column values out of an entity during array-bind loading without paying reflection's per-call cost.
    /// This is a deliberately smaller version of the PostgreSQL bulk package's <c>Compiler</c> class -
    /// Oracle's array binding writes plain boxed .NET values into <see cref="Oracle.ManagedDataAccess.Client.OracleParameter"/>
    /// arrays (ODP.NET does the wire-format conversion itself), so there is no need for the low-level
    /// binary-protocol writer delegates that PostgreSQL's <c>NpgsqlBinaryImporter</c> requires.
    /// </summary>
    internal static class Compiler
    {
        private static readonly ConcurrentDictionary<(Type EntityType, string PropertyName), Func<object, object>> getterCache = new();

        /// <summary>
        /// Gets (compiling and caching on first use) a delegate that reads the value of <paramref name="propertyInfo"/>
        /// off a boxed instance of <paramref name="entityType"/>.
        /// </summary>
        public static Func<object, object> GetPropertyGetter(Type entityType,
            PropertyInfo propertyInfo)
        {
            var key = (entityType, propertyInfo.Name);

            if (getterCache.TryGetValue(key, out var getter))
            {
                return getter;
            }

            getter = CompilePropertyGetter(entityType, propertyInfo);
            getterCache.TryAdd(key, getter);

            return getter;
        }

        private static Func<object, object> CompilePropertyGetter(Type entityType,
            PropertyInfo propertyInfo)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var typedInstance = entityType.IsValueType ?
                (Expression)Expression.Unbox(instanceParameter, entityType) :
                Expression.Convert(instanceParameter, entityType);
            var propertyAccess = Expression.Property(typedInstance, propertyInfo);
            var boxedResult = Expression.Convert(propertyAccess, typeof(object));

            return Expression
                .Lambda<Func<object, object>>(boxedResult, instanceParameter)
                .Compile();
        }

        /// <summary>
        /// Builds one compiled getter per mapped (database) column name for the given entity type, for fast
        /// per-row value extraction during array-bind loading.
        /// </summary>
        public static IDictionary<string, Func<object, object>> GetPropertyGettersByMappedName(Type entityType)
        {
            var map = new Dictionary<string, Func<object, object>>(StringComparer.OrdinalIgnoreCase);

            foreach (var property in PropertyCache.Get(entityType))
            {
                map[property.GetMappedName()] = GetPropertyGetter(entityType, property.PropertyInfo);
            }

            return map;
        }
    }
}
