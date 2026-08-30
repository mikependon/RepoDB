#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.Db2;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the raw Db2 database type name (e.g. as returned by
    /// <c>SYSCAT.COLUMNS.TYPENAME</c>) into its equivalent <see cref="DB2Type"/>.
    /// </summary>
    public class Db2DbTypeNameToDb2TypeResolver : IResolver<string, DB2Type>
    {
        /*
         * Taken:
         * https://www.ibm.com/docs/en/db2/11.5?topic=catalog-syscatcolumns-view (TYPENAME domain)
         * https://www.ibm.com/docs/en/db2/11.5?topic=elements-data-types (SQL data types)
         */

        /// <summary>
        /// Returns the equivalent <see cref="DB2Type"/> of the database type name.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type (e.g. as returned by SYSCAT.COLUMNS.TYPENAME).</param>
        /// <returns>The equivalent <see cref="DB2Type"/>.</returns>
        public virtual DB2Type Resolve(string dbTypeName)
        {
            if (dbTypeName == null)
            {
                throw new NullReferenceException("The DB Type name must not be null.");
            }

            var name = dbTypeName.ToLowerInvariant().Trim();

            if (name.StartsWith("timestamp"))
            {
                return name.Contains("with time zone") ? DB2Type.TimeStampWithTimeZone : DB2Type.Timestamp;
            }

            return name switch
            {
                "smallint" => DB2Type.SmallInt,
                "integer" or "int" => DB2Type.Integer,
                "bigint" => DB2Type.BigInt,
                "decimal" or "numeric" or "dec" => DB2Type.Decimal,
                "decfloat" => DB2Type.DecimalFloat,
                "real" => DB2Type.Real,
                "double" or "double precision" or "float" => DB2Type.Double,
                "char" or "character" => DB2Type.Char,
                "varchar" => DB2Type.VarChar,
                "long varchar" => DB2Type.LongVarChar,
                "graphic" => DB2Type.Graphic,
                "vargraphic" => DB2Type.VarGraphic,
                "long vargraphic" => DB2Type.LongVarGraphic,
                "clob" => DB2Type.Clob,
                "dbclob" => DB2Type.DbClob,
                "blob" => DB2Type.Blob,
                "binary" => DB2Type.Binary,
                "varbinary" => DB2Type.VarBinary,
                "xml" => DB2Type.Xml,
                "rowid" => DB2Type.RowId,
                "date" => DB2Type.Date,
                "time" => DB2Type.Time,
                "boolean" => DB2Type.Boolean,
                _ => DB2Type.VarChar,
            };
        }
    }
}
