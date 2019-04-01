using RepoDb.Attributes;

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration used to define the ordering of the query field.
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
