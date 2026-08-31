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
    /// A class that is being used to resolve the SAP HANA Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class SapHanaDbTypeNameToClientTypeResolver : IResolver<string, Type>
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
            HANA SYS.TABLE_COLUMNS.DATA_TYPE_NAME values (non-exhaustive):
            TINYINT (System.Byte)
            SMALLINT (System.Int16)
            INTEGER (System.Int32)
            BIGINT (System.Int64)
            DECIMAL / SMALLDECIMAL (System.Decimal)
            REAL (System.Single)
            DOUBLE (System.Double)
            BOOLEAN (System.Boolean)
            VARCHAR / NVARCHAR / ALPHANUM / SHORTTEXT (System.String)
            CHAR / NCHAR (System.String)
            TEXT / CLOB / NCLOB (System.String)
            DATE (System.DateTime)
            TIME (System.TimeSpan)
            TIMESTAMP / SECONDDATE (System.DateTime)
            BLOB / VARBINARY / BINARY / BINTEXT (System.Byte[])
            */
            return dbTypeName.ToLowerInvariant() switch
            {
                "tinyint" => typeof(byte),
                "smallint" => typeof(short),
                "integer" or "int" => typeof(int),
                "bigint" => typeof(long),
                "decimal" or "smalldecimal" or "numeric" => typeof(decimal),
                "real" or "float" => typeof(float),
                "double" => typeof(double),
                "boolean" => typeof(bool),
                "varchar" or "nvarchar" or "alphanum" or "shorttext" or "char" or "nchar" or "text" or "clob" or "nclob" or "string" => typeof(string),
                "date" => typeof(DateTime),
                "time" => typeof(TimeSpan),
                "timestamp" or "seconddate" => typeof(DateTime),
                "blob" or "varbinary" or "binary" or "bintext" => typeof(byte[]),
                "none" => typeof(object),
                _ => typeof(object),
            };
        }
    }
}
