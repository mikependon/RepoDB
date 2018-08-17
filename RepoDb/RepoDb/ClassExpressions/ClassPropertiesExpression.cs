using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RepoDb
{
    /// <summary>
    /// A class used for manipulating class objects via expressions.
    /// </summary>
    public static partial class ClassExpression
    {
        /// <summary>
        /// Gets the properties of the class based on the target command.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>The properties of the class based on the target command.</returns>
        public static IEnumerable<ClassProperty> GetProperties<TEntity>(Command command)
            where TEntity : class
        {
            switch (command)
            {
                case Command.BatchQuery:
                    return ClassPropertiesExtractorForBatchQuery<TEntity>.Extract();
                case Command.BulkInsert:
                    return ClassPropertiesExtractorForBulkInsert<TEntity>.Extract();
                case Command.Count:
                    return ClassPropertiesExtractorForCount<TEntity>.Extract();
                case Command.Delete:
                    return ClassPropertiesExtractorForDelete<TEntity>.Extract();
                case Command.DeleteAll:
                    return ClassPropertiesExtractorForDeleteAll<TEntity>.Extract();
                case Command.InlineInsert:
                    return ClassPropertiesExtractorForInlineInsert<TEntity>.Extract();
                case Command.InlineMerge:
                    return ClassPropertiesExtractorForInlineMerge<TEntity>.Extract();
                case Command.InlineUpdate:
                    return ClassPropertiesExtractorForInlineUpdate<TEntity>.Extract();
                case Command.Insert:
                    return ClassPropertiesExtractorForInsert<TEntity>.Extract();
                case Command.Merge:
                    return ClassPropertiesExtractorForMerge<TEntity>.Extract();
                case Command.Query:
                    return ClassPropertiesExtractorForQuery<TEntity>.Extract();
                case Command.Truncate:
                    return ClassPropertiesExtractorForTruncate<TEntity>.Extract();
                case Command.Update:
                    return ClassPropertiesExtractorForUpdate<TEntity>.Extract();
                default:
                    return ClassPropertiesExtractor<TEntity>.Extract();
            }
        }

        /// <summary>
        /// Gets a function used to extract the properties of a class.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>The properties of the class.</returns>
        private static Func<IEnumerable<ClassProperty>> GetCompiledFunctionForClassPropertiesExtractor<T>(Command command)
            where T : class
        {
            // Variables type (Command.None is preloaded everywhere)
            var properties = DataEntityExtension.GetPropertiesFor<T>(command);

            // Set the body
            var body = Expression.Constant(properties);

            // Set the function value
            return Expression
                .Lambda<Func<IEnumerable<ClassProperty>>>(body)
                .Compile();
        }

        #region ClassPropertiesExtractor

        private static class ClassPropertiesExtractor<T>
            where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractor()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.None);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForBatchQuery

        private static class ClassPropertiesExtractorForBatchQuery<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForBatchQuery()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.BatchQuery);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForBulkInsert

        private static class ClassPropertiesExtractorForBulkInsert<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForBulkInsert()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.BulkInsert);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForCount

        private static class ClassPropertiesExtractorForCount<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForCount()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.Count);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForDelete

        private static class ClassPropertiesExtractorForDelete<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForDelete()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.Delete);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForDeleteAll

        private static class ClassPropertiesExtractorForDeleteAll<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForDeleteAll()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.DeleteAll);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForInlineInsert

        private static class ClassPropertiesExtractorForInlineInsert<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForInlineInsert()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.InlineInsert);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForInlineMerge

        private static class ClassPropertiesExtractorForInlineMerge<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForInlineMerge()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.InlineMerge);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForInlineUpdate

        private static class ClassPropertiesExtractorForInlineUpdate<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForInlineUpdate()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.InlineUpdate);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForInsert

        private static class ClassPropertiesExtractorForInsert<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForInsert()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.Insert);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForMerge

        private static class ClassPropertiesExtractorForMerge<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForMerge()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.Merge);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForQuery

        private static class ClassPropertiesExtractorForQuery<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForQuery()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.Query);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForTruncate

        private static class ClassPropertiesExtractorForTruncate<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForTruncate()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.Truncate);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #region ExtractorForUpdate

        private static class ClassPropertiesExtractorForUpdate<T> where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractorForUpdate()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>(Command.Update);
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion
    }
}
