using System;
using System.Collections.Generic;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="QueryGroup"/> object.
    /// </summary>
    public static class QueryGroupExtension
    {
        /// <summary>
        /// Convert an instance of query group into an enumerable list of query groups.
        /// </summary>
        /// <param name="queryGroup">The <see cref="QueryGroup"/> object to be converted.</param>
        /// <returns>An enumerable list of query groups.</returns>
        public static IEnumerable<QueryGroup> AsEnumerable(this QueryGroup queryGroup)
        {
            yield return queryGroup;
        }

        /// <summary>
        /// Maps the current <see cref="QueryGroup"/> object to a type.
        /// </summary> 
        /// <typeparam name="TEntity">The target type where the current <see cref="QueryGroup"/> is to be mapped.</typeparam>
        /// <param name="queryGroup">The <see cref="QueryGroup"/> object to be mapped.</param>
        /// <returns>An instance of <see cref="QueryGroupTypeMap"/> object that holds the mapping.</returns>
        internal static QueryGroupTypeMap MapTo<TEntity>(this QueryGroup queryGroup)
            where TEntity : class =>
            new(queryGroup, typeof(TEntity));

        /// <summary>
        /// Maps the current <see cref="QueryGroup"/> object to a type.
        /// </summary>
        /// <param name="queryGroup">The <see cref="QueryGroup"/> object to be mapped.</param>
        /// <param name="type">The target type where the current <see cref="QueryGroup"/> is to be mapped.</param>
        /// <returns>An instance of <see cref="QueryGroupTypeMap"/> object that holds the mapping.</returns>
        internal static QueryGroupTypeMap MapTo(this QueryGroup queryGroup,
            Type type) =>
            new(queryGroup, type);
    }
}
