using MySql.Data.MySqlClient;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the .NET CLR Types into its equivalent SQL Server <see cref="DbType"/> value.
    /// </summary>
    public class ClientTypeToMySqlDbTypeResolver : IResolver<Type, MySqlDbType?>
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
         */

        /// <summary>
        /// Returns the equivalent <see cref="DbType"/> of the target .NET CLR Type.
        /// </summary>
        /// <param name="type">The .NET CLR Type.</param>
        /// <returns>The equivalent <see cref="DbType"/> Type.</returns>
        public MySqlDbType? Resolve(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The type must not be null.");
            }

            type = type?.GetUnderlyingType();

            if (type == typeof(long))
            {
                return MySqlDbType.Int64;
            }

            return null;
        }
    }
}
