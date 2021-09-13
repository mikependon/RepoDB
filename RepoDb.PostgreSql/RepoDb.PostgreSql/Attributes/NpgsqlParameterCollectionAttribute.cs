using Npgsql;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="NpgsqlParameter.Collection"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class NpgsqlParameterCollectionAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlParameterCollectionAttribute"/> class.
        /// </summary>
        /// <param name="collection">The collection to which the parameter belongs to.</param>
        public NpgsqlParameterCollectionAttribute(NpgsqlParameterCollection collection)
            : base(typeof(NpgsqlParameter), nameof(NpgsqlParameter.Collection), collection)
        { }

        /// <summary>
        /// Gets the mapped collection to which the parameter belongs to.
        /// </summary>
        public NpgsqlParameterCollection Collection => (NpgsqlParameterCollection)Value;
    }
}