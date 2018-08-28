using System;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A type mapping object that holds the mapping between the .NET CLR Types and database types.
    /// </summary>
    public class TypeMapItem
    {
        /// <summary>
        /// Creates a a new instance of <see cref="TypeMapItem"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be mapped.</param>
        /// <param name="dbType">The database type to map (typeof <see cref="DbType"/>).</param>
        public TypeMapItem(Type type, DbType dbType)
        {
            Type = type;
            DbType = dbType;
        }

        /// <summary>
        /// Gets the .NET CLR Type used for mapping.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the database type used for mapping.
        /// </summary>
        public DbType DbType { get; private set; }

        /// <summary>
        /// Internally sets the value of the <see cref="DbType"/> property.
        /// </summary>
        /// <param name="dbType">The value of the <see cref="DbType"/>.</param>
        internal void SetDbType(DbType dbType)
        {
            DbType = dbType;
        }
    }
}
