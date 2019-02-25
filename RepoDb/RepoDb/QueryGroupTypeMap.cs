using System;

namespace RepoDb
{
    /// <summary>
    /// A class used to hold the <see cref="QueryGroup"/> object type mapping. This class has been introduced
    /// to support the needs of the multi-resultsets query operation.
    /// </summary>
    internal class QueryGroupTypeMap
    {
        /// <summary>
        /// Creates an instance of <see cref="QueryGroupTypeMap"/> class.
        /// </summary>
        /// <param name="queryGroup">The <see cref="QueryGroup"/> object.</param>
        /// <param name="type">The type where the <see cref="QueryGroup"/> object is mapped.</param>
        public QueryGroupTypeMap(QueryGroup queryGroup, Type type)
        {
            QueryGroup = queryGroup;
            MappedType = type;
        }

        /// <summary>
        /// Gets the current <see cref="QueryGroup"/> object.
        /// </summary>
        public QueryGroup QueryGroup { get; }

        /// <summary>
        /// Gets the type where the current <see cref="QueryGroup"/> is mapped.
        /// </summary>
        public Type MappedType { get; }
    }
}
