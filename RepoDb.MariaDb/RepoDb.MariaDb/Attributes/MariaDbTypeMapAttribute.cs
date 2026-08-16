using MySql.Data.MySqlClient;
using RepoDb.Attributes.Parameter.MariaDb;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="MySqlDbType"/> value.
    /// </summary>
    [Obsolete("Use the RepoDb.Attributes.Parameter.MariaDb.MySqlDbTypeAttribute instead.")]
    public class MariaDbTypeMapAttribute : MySqlDbTypeAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MariaDbTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="mySqlDbType">A target <see cref="MySqlDbType"/> value.</param>
        public MariaDbTypeMapAttribute(MySqlDbType mySqlDbType)
            : base(mySqlDbType)
        { }
    }
}
