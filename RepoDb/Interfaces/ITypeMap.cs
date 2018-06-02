using System;
using System.Data;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark the class to be a type mapping object that holds the mapping between the .Net CLR Types and database types.
    /// </summary>
    public interface ITypeMap
    {
        /// <summary>
        /// Gets the .Net CLR Type used for mapping.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets the database type used for mapping.
        /// </summary>
        DbType DbType { get; }
    }
}
