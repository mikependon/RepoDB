using RepoDb.Attributes;

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration used to define an operation on the query expression.
    /// </summary>
    public enum Operation
    {
        /// <summary>
        /// An equal operation.
        /// </summary>
        [Text("=")] Equal = 1968935258,
        /// <summary>
        /// A not-equal operation.
        /// </summary>
        [Text("<>")] NotEqual = 1861928312,
        /// <summary>
        /// A less-than operation.
        /// </summary>
        [Text("<")] LessThan = 1185489222,
        /// <summary>
        /// A greater-than operation.
        /// </summary>
        [Text(">")] GreaterThan = 1904766133,
        /// <summary>
        /// A less-than-or-equal operation.
        /// </summary>
        [Text("<=")] LessThanOrEqual = 1969886037,
        /// <summary>
        /// A greater-than-or-equal operation.
        /// </summary>
        [Text(">=")] GreaterThanOrEqual = 1388583607,
        /// <summary>
        /// A like operation. Defines the (LIKE) keyword in SQL Statement.
        /// </summary>
        [Text("LIKE")] Like = 761684150,
        /// <summary>
        /// A not-like operation. Defines the (NOT LIKE) keyword in SQL Statement.
        /// </summary>
        [Text("NOT LIKE")] NotLike = 9672391,
        /// <summary>
        /// A between operation. Defines the (BETWEEN) keyword in SQL Statement.
        /// </summary>
        [Text("BETWEEN")] Between = 1639933206,
        /// <summary>
        /// A not-between operation. Defines the (NOT BETWEEN) keyword in SQL Statement.
        /// </summary>
        [Text("NOT BETWEEN")] NotBetween = 729234306,
        /// <summary>
        /// An in operation. Defines the (IN) keyword in SQL Statement.
        /// </summary>
        [Text("IN")] In = 1986985380,
        /// <summary>
        /// A not-in operation. Defines the (NOT IN) keyword in SQL Statement.
        /// </summary>
        [Text("NOT IN")] NotIn = 277789918
    }
}