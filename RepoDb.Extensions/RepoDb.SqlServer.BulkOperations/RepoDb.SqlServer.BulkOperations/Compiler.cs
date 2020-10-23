using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

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
            private static ConcurrentDictionary<int, Func<TEntity, TResult>> m_cache =
                new ConcurrentDictionary<int, Func<TEntity, TResult>>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="methodName"></param>
            /// <returns></returns>
            public static Func<TEntity, TResult> GetFunc(string methodName)
            {
                var func = (Func<TEntity, TResult>)null;
                if (m_cache.TryGetValue(methodName.GetHashCode(), out func) == false)
                {
                    var typeOfEntity = typeof(TEntity);
                    var method = typeOfEntity.GetMethod(methodName);
                    var entity = Expression.Parameter(typeOfEntity, "entity");
                    var body = Expression.Convert(Expression.Call(entity, method), typeof(TResult));

                    func = Expression
                        .Lambda<Func<TEntity, TResult>>(body, entity)
                        .Compile();
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
            private static ConcurrentDictionary<int, Action<TEntity>> m_cache =
                new ConcurrentDictionary<int, Action<TEntity>>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="methodName"></param>
            /// <returns></returns>
            public static Action<TEntity> GetFunc(string methodName)
            {
                var func = (Action<TEntity>)null;
                if (m_cache.TryGetValue(methodName.GetHashCode(), out func) == false)
                {
                    var typeOfEntity = typeof(TEntity);
                    var method = typeOfEntity.GetMethod(methodName);
                    var entity = Expression.Parameter(typeOfEntity, "entity");
                    var body = Expression.Call(entity, method);

                    func = Expression
                        .Lambda<Action<TEntity>>(body, entity)
                        .Compile();
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
            private static ConcurrentDictionary<int, Func<TEntity, object[], TResult>> m_cache =
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
                var func = (Func<TEntity, object[], TResult>)null;
                if (m_cache.TryGetValue(methodName.GetHashCode(), out func) == false)
                {
                    var typeOfEntity = typeof(TEntity);
                    var method = typeOfEntity.GetMethod(methodName, types);
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
            private static ConcurrentDictionary<int, Action<TEntity, object[]>> m_cache =
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
                var func = (Action<TEntity, object[]>)null;
                if (m_cache.TryGetValue(methodName.GetHashCode(), out func) == false)
                {
                    var typeOfEntity = typeof(TEntity);
                    var method = typeOfEntity.GetMethod(methodName, types);
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
        /// <param name="methodName"></param>
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
            private static ConcurrentDictionary<int, Func<TEntity, TResult>> m_cache =
                new ConcurrentDictionary<int, Func<TEntity, TResult>>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="classProperty"></param>
            /// <returns></returns>
            public static Func<TEntity, TResult> GetFunc(ClassProperty classProperty)
            {
                var func = (Func<TEntity, TResult>)null;
                if (m_cache.TryGetValue(classProperty.GetHashCode(), out func) == false)
                {
                    var typeOfEntity = typeof(TEntity);
                    var entity = Expression.Parameter(typeOfEntity, "entity");
                    var body = Expression.Convert(Expression.Call(entity, classProperty.PropertyInfo.GetMethod), typeof(TResult));

                    func = Expression
                        .Lambda<Func<TEntity, TResult>>(body, entity)
                        .Compile();
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
            private static ConcurrentDictionary<int, Action<TEntity, object>> m_cache =
                new ConcurrentDictionary<int, Action<TEntity, object>>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="property"></param>
            /// <returns></returns>
            public static Action<TEntity, object> GetFunc(ClassProperty property)
            {
                var func = (Action<TEntity, object>)null;
                if (m_cache.TryGetValue(property.GetHashCode(), out func) == false)
                {
                    var entity = Expression.Parameter(typeof(TEntity), "entity");
                    var value = Expression.Parameter(typeof(object), "value");
                    var converted = Expression.Convert(value, property.PropertyInfo.PropertyType);
                    var body = (Expression)Expression.Call(entity, property.PropertyInfo.SetMethod, converted);

                    func = Expression
                        .Lambda<Action<TEntity, object>>(body, entity, value)
                        .Compile();
                }
                return func;
            }
        }

        #endregion
    }
}
