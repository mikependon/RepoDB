using MySql.Data.MySqlClient;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="MySqlDbType"/> value.
    /// </summary>
    public class MySqlTypeMapAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target <see cref="MySqlDbType"/> value.</param>
        public MySqlTypeMapAttribute(MySqlDbType dbType)
        {
            DbType = dbType;
            ParameterType = typeof(MySqlParameter);
        }

        /// <summary>
        /// Gets a <see cref="MySqlDbType"/> that is currently mapped.
        /// </summary>
        public MySqlDbType DbType { get; }

        /// <summary>
        /// Gets the represented <see cref="Type"/> of the <see cref="MySqlDbType"/>.
        /// </summary>
        public Type ParameterType { get; }
    }
}