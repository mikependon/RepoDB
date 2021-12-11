using Npgsql;
using NpgsqlTypes;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.BulkOperations
{
    /// <summary>
    /// An internal compiler class used to compile necessary expressions that is needed to enhance the code execution.
    /// </summary>
    internal static class Compiler
    {
        #region GetNpgsqlBinaryImporterWriteFunc (Mappings)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        internal static Action<NpgsqlBinaryImporter, TEntity> GetNpgsqlBinaryImporterWriteFunc<TEntity>(string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType)
            where TEntity : class =>
            GetNpgsqlBinaryImporterWriteFuncCache<TEntity>.Get(tableName, mappings, entityType);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="entityType"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Action<NpgsqlBinaryImporter, TEntity> GetNpgsqlBinaryImporterWriteFunc<TEntity>(string tableName,
            IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            Type entityType,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting = null)
            where TEntity : class =>
            GetNpgsqlBinaryImporterWriteFuncCache<TEntity>.Get(tableName, dbFields, properties, entityType, identityBehavior, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        private static class GetNpgsqlBinaryImporterWriteFuncCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<int, Action<NpgsqlBinaryImporter, TEntity>> cache = new();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tableName"></param>
            /// <param name="mappings"></param>
            /// <param name="entityType"></param>
            /// <returns></returns>
            public static Action<NpgsqlBinaryImporter, TEntity> Get(string tableName,
                IEnumerable<NpgsqlBulkInsertMapItem> mappings,
                Type entityType) =>
                GetFunc(tableName, mappings, entityType);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tableName"></param>
            /// <param name="dbFields"></param>
            /// <param name="properties"></param>
            /// <param name="entityType"></param>
            /// <param name="identityBehavior"></param>
            /// <param name="dbSetting"></param>
            /// <returns></returns>
            public static Action<NpgsqlBinaryImporter, TEntity> Get(string tableName,
                IEnumerable<DbField> dbFields,
                IEnumerable<ClassProperty> properties,
                Type entityType,
                BulkImportIdentityBehavior identityBehavior,
                IDbSetting dbSetting = null)
            {
                var includeIdentity = (identityBehavior == BulkImportIdentityBehavior.KeepIdentity);
                var primaryDbField = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
                var isPrimaryAnIdentity = primaryDbField?.IsIdentity == true;
                var includePrimary = isPrimaryAnIdentity == false ||
                    (isPrimaryAnIdentity && identityBehavior == BulkImportIdentityBehavior.KeepIdentity);
                var matchedProperties = NpgsqlConnectionExtension.GetMatchedProperties(dbFields,
                    properties,
                    includePrimary,
                    includeIdentity,
                    dbSetting);
                var mappings = matchedProperties.Select(property =>
                    new NpgsqlBulkInsertMapItem(property.PropertyInfo.Name, property.GetMappedName()));

                return GetFunc(tableName, mappings, entityType);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tableName"></param>
            /// <param name="mappings"></param>
            /// <param name="entityType"></param>
            /// <returns></returns>
            private static Action<NpgsqlBinaryImporter, TEntity> GetFunc(string tableName,
                IEnumerable<NpgsqlBulkInsertMapItem> mappings,
                Type entityType)
            {
                var targetTableName = tableName ?? ClassMappedNameCache.Get<TEntity>();
                var hashCode = GetHashCode<TEntity>(targetTableName, mappings);

                if (cache.TryGetValue(hashCode, out var value))
                {
                    return value;
                }

                // Entity types (covering the anonymous)
                var typeOfEntity = typeof(TEntity);
                entityType ??= typeOfEntity;

                // Variables
                var importerType = typeof(NpgsqlBinaryImporter);
                var importerParameterExpression = Expression.Parameter(importerType, "importer");
                var entityParameterExpression = Expression.Parameter(typeOfEntity, "entity");
                var expressions = new List<Expression>();

                // Anonymous
                var entityExpression = (Expression)entityParameterExpression;
                if (typeOfEntity != entityType)
                {
                    entityExpression = Expression.Convert(entityParameterExpression, entityType);
                }

                // Mappings
                foreach (var mapping in mappings)
                {
                    var entityPropertyExpression = GetEntityPropertyExpression(entityExpression, entityType, mapping);
                    var propertyExpression = Expression.Convert(entityPropertyExpression, typeof(object));
                    var parameters = mapping.NpgsqlDbType.HasValue ?
                        new Expression[]
                        {
                            propertyExpression,
                            Expression.Constant(mapping.NpgsqlDbType)
                        } :
                        new[] { propertyExpression };
                    var writeMethod = mapping.NpgsqlDbType.HasValue ?
                        GetNpgsqlBinaryImporterWriteWithNpgsqlDbTypeMethod() : GetNpgsqlBinaryImporterWriteMethod();

                    expressions.Add(Expression.Call(importerParameterExpression, writeMethod.MakeGenericMethod(new[] { typeof(object) }), parameters));
                }

                // Check
                Action<NpgsqlBinaryImporter, TEntity> func;
                if (expressions.Any())
                {
                    func = Expression
                        .Lambda<Action<NpgsqlBinaryImporter, TEntity>>(Expression.Block(expressions), importerParameterExpression, entityParameterExpression)
                        .Compile();
                }
                else
                {
                    throw new InvalidOperationException($"There are no compiled expressions found for '{entityType.FullName}'. " +
                        $"Please check whether you had provided the proper 'mappings' or ensure that the entity properties are " +
                        $"matching with the table columns.");
                }

                // Cache
                if (cache.TryAdd(hashCode, func))
                {
                    return func;
                }

                // Throw an error
                throw new InvalidOperationException($"Failed to add a compiled '{importerType.FullName}.Write' function for '{tableName}'.");
            }
        }

        #endregion

        #region GetNpgsqlBinaryImporterWriteAsyncFunc (Mappings)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        internal static Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task> GetNpgsqlBinaryImporterWriteAsyncFunc<TEntity>(string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings,
            Type entityType)
            where TEntity : class =>
            GetNpgsqlBinaryImporterWriteAsyncFuncCache<TEntity>.Get(tableName, mappings, entityType);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <param name="properties"></param>
        /// <param name="entityType"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task> GetNpgsqlBinaryImporterWriteAsyncFunc<TEntity>(string tableName,
            IEnumerable<DbField> dbFields,
            IEnumerable<ClassProperty> properties,
            Type entityType,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting = null)
            where TEntity : class =>
            GetNpgsqlBinaryImporterWriteAsyncFuncCache<TEntity>.Get(tableName, dbFields, properties, entityType, identityBehavior, dbSetting);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        private static class GetNpgsqlBinaryImporterWriteAsyncFuncCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<int,
                Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task>> cache = new();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tableName"></param>
            /// <param name="mappings"></param>
            /// <param name="entityType"></param>
            /// <returns></returns>
            public static Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task> Get(string tableName,
                IEnumerable<NpgsqlBulkInsertMapItem> mappings,
                Type entityType) =>
                GetFunc(tableName, mappings, entityType);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tableName"></param>
            /// <param name="dbFields"></param>
            /// <param name="properties"></param>
            /// <param name="entityType"></param>
            /// <param name="identityBehavior"></param>
            /// <param name="dbSetting"></param>
            /// <returns></returns>
            public static Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task> Get(string tableName,
                IEnumerable<DbField> dbFields,
                IEnumerable<ClassProperty> properties,
                Type entityType,
                BulkImportIdentityBehavior identityBehavior,
                IDbSetting dbSetting = null)
            {
                var includeIdentity = (identityBehavior == BulkImportIdentityBehavior.KeepIdentity);
                var primaryDbField = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
                var isPrimaryAnIdentity = primaryDbField?.IsIdentity == true;
                var includePrimary = isPrimaryAnIdentity == false ||
                    (isPrimaryAnIdentity && identityBehavior == BulkImportIdentityBehavior.KeepIdentity);
                var matchedProperties = NpgsqlConnectionExtension.GetMatchedProperties(dbFields,
                    properties,
                    includePrimary,
                    includeIdentity,
                    dbSetting);
                var mappings = matchedProperties.Select(property =>
                    new NpgsqlBulkInsertMapItem(property.PropertyInfo.Name, property.GetMappedName()));

                return GetFunc(tableName, mappings, entityType);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tableName"></param>
            /// <param name="mappings"></param>
            /// <param name="entityType"></param>
            /// <returns></returns>
            private static Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task> GetFunc(string tableName,
                IEnumerable<NpgsqlBulkInsertMapItem> mappings,
                Type entityType)
            {
                var targetTableName = tableName ?? ClassMappedNameCache.Get<TEntity>();
                var hashCode = GetHashCode<TEntity>(targetTableName, mappings);

                if (cache.TryGetValue(hashCode, out var value))
                {
                    return value;
                }

                // Entity types (covering the anonymous)
                var typeOfEntity = typeof(TEntity);
                entityType ??= typeOfEntity;

                // Variables
                var importerType = typeof(NpgsqlBinaryImporter);
                var importerParameterExpression = Expression.Parameter(importerType, "importer");
                var entityParameterExpression = Expression.Parameter(typeOfEntity, "entity");
                var cancellationTokenExpression = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                var expressions = new List<Expression>();

                // Anonymous
                var entityExpression = (Expression)entityParameterExpression;
                if (typeOfEntity != entityType)
                {
                    entityExpression = Expression.Convert(entityParameterExpression, entityType);
                }

                // Mappings
                foreach (var mapping in mappings)
                {
                    var entityPropertyExpression = GetEntityPropertyExpression(entityExpression, entityType, mapping);
                    var propertyExpression = Expression.Convert(entityPropertyExpression, typeof(object));
                    var parameters = mapping.NpgsqlDbType.HasValue ?
                        new Expression[] { propertyExpression, Expression.Constant(mapping.NpgsqlDbType), cancellationTokenExpression } :
                        new Expression[] { propertyExpression, cancellationTokenExpression };
                    var writeMethod = mapping.NpgsqlDbType.HasValue ?
                        GetNpgsqlBinaryImporterWriteAsyncWithNpgsqlDbTypeMethod() : GetNpgsqlBinaryImporterWriteAsyncMethod();

                    expressions.Add(Expression.Call(importerParameterExpression, writeMethod.MakeGenericMethod(new[] { typeof(object) }), parameters));
                }

                // Check
                Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task> func;
                if (expressions.Any())
                {
                    func = Expression
                        .Lambda<Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task>>(Expression.Block(expressions),
                            importerParameterExpression, entityParameterExpression, cancellationTokenExpression)
                        .Compile();
                }
                else
                {
                    throw new InvalidOperationException($"There are no compiled expressions found for '{entityType.FullName}'. " +
                        $"Please check whether you had provided the proper 'mappings' or ensure that the entity properties are " +
                        $"matching with the table columns.");
                }

                // Cache
                if (cache.TryAdd(hashCode, func))
                {
                    return func;
                }

                // Throw an error
                throw new InvalidOperationException($"Failed to add a compiled '{importerType.FullName}.Write' function for '{tableName}'.");
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static MethodInfo GetNpgsqlBinaryImporterWriteMethod() =>
            typeof(NpgsqlBinaryImporter)
                .GetMethods()
                .Where(method =>
                    string.Equals("Write", method.Name, StringComparison.OrdinalIgnoreCase))
                .First(method => method.GetParameters().Length == 1);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static MethodInfo GetNpgsqlBinaryImporterWriteAsyncMethod() =>
            typeof(NpgsqlBinaryImporter)
                .GetMethods()
                .Where(method =>
                    string.Equals("WriteAsync", method.Name, StringComparison.OrdinalIgnoreCase))
                .First(method =>
                {
                    var parameters = method.GetParameters();
                    return parameters.Length == 2 &&
                        parameters[1].ParameterType == typeof(CancellationToken);
                });

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static MethodInfo GetNpgsqlBinaryImporterWriteWithNpgsqlDbTypeMethod()
        {
            var methods = typeof(NpgsqlBinaryImporter)
                .GetMethods()
                .Where(method => string.Equals("Write", method.Name, StringComparison.OrdinalIgnoreCase));

            return methods.First(method =>
            {
                var parameters = method.GetParameters();
                return parameters.Length == 2 &&
                    parameters[1].ParameterType == typeof(NpgsqlDbType);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static MethodInfo GetNpgsqlBinaryImporterWriteAsyncWithNpgsqlDbTypeMethod()
        {
            var methods = typeof(NpgsqlBinaryImporter)
                .GetMethods()
                .Where(method => string.Equals("WriteAsync", method.Name, StringComparison.OrdinalIgnoreCase));

            return methods.First(method =>
            {
                var parameters = method.GetParameters();
                return parameters.Length == 3 &&
                    parameters[1].ParameterType == typeof(NpgsqlDbType) &&
                    parameters[2].ParameterType == typeof(CancellationToken);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityExpression"></param>
        /// <param name="entityType"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        private static Expression GetEntityPropertyExpression(Expression entityExpression,
            Type entityType,
            NpgsqlBulkInsertMapItem mapping)
        {
            // Property
            var classProperty = PropertyCache.Get(entityType, mapping.SourceColumn);
            if (classProperty == null)
            {
                throw new PropertyNotFoundException($"Property '{mapping.SourceColumn}' is not found from type '{entityType.FullName}'.");
            }

            var propertyExpression = (Expression)Expression.Property(entityExpression, mapping.SourceColumn);

            // Enum
            if (classProperty.PropertyInfo.PropertyType.GetUnderlyingType().IsEnum)
            {
                propertyExpression = GetEntityPropertyExpressionForEnum(propertyExpression, mapping.NpgsqlDbType);
            }

            // Return
            return propertyExpression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <param name="npgsqlDbType"></param>
        /// <returns></returns>
        private static Expression GetEntityPropertyExpressionForEnum(Expression propertyExpression,
            NpgsqlDbType? npgsqlDbType)
        {
            var expression = npgsqlDbType switch
            {
                NpgsqlDbType.Text => ConvertEnumExpressionToString(propertyExpression),
                NpgsqlDbType.Integer => ConvertEnumExpressionToIntBasedType(propertyExpression, typeof(int)),
                NpgsqlDbType.Bigint => ConvertEnumExpressionToIntBasedType(propertyExpression, typeof(long)),
                NpgsqlDbType.Smallint => ConvertEnumExpressionToIntBasedType(propertyExpression, typeof(short)),
                _ => propertyExpression
            };

            if (propertyExpression.Type.IsNullable())
            {
                var underlyingType = expression.Type.GetUnderlyingType();
                var nullableType = underlyingType.IsValueType ?
                    typeof(Nullable<>).MakeGenericType(underlyingType) : underlyingType;
                var testExpression = Expression.Equal(Expression.Constant(null), propertyExpression);
                var trueExpression = Expression.Default(nullableType);
                var falseExpression = underlyingType.IsValueType && npgsqlDbType != NpgsqlDbType.Unknown ?
                    Expression.New(nullableType.GetConstructor(new[] { underlyingType }), expression) : expression;
                expression = Expression.Condition(testExpression, trueExpression, falseExpression);
            }

            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static Expression ConvertEnumExpressionToString(Expression propertyExpression) =>
            Expression.Call(GetConvertToTypeMethod(typeof(string)),
                Expression.Convert(propertyExpression, typeof(object)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <param name="intBasedType"></param>
        /// <returns></returns>
        private static Expression ConvertEnumExpressionToIntBasedType(Expression propertyExpression,
            Type intBasedType)
        {
            var typeExpression = Expression.Constant(propertyExpression.Type.GetUnderlyingType());
            var nameExpression = Expression.Call(GetEnumGetNameMethod(),
                Expression.Constant(propertyExpression.Type.GetUnderlyingType()),
                Expression.Convert(propertyExpression, typeof(object)));
            var ignoreCaseExpression = Expression.Constant(true);
            var valueExpression = Expression.Call(GetEnumParseMethod(), typeExpression, nameExpression, ignoreCaseExpression);
            return Expression.Call(GetConvertToTypeMethod(intBasedType), valueExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static MethodInfo GetEnumParseMethod() =>
            typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string), typeof(bool) });

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static MethodInfo GetEnumGetNameMethod() =>
            typeof(Enum).GetMethod("GetName", new[] { typeof(Type), typeof(object) });

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static MethodInfo GetConvertToTypeMethod(Type type) =>
            typeof(Convert).GetMethod($"To{type.Name}", new[] { typeof(object) });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static int GetHashCode(Type entityType,
            string tableName) =>
            (tableName ?? ClassMappedNameCache.Get(entityType)).GetHashCode();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        private static int GetHashCode<TEntity>(string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings) =>
            GetHashCode(typeof(TEntity), tableName, mappings);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="tableName"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        private static int GetHashCode(Type entityType,
            string tableName,
            IEnumerable<NpgsqlBulkInsertMapItem> mappings)
        {
            var hashCode = GetHashCode(entityType, tableName);

            if (mappings?.Any() == true)
            {
                foreach (var mapping in mappings)
                {
                    hashCode += HashCode.Combine(hashCode, mapping.GetHashCode());
                }
            }

            return hashCode;
        }


        #endregion





        // TODO: Remove

        #region GetMethodFunc

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static Func<TEntity, TResult> GetMethodFunc<TEntity, TResult>(string methodName)
            where TEntity : class =>
            MethodFuncCache<TEntity, TResult>.GetFunc(methodName);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        private static class MethodFuncCache<TEntity, TResult>
            where TEntity : class
        {
            private static ConcurrentDictionary<int, Func<TEntity, TResult>> cache = new();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="methodName"></param>
            /// <returns></returns>
            public static Func<TEntity, TResult> GetFunc(string methodName)
            {
                if (cache.TryGetValue(methodName.GetHashCode(), out var func) == false)
                {
                    var typeOfEntity = typeof(TEntity);
                    var method = typeOfEntity.GetMethod(methodName);

                    if (method != null)
                    {
                        var entity = Expression.Parameter(typeOfEntity, "entity");
                        var body = Expression.Convert(Expression.Call(entity, method), typeof(TResult));

                        func = Expression
                            .Lambda<Func<TEntity, TResult>>(body, entity)
                            .Compile();
                    }

                    cache.TryAdd(methodName.GetHashCode(), func);
                }
                return func;
            }
        }

        #endregion

        #region GetVoidMethodFunc

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static Action<TEntity> GetMethodFunc<TEntity>(string methodName)
            where TEntity : class =>
            VoidMethodFuncCache<TEntity>.GetFunc(methodName);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        private static class VoidMethodFuncCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<int, Action<TEntity>> cache = new();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="methodName"></param>
            /// <returns></returns>
            public static Action<TEntity> GetFunc(string methodName)
            {
                if (cache.TryGetValue(methodName.GetHashCode(), out var func) == false)
                {
                    var typeOfEntity = typeof(TEntity);
                    var method = typeOfEntity.GetMethod(methodName);

                    if (method != null)
                    {
                        var entity = Expression.Parameter(typeOfEntity, "entity");
                        var body = Expression.Call(entity, method);

                        func = Expression
                            .Lambda<Action<TEntity>>(body, entity)
                            .Compile();
                    }

                    cache.TryAdd(methodName.GetHashCode(), func);
                }
                return func;
            }
        }

        #endregion

        #region GetParameterizedMethodFunc

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Func<TEntity, object[], TResult> GetParameterizedMethodFunc<TEntity, TResult>(string methodName,
            Type[] types)
            where TEntity : class =>
            ParameterizedMethodFuncCache<TEntity, TResult>.GetFunc(methodName, types);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        private static class ParameterizedMethodFuncCache<TEntity, TResult>
            where TEntity : class
        {
            private static ConcurrentDictionary<int, Func<TEntity, object[], TResult>> cache = new();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="methodName"></param>
            /// <param name="types"></param>
            /// <returns></returns>
            public static Func<TEntity, object[], TResult> GetFunc(string methodName,
                Type[] types)
            {
                var key = methodName.GetHashCode() + types?.Sum(e => e.GetHashCode());
                if (cache.TryGetValue(key.Value, out var func) == false)
                {
                    var typeOfEntity = typeof(TEntity);
                    var method = typeOfEntity.GetMethod(methodName, types);

                    if (method != null)
                    {
                        var entity = Expression.Parameter(typeOfEntity, "entity");
                        var arguments = Expression.Parameter(typeof(object[]), "arguments");
                        var parameters = new List<Expression>();

                        for (var index = 0; index < types.Length; index++)
                        {
                            parameters.Add(Expression.Convert(Expression.ArrayIndex(arguments, Expression.Constant(index)), types[index]));
                        }

                        var body = Expression.Convert(Expression.Call(entity, method, parameters), typeof(TResult));

                        func = Expression
                            .Lambda<Func<TEntity, object[], TResult>>(body, entity, arguments)
                            .Compile();
                    }

                    cache.TryAdd(key.Value, func);
                }
                return func;
            }
        }

        #endregion

        #region GetParameterizedVoidMethodFunc

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Action<TEntity, object[]> GetParameterizedVoidMethodFunc<TEntity>(string methodName,
            Type[] types)
            where TEntity : class =>
            ParameterizedVoidMethodFuncCache<TEntity>.GetFunc(methodName, types);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        private static class ParameterizedVoidMethodFuncCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<int, Action<TEntity, object[]>> cache = new();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="methodName"></param>
            /// <param name="types"></param>
            /// <returns></returns>
            public static Action<TEntity, object[]> GetFunc(string methodName,
                Type[] types)
            {
                var key = methodName.GetHashCode() + types?.Sum(e => e.GetHashCode());
                if (cache.TryGetValue(key.Value, out var func) == false)
                {
                    var typeOfEntity = typeof(TEntity);
                    var method = typeOfEntity.GetMethod(methodName, types);

                    if (method != null)
                    {
                        var entity = Expression.Parameter(typeOfEntity, "entity");
                        var arguments = Expression.Parameter(typeof(object[]), "arguments");
                        var parameters = new List<Expression>();

                        for (var index = 0; index < types.Length; index++)
                        {
                            parameters.Add(Expression.Convert(Expression.ArrayIndex(arguments, Expression.Constant(index)), types[index]));
                        }

                        var body = Expression.Call(entity, method, parameters);

                        func = Expression
                            .Lambda<Action<TEntity, object[]>>(body, entity, arguments)
                            .Compile();
                    }

                    cache.TryAdd(key.Value, func);
                }
                return func;
            }
        }

        #endregion

        #region GetPropertyGetterFunc

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Func<TEntity, TResult> GetPropertyGetterFunc<TEntity, TResult>(string propertyName)
            where TEntity : class =>
            PropertyGetterFuncCache<TEntity, TResult>.GetFunc(PropertyCache.Get<TEntity>(propertyName));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        private static class PropertyGetterFuncCache<TEntity, TResult>
            where TEntity : class
        {
            private static ConcurrentDictionary<int, Func<TEntity, TResult>> cache = new();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="classProperty"></param>
            /// <returns></returns>
            public static Func<TEntity, TResult> GetFunc(ClassProperty classProperty)
            {
                if (cache.TryGetValue(classProperty.GetHashCode(), out var func) == false)
                {
                    var typeOfEntity = typeof(TEntity);
                    var entity = Expression.Parameter(typeOfEntity, "entity");
                    var body = Expression.Convert(Expression.Call(entity, classProperty.PropertyInfo.GetMethod), typeof(TResult));

                    func = Expression
                        .Lambda<Func<TEntity, TResult>>(body, entity)
                        .Compile();

                    cache.TryAdd(classProperty.GetHashCode(), func);
                }
                return func;
            }
        }

        #endregion

        #region GetPropertySetterFunc

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Action<TEntity, object> GetPropertySetterFunc<TEntity>(string propertyName)
            where TEntity : class =>
            PropertySetterFuncCache<TEntity>.GetFunc(PropertyCache.Get<TEntity>(propertyName, true));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        private static class PropertySetterFuncCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<int, Action<TEntity, object>> cache = new();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="classProperty"></param>
            /// <returns></returns>
            public static Action<TEntity, object> GetFunc(ClassProperty classProperty)
            {
                if (classProperty == null)
                {
                    return null;
                }

                if (cache.TryGetValue(classProperty.GetHashCode(), out var func) == false)
                {
                    if (classProperty != null)
                    {
                        var entity = Expression.Parameter(typeof(TEntity), "entity");
                        var value = Expression.Parameter(typeof(object), "value");
                        var converted = Expression.Convert(value, classProperty.PropertyInfo.PropertyType);
                        var body = (Expression)Expression.Call(entity, classProperty.PropertyInfo.SetMethod, converted);

                        func = Expression
                            .Lambda<Action<TEntity, object>>(body, entity, value)
                            .Compile();
                    }

                    cache.TryAdd(classProperty.GetHashCode(), func);
                }
                return func;
            }
        }

        #endregion

        #region GetFieldGetterFunc

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Func<TEntity, TResult> GetFieldGetterFunc<TEntity, TResult>(string fieldName)
            where TEntity : class =>
            FieldGetterFuncCache<TEntity, TResult>.GetFunc(fieldName);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        private static class FieldGetterFuncCache<TEntity, TResult>
            where TEntity : class
        {
            private static ConcurrentDictionary<int, Func<TEntity, TResult>> cache = new();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fieldName"></param>
            /// <returns></returns>
            public static Func<TEntity, TResult> GetFunc(string fieldName)
            {
                if (cache.TryGetValue(fieldName.GetHashCode(), out var func) == false)
                {
                    var typeOfEntity = typeof(TEntity);
                    var fieldInfo = typeOfEntity
                        .GetField("_rowsCopied", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        var entity = Expression.Parameter(typeOfEntity, "entity");
                        var field = Expression.Field(entity, fieldInfo);
                        var body = Expression.Convert(field, typeof(TResult));

                        func = Expression
                            .Lambda<Func<TEntity, TResult>>(body, entity)
                            .Compile();
                    }

                    cache.TryAdd(fieldName.GetHashCode(), func);
                }
                return func;
            }
        }

        #endregion

        #region GetEnumFunc

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Func<TEnum> GetEnumFunc<TEnum>(string value)
            where TEnum : Enum =>
            EnumFuncCache<TEnum>.GetFunc(value);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        private static class EnumFuncCache<TEnum>
            where TEnum : Enum
        {
            private static ConcurrentDictionary<int, Func<TEnum>> cache = new();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static Func<TEnum> GetFunc(string value)
            {
                if (cache.TryGetValue(value.GetHashCode(), out var func) == false)
                {
                    var typeOfEnum = typeof(TEnum);
                    var fieldInfo = typeOfEnum.GetField(value);

                    if (fieldInfo != null)
                    {
                        var body = Expression.Field(null, fieldInfo);

                        func = Expression
                            .Lambda<Func<TEnum>>(body)
                            .Compile();
                    }

                    cache.TryAdd(value.GetHashCode(), func);
                }
                return func;
            }
        }

        #endregion

        #region SetProperty

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetProperty<TEntity>(TEntity instance,
            string propertyName,
            object value)
            where TEntity : class
        {
            var propertySetter = Compiler.GetPropertySetterFunc<TEntity>(propertyName);
            propertySetter?.Invoke(instance, value);
        }

        #endregion
    }
}
