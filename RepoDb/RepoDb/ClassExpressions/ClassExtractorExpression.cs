using RepoDb.Enumerations;
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
            return ClassExtractor<TEntity>.Extract(obj);
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
                    return ClassExtractorForBatchQuery<TEntity>.Extract(obj);
                case Command.BulkInsert:
                    return ClassExtractorForBulkInsert<TEntity>.Extract(obj);
                case Command.Count:
                    return ClassExtractorForCount<TEntity>.Extract(obj);
                case Command.Delete:
                    return ClassExtractorForDelete<TEntity>.Extract(obj);
                case Command.DeleteAll:
                    return ClassExtractorForDeleteAll<TEntity>.Extract(obj);
                case Command.InlineInsert:
                    return ClassExtractorForInlineInsert<TEntity>.Extract(obj);
                case Command.InlineMerge:
                    return ClassExtractorForInlineMerge<TEntity>.Extract(obj);
                case Command.InlineUpdate:
                    return ClassExtractorForInlineUpdate<TEntity>.Extract(obj);
                case Command.Insert:
                    return ClassExtractorForInsert<TEntity>.Extract(obj);
                case Command.Merge:
                    return ClassExtractorForMerge<TEntity>.Extract(obj);
                case Command.Query:
                    return ClassExtractorForQuery<TEntity>.Extract(obj);
                case Command.Truncate:
                    return ClassExtractorForTruncate<TEntity>.Extract(obj);
                case Command.Update:
                    return ClassExtractorForUpdate<TEntity>.Extract(obj);
                default:
                    return ClassExtractor<TEntity>.Extract(obj);
            }
        }

        /// <summary>
        /// Gets a function from the defined properties.
        /// </summary>
        /// <param name="properties">The list of properties.</param>
        /// <returns>The enumerable value of class property values.</returns>
        private static Func<T, IEnumerable<PropertyValue>> GetCompiledFunctionForClassExtractor<T>(IEnumerable<ClassProperty> properties)
            where T : class
        {
            // Expressions
            var listType = typeof(List<PropertyValue>);
            var method = listType.GetMethod("Add", new[] { typeof(PropertyValue) });
            var param = Expression.Parameter(typeof(T), "obj");
            var constructor = typeof(PropertyValue).GetConstructor(new[]
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
                    var value = Expression.Convert(Expression.Property(param, property.PropertyInfo), typeof(object));
                    var propertyValue = Expression.New(constructor,
                        name,
                        value,
                        Expression.Constant(property.PropertyInfo));
                    return Expression.ElementInit(method, propertyValue);
                }));

            // Set the function value
            return Expression
                .Lambda<Func<T, IEnumerable<PropertyValue>>>(body, param)
                .Compile();
        }

        #region Extractor

        private static class ClassExtractor<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractor()
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

        private static class ClassExtractorForBatchQuery<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForBatchQuery()
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

        private static class ClassExtractorForBulkInsert<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForBulkInsert()
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

        private static class ClassExtractorForCount<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForCount()
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

        private static class ClassExtractorForDelete<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForDelete()
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

        private static class ClassExtractorForDeleteAll<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForDeleteAll()
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

        private static class ClassExtractorForInlineInsert<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForInlineInsert()
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

        private static class ClassExtractorForInlineMerge<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForInlineMerge()
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

        private static class ClassExtractorForInlineUpdate<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForInlineUpdate()
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

        private static class ClassExtractorForInsert<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForInsert()
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

        private static class ClassExtractorForMerge<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForMerge()
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

        private static class ClassExtractorForQuery<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForQuery()
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

        private static class ClassExtractorForTruncate<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForTruncate()
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

        private static class ClassExtractorForUpdate<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassExtractorForUpdate()
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
