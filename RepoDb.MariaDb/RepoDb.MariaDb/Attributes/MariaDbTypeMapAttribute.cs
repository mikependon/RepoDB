using RepoDb.Attributes.Parameter.MariaDb;
using RepoDb.Connector.MariaDb;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="MariaDbType"/> value.
    /// </summary>
    [Obsolete("Use the RepoDb.Attributes.Parameter.MariaDb.MariaDbTypeAttribute instead.")]
    public class MariaDbTypeMapAttribute : MariaDbTypeAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MariaDbTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="mariaDbType">A target <see cref="MariaDbType"/> value.</param>
        public MariaDbTypeMapAttribute(MariaDbType mariaDbType)
            : base(mariaDbType)
        { }
    }
}
