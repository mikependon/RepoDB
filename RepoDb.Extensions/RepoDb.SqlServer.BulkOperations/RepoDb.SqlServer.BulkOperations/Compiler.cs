using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb.SqlServer.BulkOperations
{
    /// <summary>
    /// An internal compiler class used to compile necessary expressions that is needed to enhance the code execution.
    /// </summary>
    internal static class Compiler
    {
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
            private static ConcurrentDictionary<int, Func<TEntity, TResult>> cache =
                new ConcurrentDictionary<int, Func<TEntity, TResult>>();

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
            private static ConcurrentDictionary<int, Action<TEntity>> cache =
                new ConcurrentDictionary<int, Action<TEntity>>();

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
            private static ConcurrentDictionary<int, Func<TEntity, object[], TResult>> cache =
                new ConcurrentDictionary<int, Func<TEntity, object[], TResult>>();

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
            private static ConcurrentDictionary<int, Action<TEntity, object[]>> cache =
                new ConcurrentDictionary<int, Action<TEntity, object[]>>();

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
            private static ConcurrentDictionary<int, Func<TEntity, TResult>> cache =
                new ConcurrentDictionary<int, Func<TEntity, TResult>>();

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
            PropertySetterFuncCache<TEntity>.GetFunc(PropertyCache.Get<TEntity>(propertyName));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        private static class PropertySetterFuncCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<int, Action<TEntity, object>> cache =
                new ConcurrentDictionary<int, Action<TEntity, object>>();

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
            private static ConcurrentDictionary<int, Func<TEntity, TResult>> cache =
                new ConcurrentDictionary<int, Func<TEntity, TResult>>();

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
            private static ConcurrentDictionary<int, Func<TEnum>> cache =
                new ConcurrentDictionary<int, Func<TEnum>>();

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
