using MySql.Data.MySqlClient;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="MySqlDbType"/> value.
    /// </summary>
    [Obsolete("Use the MySqlParameterDbTypeAttribute instead.")]
    public class MySqlTypeMapAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="mySqlDbType">A target <see cref="MySqlDbType"/> value.</param>
        public MySqlTypeMapAttribute(MySqlDbType mySqlDbType)
            : base(typeof(MySqlParameter), nameof(MySqlParameter.MySqlDbType), mySqlDbType)
        { }

        /// <summary>
        /// Gets a <see cref="MySqlDbType"/> that is currently mapped.
        /// </summary>
        public MySqlDbType MySqlDbType => (MySqlDbType)Value;
    }
}