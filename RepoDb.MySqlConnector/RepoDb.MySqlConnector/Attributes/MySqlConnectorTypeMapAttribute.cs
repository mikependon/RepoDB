using MySqlConnector;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="MySqlDbType"/> value.
    /// </summary>
    [Obsolete("Use the MySqlConnectorParameterDbTypeAttribute instead.")]
    public class MySqlConnectorTypeMapAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlConnectorTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target <see cref="MySqlDbType"/> value.</param>
        public MySqlConnectorTypeMapAttribute(MySqlDbType dbType)
            : base(typeof(MySqlParameter), nameof(MySqlParameter.MySqlDbType), dbType)
        { }

        /// <summary>
        /// Gets a <see cref="global::MySqlConnector.MySqlDbType"/> that is currently mapped.
        /// </summary>
        public MySqlDbType DbType => (MySqlDbType)Value;
    }
}