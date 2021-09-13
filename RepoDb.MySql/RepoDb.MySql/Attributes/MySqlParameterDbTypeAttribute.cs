using MySql.Data.MySqlClient;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="MySqlParameter.MySqlDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class MySqlParameterDbTypeAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="value">A target <see cref="MySql.Data.MySqlClient.MySqlDbType"/> value.</param>
        public MySqlParameterDbTypeAttribute(MySqlDbType value)
            : base(typeof(MySqlParameter), nameof(MySqlParameter.MySqlDbType), value)
        { }

        /// <summary>
        /// Gets a <see cref="MySql.Data.MySqlClient.MySqlDbType"/> that is currently mapped.
        /// </summary>
        public MySqlDbType MySqlDbType => (MySqlDbType)Value;
    }
}