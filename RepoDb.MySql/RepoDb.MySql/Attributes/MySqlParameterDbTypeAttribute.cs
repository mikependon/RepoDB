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
        /// Creates a new instance of <see cref="MySqlParameterDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="value">A target <see cref="MySql.Data.MySqlClient.MySqlDbType"/> value.</param>
        public MySqlParameterDbTypeAttribute(MySqlDbType value)
            : base(typeof(MySqlParameter), nameof(MySqlParameter.MySqlDbType), value)
        { }

        /// <summary>
        /// Gets the mapped <see cref="MySql.Data.MySqlClient.MySqlDbType"/> value of the parameter.
        /// </summary>
        public MySqlDbType MySqlDbType => (MySqlDbType)Value;
    }
}