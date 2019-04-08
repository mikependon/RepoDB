using RepoDb.Attributes;

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration used to define a conjuction for the query grouping. This enumeration is used at <see cref="QueryGroup"/> object.
    /// </summary>
    public enum Conjunction
    {
        /// <summary>
        /// The (AND) conjunction.
        /// </summary>
        [Text("AND")] And = 446274343,
        /// <summary>
        /// The (OR) conjunction.
        /// </summary>
        [Text("OR")] Or = 1382346125
    }
}
