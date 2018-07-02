using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A type mapping object that holds the mapping between the .NET CLR Types and database types.
    /// </summary>
    public class TypeMap
    {
        /// <summary>
        /// Creates a a new instance of <i>RepoDb.TypeMap</i> object.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be mapped.</param>
        /// <param name="dbType">The database type to map (typeof <i>System.Data.DbType</i>).</param>
        public TypeMap(Type type, DbType dbType)
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
        public DbType DbType { get; }
    }
}
