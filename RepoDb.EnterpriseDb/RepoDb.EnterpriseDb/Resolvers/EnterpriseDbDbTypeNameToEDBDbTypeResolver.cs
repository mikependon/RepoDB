#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using EnterpriseDB.EDBClient;
using EDBTypes;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the EnterpriseDb Database Types into its <see cref="EDBDbType"/>.
    /// </summary>
    public class EnterpriseDbDbTypeNameToEDBDbTypeResolver : IResolver<string, EDBDbType?>
    {
        /// <summary>
        /// Returns the equivalent <see cref="EDBDbType"/> of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent <see cref="EDBDbType"/>.</returns>
        public virtual EDBDbType? Resolve(string dbTypeName)
        {
            if (string.IsNullOrWhiteSpace(dbTypeName))
            {
                throw new NullReferenceException("The database type name must not be a null or whitespace.");
            }

            // Try parse
            if (Enum.TryParse<EDBDbType>(dbTypeName, true, out var result))
            {
                return result;
            }

            // User-Defined
            if ("USER-DEFINED".Equals(dbTypeName, StringComparison.OrdinalIgnoreCase))
            {
                return EDBDbType.Unknown;
            }

            // Covert to .NET CLR Type
            var clientTypeResolver = new EnterpriseDbDbTypeNameToClientTypeResolver()
                .Resolve(dbTypeName);

            // Try resolve
            return new ClientTypeToEDBDbTypeResolver().Resolve(clientTypeResolver);
        }

        #region Extraction

        //private string Extract()
        //{
        //    using (var connection = new EDBConnection(Database.ConnectionString))
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
