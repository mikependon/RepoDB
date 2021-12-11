using Npgsql;
using NpgsqlTypes;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the PostgreSql Database Types into its <see cref="NpgsqlDbType"/>.
    /// </summary>
    public class PostgreSqlDbTypeNameToNpgsqlDbTypeResolver : IResolver<string, NpgsqlDbType?>
    {
        /// <summary>
        /// Returns the equivalent <see cref="NpgsqlDbType"/> of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent <see cref="NpgsqlDbType"/>.</returns>
        public virtual NpgsqlDbType? Resolve(string dbTypeName)
        {
            if (string.IsNullOrWhiteSpace(dbTypeName))
            {
                throw new NullReferenceException("The database type name must not be a null or whitespace.");
            }

            // Try parse
            if (Enum.TryParse<NpgsqlDbType>(dbTypeName, true, out var result))
            {
                return result;
            }

            // User-Defined
            if ("USER-DEFINED".Equals(dbTypeName, StringComparison.OrdinalIgnoreCase))
            {
                return NpgsqlDbType.Unknown;
            }

            // Covert to .NET CLR Type
            var clientTypeResolver = new PostgreSqlDbTypeNameToClientTypeResolver()
                .Resolve(dbTypeName);

            // Try resolve
            return new ClientTypeToNpgsqlDbTypeResolver().Resolve(clientTypeResolver);
        }

        #region Extraction

        //private string Extract()
        //{
        //    using (var connection = new NpgsqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        connection.Open();
        //        using (var command = connection.CreateCommand())
        //        {
        //            using (var reader = connection.ExecuteReader("SELECT * FROM \"CompleteTable\";"))
        //            {
        //                var builder = new StringBuilder();
        //                for (var i = 0; i < reader.FieldCount; i++)
        //                {
        //                    var dataTypeName = reader.GetDataTypeName(i);
        //                    var fieldType = reader.GetFieldType(i);
        //                    builder.AppendLine($"\"{dataTypeName}\" => typeof({fieldType.FullName})");
        //                }
        //                var extracted = builder.ToString();
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}
