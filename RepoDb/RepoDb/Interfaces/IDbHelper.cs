using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to be a helper class on some database related activity.
    /// </summary>
    public interface IDbHelper
    {
        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table.
        /// </summary>
        /// <param name="connectionString">The connection string to connect to.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        IEnumerable<DbField> GetFields(string connectionString, string tableName);

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        IEnumerable<DbField> GetFields<TDbConnection>(TDbConnection connection, string tableName)
            where TDbConnection : IDbConnection;

        /// <summary>
        /// Gets the type resolver used by this <see cref="IDbHelper"/> instance.
        /// </summary>
        IResolver<string, Type> DbTypeResolver { get; }
    }
}
