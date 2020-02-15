using System;
using System.Data;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="System.Data.DbType"/>.
    /// </summary>
    public class TypeMapAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="TypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target <see cref="System.Data.DbType"/> value.</param>
        public TypeMapAttribute(DbType dbType)
        {
            DbType = dbType;
        }

        /// <summary>
        /// Gets a <see cref="System.Data.DbType"/> value that is currently mapped.
        /// </summary>
        public DbType DbType { get; }
    }
}