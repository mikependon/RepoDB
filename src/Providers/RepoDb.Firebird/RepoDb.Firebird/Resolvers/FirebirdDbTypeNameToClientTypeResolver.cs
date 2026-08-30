#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the Firebird Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class FirebirdDbTypeNameToClientTypeResolver : IResolver<string, Type>
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
                throw new NullReferenceException("The DB Type name must not be null.");
            }
            /*
            Id (System.Int64)
            ColumnVarchar (System.String)
            ColumnInt (System.Int32)
            ColumnDecimal2 (System.Decimal)
            ColumnDateTime (System.DateTime)
            ColumnBlob (System.Byte[])
            ColumnBinary (System.Byte[])
            ColumnVarBinary (System.Byte[])
            ColumnDate (System.DateTime)
            ColumnTime (System.TimeSpan)
            ColumnTimeStamp (System.DateTime)
            ColumnBigint (System.Int64)
            ColumnDecimal (System.Decimal)
            ColumnDouble (System.Double)
            ColumnFloat (System.Single)
            ColumnSmallInt (System.Int16)
            ColumnChar (System.String)
            ColumnNChar (System.String)
            ColumnNVarChar (System.String)
            ColumnText (System.String)
            ColumnBoolean (System.Boolean)
            ColumnInt128 (System.Numerics.BigInteger)
             */
            return dbTypeName.ToLowerInvariant() switch
            {
                "smallint" => typeof(short),
                "integer" => typeof(int),
                "bigint" => typeof(long),
                "int128" => typeof(System.Numerics.BigInteger),
                "float" => typeof(float),
                "double precision" => typeof(double),
                "numeric" or "decimal" or "dec16" or "dec34" => typeof(decimal),
                "char" or "varchar" or "blob_text" => typeof(string),
                "blob_binary" or "binary" or "varbinary" => typeof(byte[]),
                "boolean" => typeof(bool),
                "date" => typeof(DateTime),
                "time" => typeof(TimeSpan),
                "timestamp" => typeof(DateTime),
                "time_tz" or "timestamp_tz" => typeof(DateTimeOffset),
                "none" => typeof(object),
                _ => typeof(object),
            };
        }
    }
}
