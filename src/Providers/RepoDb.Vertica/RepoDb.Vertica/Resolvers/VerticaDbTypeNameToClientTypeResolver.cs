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
    /// A class that is being used to resolve the Vertica Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class VerticaDbTypeNameToClientTypeResolver : IResolver<string, Type>
    {
        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type, as reported by <c>v_catalog.columns.data_type</c>
        /// (with its <c>(size)</c>/<c>(precision,scale)</c> suffix already stripped - see <c>VerticaDbHelper</c>).</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public virtual Type Resolve(string dbTypeName)
        {
            if (dbTypeName == null)
            {
                throw new NullReferenceException("The DB Type name must not be null.");
            }
            /*
            Verified directly against VerticaDataReader.GetSchemaTable() - Vertica has no distinct storage
            widths for its integer or floating-point types: SMALLINT/INTEGER/BIGINT/etc. are all synonyms
            for one 8-byte integer (reported as "int"), and FLOAT/DOUBLE PRECISION/REAL are all synonyms
            for one 8-byte float (reported as "float") - so both resolve to their widest CLR type, not
            Int32/Single. TIME is reported back as System.DateTime, not TimeSpan.

            Id (System.Int64)
            ColumnVarchar (System.String)
            ColumnInt (System.Int64)
            ColumnDecimal2 (System.Decimal)
            ColumnDateTime (System.DateTime)
            ColumnBlob (System.Byte[])
            ColumnBinary (System.Byte[])
            ColumnVarBinary (System.Byte[])
            ColumnDate (System.DateTime)
            ColumnTime (System.DateTime)
            ColumnTimeStamp (System.DateTime)
            ColumnBigint (System.Int64)
            ColumnDecimal (System.Decimal)
            ColumnDouble (System.Double)
            ColumnFloat (System.Double)
            ColumnChar (System.String)
            ColumnNChar (System.String)
            ColumnNVarChar (System.String)
            ColumnText (System.String)
            ColumnBit (System.Boolean)
             */
            return dbTypeName.ToLowerInvariant() switch
            {
                "int" or "integer" or "smallint" or "bigint" or "int8" or "tinyint" => typeof(long),
                "float" or "float8" or "double precision" or "real" => typeof(double),
                "numeric" or "decimal" or "number" or "money" => typeof(decimal),
                "char" or "varchar" or "long varchar" => typeof(string),
                "binary" or "varbinary" or "long varbinary" or "bytea" or "raw" => typeof(byte[]),
                "boolean" => typeof(bool),
                "date" => typeof(DateTime),
                "time" => typeof(DateTime),
                "time with timezone" => typeof(DateTimeOffset),
                "timestamp" or "datetime" or "smalldatetime" => typeof(DateTime),
                "timestamp with timezone" => typeof(DateTimeOffset),
                "uuid" => typeof(Guid),
                "none" => typeof(object),
                _ => typeof(object),
            };
        }
    }
}
