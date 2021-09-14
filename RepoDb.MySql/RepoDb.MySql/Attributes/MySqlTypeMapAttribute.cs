using MySql.Data.MySqlClient;
using RepoDb.Attributes.Parameter.MySql;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="MySqlDbType"/> value.
    /// </summary>
    [Obsolete("Use the RepoDb.Attributes.Parameter.MySqlDbTypeAttribute instead.")]
    public class MySqlTypeMapAttribute : MySqlDbTypeAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="mySqlDbType">A target <see cref="MySqlDbType"/> value.</param>
        public MySqlTypeMapAttribute(MySqlDbType mySqlDbType)
            : base(mySqlDbType)
        { }
    }
}