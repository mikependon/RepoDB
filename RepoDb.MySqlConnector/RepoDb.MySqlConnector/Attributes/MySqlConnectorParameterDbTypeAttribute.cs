using MySqlConnector;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="MySqlParameter.MySqlDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class MySqlConnectorParameterDbTypeAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlConnectorTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="mySqlDbType">A target <see cref="MySqlConnector.MySqlDbType"/> value.</param>
        public MySqlConnectorParameterDbTypeAttribute(MySqlDbType mySqlDbType)
            : base(typeof(MySqlParameter), nameof(MySqlParameter.MySqlDbType), mySqlDbType)
        { }

        /// <summary>
        /// Gets a <see cref="MySqlConnector.MySqlDbType"/> that is currently mapped.
        /// </summary>
        public MySqlDbType MySqlDbType => (MySqlDbType)Value;
    }
}