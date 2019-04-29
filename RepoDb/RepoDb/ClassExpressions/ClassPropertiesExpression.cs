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
        /// Gets the properties of the class.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <returns>The properties of the class.</returns>
        public static IEnumerable<ClassProperty> GetProperties<TEntity>()
            where TEntity : class
        {
            return ClassPropertiesExtractor<TEntity>.Extract();
        }

        #region GetProperties<TEntity> Functions

        /// <summary>
        /// Gets a function used to extract the properties of a class.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>The properties of the class.</returns>
        private static Func<IEnumerable<ClassProperty>> GetCompiledFunctionForClassPropertiesExtractor<T>()
            where T : class
        {
            // Variables type (Command.None is preloaded everywhere)
            var properties = DataEntityExtension.GetProperties<T>();

            // Set the body
            var body = Expression.Constant(properties);

            // Set the function value
            return Expression
                .Lambda<Func<IEnumerable<ClassProperty>>>(body)
                .Compile();
        }

        #region ClassPropertiesExtractor<T>

        private static class ClassPropertiesExtractor<T>
            where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static ClassPropertiesExtractor()
            {
                m_func = GetCompiledFunctionForClassPropertiesExtractor<T>();
            }

            public static IEnumerable<ClassProperty> Extract()
            {
                return m_func();
            }
        }

        #endregion

        #endregion
    }
}
