using Npgsql;
using NpgsqlTypes;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="NpgsqlParameter.NpgsqlDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class NpgsqlParameterDbTypeAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="npgsqlDbType">A target <see cref="NpgsqlTypes.NpgsqlDbType"/> value.</param>
        public NpgsqlParameterDbTypeAttribute(NpgsqlDbType npgsqlDbType)
            : base(typeof(NpgsqlParameter), nameof(NpgsqlParameter.NpgsqlDbType), npgsqlDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="NpgsqlTypes.NpgsqlDbType"/> value.
        /// </summary>
        public NpgsqlDbType NpgsqlDbType => (NpgsqlDbType)Value;
    }
}