using System;
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
        /// Gets the mapped-name of the property object.
        /// </summary>
        /// <param name="property">The target property object.</param>
        /// <returns>The mapped-name of the property.</returns>
        public static string GetPropertyMappedName(PropertyInfo property)
        {
            return PropertyMappedNameExtractor.Extract(property);
        }

        #region ClassMappedNameExtractor

        private static class PropertyMappedNameExtractor
        {
            private static readonly Func<PropertyInfo, string> m_func;

            static PropertyMappedNameExtractor()
            {
                // Parameter
                var property = Expression.Parameter(typeof(PropertyInfo), "property");
                var method = typeof(PropertyMappedNameCache).GetTypeInfo().GetMethod("Get", new[] { typeof(PropertyInfo) });

                // Expressions
                var body = Expression.Call(method, property);

                // Set the function value
                m_func = Expression
                    .Lambda<Func<PropertyInfo, string>>(body, property)
                    .Compile();
            }

            public static string Extract(PropertyInfo property)
            {
                return m_func(property);
            }
        }

        #endregion
    }
}
