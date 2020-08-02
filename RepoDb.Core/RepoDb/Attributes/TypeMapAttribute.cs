using System;
using System.Data;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping between the .NET CLR <see cref="Type"/> and the <see cref="System.Data.DbType"/>.
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