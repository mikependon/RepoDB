using MySqlConnector;
using RepoDb.Attributes.Parameter.MySqlConnector;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="MySqlDbType"/> value.
    /// </summary>
    [Obsolete("Use the RepoDb.Attributes.Parameter.MySqlConnector.MySqlDbTypeAttribute instead.")]
    public class MySqlConnectorTypeMapAttribute : MySqlDbTypeAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlConnectorTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="mySqlDbType">A target <see cref="MySqlDbType"/> value.</param>
        public MySqlConnectorTypeMapAttribute(MySqlDbType mySqlDbType)
            : base(mySqlDbType)
        { }
    }
}