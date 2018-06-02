using RepoDb.Attributes;

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration used to define a conjuction for the query grouping. This enumeration is used at <i>RepoDb.QueryGroup</i> object.
    /// </summary>
    public enum Conjunction : short
    {
        /// <summary>
        /// The (AND) conjunction.
        /// </summary>
        [Text("AND")] And = 1,
        /// <summary>
        /// The (OR) conjunction.
        /// </summary>
        [Text("OR")] Or = 2
    }
}
