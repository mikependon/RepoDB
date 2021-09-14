using Npgsql;

namespace RepoDb.Attributes.Parameter.Npgsql
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="NpgsqlParameter.Collection"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class CollectionAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="CollectionAttribute"/> class.
        /// </summary>
        /// <param name="collection">The collection to which the parameter belongs to.</param>
        public CollectionAttribute(NpgsqlParameterCollection collection)
            : base(typeof(NpgsqlParameter), nameof(NpgsqlParameter.Collection), collection)
        { }

        /// <summary>
        /// Gets the mapped collection to which the parameter belongs to.
        /// </summary>
        public NpgsqlParameterCollection Collection => (NpgsqlParameterCollection)Value;
    }
}