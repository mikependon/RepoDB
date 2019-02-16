using RepoDb.Attributes;

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration used to define an operation on the query expression.
    /// </summary>
    public enum Operation : short
    {
        /// <summary>
        /// An equal operation.
        /// </summary>
        [Text("=")] Equal,
        /// <summary>
        /// A not-equal operation.
        /// </summary>
        [Text("<>")] NotEqual,
        /// <summary>
        /// A less-than operation.
        /// </summary>
        [Text("<")] LessThan,
        /// <summary>
        /// A greater-than operation.
        /// </summary>
        [Text(">")] GreaterThan,
        /// <summary>
        /// A less-than-or-equal operation.
        /// </summary>
        [Text("<=")] LessThanOrEqual,
        /// <summary>
        /// A greater-than-or-equal operation.
        /// </summary>
        [Text(">=")] GreaterThanOrEqual,
        /// <summary>
        /// A like operation. Defines the (LIKE) keyword in SQL Statement.
        /// </summary>
        [Text("LIKE")] Like,
        /// <summary>
        /// A not-like operation. Defines the (NOT LIKE) keyword in SQL Statement.
        /// </summary>
        [Text("NOT LIKE")] NotLike,
        /// <summary>
        /// A between operation. Defines the (BETWEEN) keyword in SQL Statement.
        /// </summary>
        [Text("BETWEEN")] Between,
        /// <summary>
        /// A not-between operation. Defines the (NOT BETWEEN) keyword in SQL Statement.
        /// </summary>
        [Text("NOT BETWEEN")] NotBetween,
        /// <summary>
        /// An in operation. Defines the (IN) keyword in SQL Statement.
        /// </summary>
        [Text("IN")] In,
        /// <summary>
        /// A non-in operation. Defines the (NOT IN) keyword in SQL Statement.
        /// </summary>
        [Text("NOT IN")] NotIn
    }
}