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
        /// A like operation. Defines the <i>LIKE</i> keyword in SQL Statement.
        /// </summary>
        [Text("LIKE")] Like,
        /// <summary>
        /// A not-like operation. Defines the <i>NOT LIKE</i> keyword in SQL Statement.
        /// </summary>
        [Text("NOT LIKE")] NotLike,
        /// <summary>
        /// A between operation. Defines the <i>BETWEEN</i> keyword in SQL Statement.
        /// </summary>
        [Text("BETWEEN")] Between,
        /// <summary>
        /// A not-between operation. Defines the <i>NOT BETWEEN</i> keyword in SQL Statement.
        /// </summary>
        [Text("NOT BETWEEN")] NotBetween,
        /// <summary>
        /// An in operation. Defines the <i>IN</i> keyword in SQL Statement.
        /// </summary>
        [Text("IN")] In,
        /// <summary>
        /// A non-in operation. Defines the <i>NOT IN</i> keyword in SQL Statement.
        /// </summary>
        [Text("NOT IN")] NotIn,
        /// <summary>
        /// An (AND) equation and operation. This defines that all query expression must be true.
        /// </summary>
        [Text("AND")] All,
        /// <summary>
        /// An (OR) equation and operation. This defines that any query expression must be true.
        /// </summary>
        [Text("OR")] Any
    }
}