using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class used for manipulating class objects via expressions.
    /// </summary>
    public static partial class ClassExpression
    {
        /// <summary>
        /// Extracts a dynamic object value to become an enumerable of objects.
        /// </summary>
        /// <typeparam name="TEntity">The target type of the class.</typeparam>
        /// <param name="obj">The object to be extracted.</param>
        /// <returns>The extracted values.</returns>
        public static IEnumerable<PropertyValue> Extract<TEntity>(TEntity obj)
            where TEntity : class
        {
            return Extractor<TEntity>.Extract(obj);
        }

        /// <summary>
        /// Extracts a dynamic object value to become an enumerable of objects.
        /// </summary>
        /// <typeparam name="TEntity">The target type of the class.</typeparam>
        /// <param name="obj">The object to be extracted.</param>
        /// <param name="command">The target command for extraction.</param>
        /// <returns>The extracted values.</returns>
        public static IEnumerable<PropertyValue> Extract<TEntity>(TEntity obj, Command command)
            where TEntity : class
        {
            switch (command)
            {
                case Command.BatchQuery:
                    return ExtractorForBatchQuery<TEntity>.Extract(obj);
                case Command.BulkInsert:
                    return ExtractorForBulkInsert<TEntity>.Extract(obj);
                case Command.Count:
                    return ExtractorForCount<TEntity>.Extract(obj);
                case Command.Delete:
                    return ExtractorForDelete<TEntity>.Extract(obj);
                case Command.DeleteAll:
                    return ExtractorForDeleteAll<TEntity>.Extract(obj);
                case Command.InlineInsert:
                    return ExtractorForInlineInsert<TEntity>.Extract(obj);
                case Command.InlineMerge:
                    return ExtractorForInlineMerge<TEntity>.Extract(obj);
                case Command.InlineUpdate:
                    return ExtractorForInlineUpdate<TEntity>.Extract(obj);
                case Command.Insert:
                    return ExtractorForInsert<TEntity>.Extract(obj);
                case Command.Merge:
                    return ExtractorForMerge<TEntity>.Extract(obj);
                case Command.Query:
                    return ExtractorForQuery<TEntity>.Extract(obj);
                case Command.Truncate:
                    return ExtractorForTruncate<TEntity>.Extract(obj);
                case Command.Update:
                    return ExtractorForUpdate<TEntity>.Extract(obj);
                default:
                    return Extractor<TEntity>.Extract(obj);
            }
        }

        /// <summary>
        /// Gets a function from the defined properties.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private static Func<T, IEnumerable<PropertyValue>> GetCompiledFunctionForClassExtractor<T>(IEnumerable<PropertyInfo> properties)
            where T : class
        {
            // Expressions
            var listType = typeof(List<PropertyValue>);
            var method = listType.GetTypeInfo().GetMethod("Add", new[] { typeof(PropertyValue) });
            var param = Expression.Parameter(typeof(T), "obj");
            var constructor = typeof(PropertyValue).GetTypeInfo().GetConstructor(new[]
            {
                    typeof(string),
                    typeof(object),
                    typeof(PropertyInfo)
                });

            // Set the body
            var body = Expression.ListInit(
                Expression.New(listType),
                properties.Select(property =>
                {
                    var name = Expression.Constant(property.GetMappedName());
                    var value = Expression.Convert(Expression.Property(param, property), typeof(object));
                    var propertyValue = Expression.New(constructor,
                        name,
                        value,
                        Expression.Constant(property));
                    return Expression.ElementInit(method, propertyValue);
                }));

            // Set the function value
            return Expression
                .Lambda<Func<T, IEnumerable<PropertyValue>>>(body, param)
                .Compile();
        }

        #region Extractor

        private static class Extractor<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static Extractor()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>());
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForBatchQuery

        private static class ExtractorForBatchQuery<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForBatchQuery()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.BatchQuery));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForBulkInsert

        private static class ExtractorForBulkInsert<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForBulkInsert()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.BulkInsert));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForCount

        private static class ExtractorForCount<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForCount()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.Count));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForDelete

        private static class ExtractorForDelete<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForDelete()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.Delete));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForDeleteAll

        private static class ExtractorForDeleteAll<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForDeleteAll()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.DeleteAll));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForInlineInsert

        private static class ExtractorForInlineInsert<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForInlineInsert()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.InlineInsert));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForInlineMerge

        private static class ExtractorForInlineMerge<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForInlineMerge()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.InlineMerge));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForInlineUpdate

        private static class ExtractorForInlineUpdate<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForInlineUpdate()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.InlineUpdate));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForInsert

        private static class ExtractorForInsert<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForInsert()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.Insert));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForMerge

        private static class ExtractorForMerge<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForMerge()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.Merge));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForQuery

        private static class ExtractorForQuery<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForQuery()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.Query));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForTruncate

        private static class ExtractorForTruncate<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForTruncate()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.Truncate));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForUpdate

        private static class ExtractorForUpdate<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ExtractorForUpdate()
            {
                m_func = GetCompiledFunctionForClassExtractor<T>(PropertyCache.Get<T>(Command.Update));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion
    }
}
