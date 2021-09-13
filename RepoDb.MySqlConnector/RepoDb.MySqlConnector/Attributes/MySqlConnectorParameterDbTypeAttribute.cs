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
        /// Creates a new instance of <see cref="MySqlConnectorParameterDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="mySqlDbType">A target <see cref="MySqlConnector.MySqlDbType"/> value.</param>
        public MySqlConnectorParameterDbTypeAttribute(MySqlDbType mySqlDbType)
            : base(typeof(MySqlParameter), nameof(MySqlParameter.MySqlDbType), mySqlDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="MySqlConnector.MySqlDbType"/> value of the parameter.
        /// </summary>
        public MySqlDbType MySqlDbType => (MySqlDbType)Value;
    }
}