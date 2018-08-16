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
        public static IEnumerable<PropertyValue> GetPropertiesAndValues<TEntity>(TEntity obj)
            where TEntity : class
        {
            return ClassPropertyValuesExtractor<TEntity>.Extract(obj);
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
                    return ClassPropertyValuesExtractorForBatchQuery<TEntity>.Extract(obj);
                case Command.BulkInsert:
                    return ClassPropertyValuesExtractorForBulkInsert<TEntity>.Extract(obj);
                case Command.Count:
                    return ClassPropertyValuesExtractorForCount<TEntity>.Extract(obj);
                case Command.Delete:
                    return ClassPropertyValuesExtractorForDelete<TEntity>.Extract(obj);
                case Command.DeleteAll:
                    return ClassPropertyValuesExtractorForDeleteAll<TEntity>.Extract(obj);
                case Command.InlineInsert:
                    return ClassPropertyValuesExtractorForInlineInsert<TEntity>.Extract(obj);
                case Command.InlineMerge:
                    return ClassPropertyValuesExtractorForInlineMerge<TEntity>.Extract(obj);
                case Command.InlineUpdate:
                    return ClassPropertyValuesExtractorForInlineUpdate<TEntity>.Extract(obj);
                case Command.Insert:
                    return ClassPropertyValuesExtractorForInsert<TEntity>.Extract(obj);
                case Command.Merge:
                    return ClassPropertyValuesExtractorForMerge<TEntity>.Extract(obj);
                case Command.Query:
                    return ClassPropertyValuesExtractorForQuery<TEntity>.Extract(obj);
                case Command.Truncate:
                    return ClassPropertyValuesExtractorForTruncate<TEntity>.Extract(obj);
                case Command.Update:
                    return ClassPropertyValuesExtractorForUpdate<TEntity>.Extract(obj);
                default:
                    return ClassPropertyValuesExtractor<TEntity>.Extract(obj);
            }
        }

        /// <summary>
        /// Gets a function that returns the list of property values of the class.
        /// </summary>
        /// <param name="properties">The list of properties.</param>
        /// <returns>The enumerable value of class property values.</returns>
        private static Func<T, IEnumerable<PropertyValue>> GetCompiledFunctionForClassPropertyValuesExtractor<T>(IEnumerable<ClassProperty> properties)
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
                typeof(ClassProperty)
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
                        Expression.Constant(property));
                    return Expression.ElementInit(method, propertyValue);
                }));

            // Set the function value
            return Expression
                .Lambda<Func<T, IEnumerable<PropertyValue>>>(body, param)
                .Compile();
        }

        #region Extractor

        private static class ClassPropertyValuesExtractor<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractor()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>());
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForBatchQuery

        private static class ClassPropertyValuesExtractorForBatchQuery<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForBatchQuery()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.BatchQuery));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForBulkInsert

        private static class ClassPropertyValuesExtractorForBulkInsert<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForBulkInsert()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.BulkInsert));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForCount

        private static class ClassPropertyValuesExtractorForCount<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForCount()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.Count));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForDelete

        private static class ClassPropertyValuesExtractorForDelete<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForDelete()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.Delete));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForDeleteAll

        private static class ClassPropertyValuesExtractorForDeleteAll<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForDeleteAll()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.DeleteAll));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForInlineInsert

        private static class ClassPropertyValuesExtractorForInlineInsert<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForInlineInsert()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.InlineInsert));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForInlineMerge

        private static class ClassPropertyValuesExtractorForInlineMerge<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForInlineMerge()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.InlineMerge));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForInlineUpdate

        private static class ClassPropertyValuesExtractorForInlineUpdate<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForInlineUpdate()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.InlineUpdate));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForInsert

        private static class ClassPropertyValuesExtractorForInsert<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForInsert()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.Insert));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForMerge

        private static class ClassPropertyValuesExtractorForMerge<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForMerge()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.Merge));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForQuery

        private static class ClassPropertyValuesExtractorForQuery<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForQuery()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.Query));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForTruncate

        private static class ClassPropertyValuesExtractorForTruncate<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForTruncate()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.Truncate));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #region ExtractorForUpdate

        private static class ClassPropertyValuesExtractorForUpdate<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractorForUpdate()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>(Command.Update));
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion
    }
}
