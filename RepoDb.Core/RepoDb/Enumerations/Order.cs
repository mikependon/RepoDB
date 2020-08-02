using RepoDb.Attributes;

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define an ordering for the query.
    /// </summary>
    public enum Order
    {
        /// <summary>
        /// The ascending order.
        /// </summary>
        [Text("ASC")] Ascending = 720208773,
        /// <summary>
        /// The descending order.
        /// </summary>
        [Text("DESC")] Descending = 1249030520
    }
}
