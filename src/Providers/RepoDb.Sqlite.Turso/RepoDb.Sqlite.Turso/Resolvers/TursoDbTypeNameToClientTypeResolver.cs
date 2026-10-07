#region Copyright Attributions

// Copyright (c) 2026 mamoreau-devolutions and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// Resolves declared SQLite types into CLR types used to bind parameters for Turso.Data.Sqlite.
    /// </summary>
    public class TursoDbTypeNameToClientTypeResolver : IResolver<string, Type>
    {
        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public virtual Type Resolve(string dbTypeName)
        {
            if (dbTypeName == null)
            {
                throw new ArgumentNullException(nameof(dbTypeName), "The DB Type name must not be null.");
            }
            return dbTypeName.ToLowerInvariant() switch
            {
                "bigint" or "int" or "integer" => typeof(long),
                "decimal" or "numeric" => typeof(decimal),
                "blob" => typeof(byte[]),
                "double" or "real" => typeof(double),
                "date" or "datetime" or "time" => typeof(DateTime),
                "boolean" or "char" or "none" or "string" or "text" or "varchar" => typeof(string),
                _ => typeof(object),
            };
        }
    }
}
