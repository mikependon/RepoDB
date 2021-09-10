using MySql.Data.MySqlClient;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="MySqlDbType"/> value.
    /// </summary>
    public class MySqlTypeMapAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target <see cref="MySqlDbType"/> value.</param>
        public MySqlTypeMapAttribute(MySqlDbType dbType)
            : base(typeof(MySqlParameter), nameof(MySqlParameter.MySqlDbType), dbType)
        { }

        /// <summary>
        /// Gets a <see cref="MySqlDbType"/> that is currently mapped.
        /// </summary>
        public MySqlDbType DbType => (MySqlDbType)Value;
    }
}