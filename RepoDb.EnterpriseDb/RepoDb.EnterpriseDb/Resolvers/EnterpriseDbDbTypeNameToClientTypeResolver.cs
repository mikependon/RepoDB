#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using EnterpriseDB.EDBClient;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the EnterpriseDb Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class EnterpriseDbDbTypeNameToClientTypeResolver : IResolver<string, Type>
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
            "bigint" => typeof(Int64),
            "bigint[]" => typeof(Array),
            "bit varying" => typeof(System.Collections.BitArray),
            "bit varying[]" => typeof(Array),
            "bit(1)" => typeof(Boolean),
            "bit(1)[]" => typeof(Array),
            "boolean" => typeof(Boolean),
            "boolean[]" => typeof(Array),
            "box" => typeof(EDBTypes.EDBBox),
            "box[]" => typeof(Array),
            "bytea" => typeof(Byte[]),
            "bytea[]" => typeof(Array),
            "char" => typeof(Char),
            "char[]" => typeof(Array),
            "character varying" => typeof(String),
            "character varying[]" => typeof(Array),
            "character(1)" => typeof(String),
            "cid" => typeof(UInt32),
            "cid[]" => typeof(Array),
            "cidr" => typeof(ValueTuple<System.Net.IPAddress, Int32>),
            "circle" => typeof(EDBTypes.EDBCircle),
            "circle[]" => typeof(Array),
            "date" => typeof(DateTime),
            "date[]" => typeof(Array),
            "daterange" => typeof(EDBTypes.EDBRange<DateTime>),
            "daterange[]" => typeof(Array),
            "double precision" => typeof(Double),
            "double precision[]" => typeof(Array),
            "inet" => typeof(System.Net.IPAddress),
            "inet[]" => typeof(Array),
            "int2vector" => typeof(Array),
            "int2vector[]" => typeof(Array),
            "int4range" => typeof(EDBTypes.EDBRange<Int32>),
            "int4range[]" => typeof(Array),
            "int8range" => typeof(EDBTypes.EDBRange<Int64>),
            "int8range[]" => typeof(Array),
            "integer" => typeof(Int32),
            "integer[]" => typeof(Array),
            "interval" => typeof(TimeSpan),
            "interval[]" => typeof(Array),
            "json" => typeof(String),
            "json[]" => typeof(Array),
            "jsonb" => typeof(String),
            "jsonb[]" => typeof(Array),
            "jsonpath" => typeof(String),
            "jsonpath[]" => typeof(Array),
            "line" => typeof(EDBTypes.EDBLine),
            "line[]" => typeof(Array),
            "lseg" => typeof(EDBTypes.EDBLSeg),
            "lseg[]" => typeof(Array),
            "macaddr" => typeof(System.Net.NetworkInformation.PhysicalAddress),
            "macaddr[]" => typeof(Array),
            "macaddr8" => typeof(System.Net.NetworkInformation.PhysicalAddress),
            "macaddr8[]" => typeof(Array),
            "money" => typeof(Decimal),
            "money[]" => typeof(Array),
            "name" => typeof(String),
            "name[]" => typeof(Array),
            "numeric" => typeof(Decimal),
            "numeric[]" => typeof(Array),
            "numrange" => typeof(EDBTypes.EDBRange<Decimal>),
            "numrange[]" => typeof(Array),
            "oid" => typeof(UInt32),
            "oid[]" => typeof(Array),
            "oidvector" => typeof(Array),
            "oidvector[]" => typeof(Array),
            "path" => typeof(EDBTypes.EDBPath),
            "path[]" => typeof(Array),
            "pg_dependencies" => typeof(String),
            "pg_lsn" => typeof(EDBTypes.EDBLogSequenceNumber),
            "pg_lsn[]" => typeof(Array),
            "pg_mcv_list" => typeof(String),
            "pg_ndistinct" => typeof(String),
            "pg_node_tree" => typeof(String),
            "point" => typeof(EDBTypes.EDBPoint),
            "point[]" => typeof(Array),
            "polygon" => typeof(EDBTypes.EDBPolygon),
            "polygon[]" => typeof(Array),
            "real" => typeof(Single),
            "real[]" => typeof(Array),
            "refcursor" => typeof(String),
            "refcursor[]" => typeof(Array),
            "regclass" => typeof(String),
            "regclass[]" => typeof(String),
            "regconfig" => typeof(UInt32),
            "regconfig[]" => typeof(Array),
            "regdictionary" => typeof(String),
            "regdictionary[]" => typeof(String),
            "regnamespace" => typeof(String),
            "regnamespace[]" => typeof(String),
            "regoper" => typeof(String),
            "regoper[]" => typeof(String),
            "regoperator" => typeof(String),
            "regoperator[]" => typeof(String),
            "regproc" => typeof(String),
            "regproc[]" => typeof(String),
            "regprocedure" => typeof(String),
            "regprocedure[]" => typeof(String),
            "regrole" => typeof(String),
            "regrole[]" => typeof(String),
            "regtype" => typeof(UInt32),
            "regtype[]" => typeof(Array),
            "smallint" => typeof(Int16),
            "smallint[]" => typeof(Array),
            "text" => typeof(String),
            "text[]" => typeof(Array),
            "tid" => typeof(EDBTypes.EDBTid),
            "tid[]" => typeof(Array),
            "time with time zone" => typeof(DateTimeOffset),
            "time with time zone[]" => typeof(Array),
            "time without time zone" => typeof(TimeSpan),
            "time without time zone[]" => typeof(Array),
            "timestamp with time zone" => typeof(DateTime),
            "timestamp with time zone[]" => typeof(Array),
            "timestamp without time zone" => typeof(DateTime),
            "timestamp without time zone[]" => typeof(Array),
            "tsquery" => typeof(EDBTypes.EDBTsQuery),
            "tsquery[]" => typeof(Array),
            "tsrange" => typeof(EDBTypes.EDBRange<DateTime>),
            "tsrange[]" => typeof(Array),
            "tstzrange" => typeof(EDBTypes.EDBRange<DateTime>),
            "tstzrange[]" => typeof(Array),
            "tsvector" => typeof(EDBTypes.EDBTsVector),
            "tsvector[]" => typeof(Array),
            "txid_snapshot" => typeof(String),
            "txid_snapshot[]" => typeof(String),
            "uuid" => typeof(Guid),
            "uuid[]" => typeof(Array),
            "xid" => typeof(UInt32),
            "xid[]" => typeof(Array),
            "xml" => typeof(String),
            "xml[]" => typeof(Array),
            _ => typeof(object)
            */

            return dbTypeName.ToLowerInvariant() switch
            {
                "bigint" => typeof(Int64),
                "char" or "\"char\"" => typeof(Char),
                "array" => typeof(Array),
                "character" or "character varying" or "json" or "jsonb" or "jsonpath" or "name" or "pg_dependencies" or "pg_lsn" or "pg_mcv_list" or "pg_ndistinct" or "pg_node_tree" or "refcursor" or "regclass" or "regdictionary" or "regnamespace" or "regoper" or "regoperator" or "regproc" or "regprocedure" or "regrole" or "text" or "txid_snapshot" or "xml" => typeof(String),
                "bit" or "boolean" => typeof(Boolean),
                "bit varying" => typeof(System.Collections.BitArray),
                "box" => typeof(EDBTypes.EDBBox),
                "bytea" => typeof(Byte[]),
                "cid" or "oid" or "regconfig" or "regtype" or "xid" => typeof(UInt32),
                "circle" => typeof(EDBTypes.EDBCircle),
                "date"
#if NET6_0_OR_GREATER
                    => typeof(DateOnly),
#else
                    or 
#endif
                "timestamp without time zone" or "timestamp" => typeof(DateTime),
                "timestamp with time zone" or "timestamptz" => typeof(DateTimeOffset),
                "double precision" => typeof(Double),
                "inet" => typeof(System.Net.IPAddress),
                "integer" => typeof(Int32),
                "time without time zone" or "time"
#if NET6_0_OR_GREATER
                    => typeof(TimeOnly),
#else
                    or 
#endif
                "interval" => typeof(TimeSpan),
                "line" => typeof(EDBTypes.EDBLine),
                "lseg" => typeof(EDBTypes.EDBLSeg),
                "macaddr" or "macaddr8" => typeof(System.Net.NetworkInformation.PhysicalAddress),
                "money" or "numeric" => typeof(Decimal),
                "path" => typeof(EDBTypes.EDBPath),
                "point" => typeof(EDBTypes.EDBPoint),
                "polygon" => typeof(EDBTypes.EDBPolygon),
                "real" => typeof(Single),
                "smallint" => typeof(Int16),
                "tid" => typeof(EDBTypes.EDBTid),
                "timetz" or "time with time zone" => typeof(DateTimeOffset),
                "tsquery" => typeof(EDBTypes.EDBTsQuery),
                "tsvector" => typeof(EDBTypes.EDBTsVector),
                "uuid" => typeof(Guid),
                _ => typeof(object),
            };
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
