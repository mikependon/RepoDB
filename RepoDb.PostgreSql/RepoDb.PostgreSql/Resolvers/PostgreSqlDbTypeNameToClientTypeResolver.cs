using System;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the PostgreSql Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class PostgreSqlDbTypeNameToClientTypeResolver : IResolver<string, Type>
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
            "box" => typeof(NpgsqlTypes.NpgsqlBox),
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
            "circle" => typeof(NpgsqlTypes.NpgsqlCircle),
            "circle[]" => typeof(Array),
            "date" => typeof(DateTime),
            "date[]" => typeof(Array),
            "daterange" => typeof(NpgsqlTypes.NpgsqlRange<DateTime>),
            "daterange[]" => typeof(Array),
            "double precision" => typeof(Double),
            "double precision[]" => typeof(Array),
            "inet" => typeof(System.Net.IPAddress),
            "inet[]" => typeof(Array),
            "int2vector" => typeof(Array),
            "int2vector[]" => typeof(Array),
            "int4range" => typeof(NpgsqlTypes.NpgsqlRange<Int32>),
            "int4range[]" => typeof(Array),
            "int8range" => typeof(NpgsqlTypes.NpgsqlRange<Int64>),
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
            "line" => typeof(NpgsqlTypes.NpgsqlLine),
            "line[]" => typeof(Array),
            "lseg" => typeof(NpgsqlTypes.NpgsqlLSeg),
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
            "numrange" => typeof(NpgsqlTypes.NpgsqlRange<Decimal>),
            "numrange[]" => typeof(Array),
            "oid" => typeof(UInt32),
            "oid[]" => typeof(Array),
            "oidvector" => typeof(Array),
            "oidvector[]" => typeof(Array),
            "path" => typeof(NpgsqlTypes.NpgsqlPath),
            "path[]" => typeof(Array),
            "pg_dependencies" => typeof(String),
            "pg_lsn" => typeof(NpgsqlTypes.NpgsqlLogSequenceNumber),
            "pg_lsn[]" => typeof(Array),
            "pg_mcv_list" => typeof(String),
            "pg_ndistinct" => typeof(String),
            "pg_node_tree" => typeof(String),
            "point" => typeof(NpgsqlTypes.NpgsqlPoint),
            "point[]" => typeof(Array),
            "polygon" => typeof(NpgsqlTypes.NpgsqlPolygon),
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
            "tid" => typeof(NpgsqlTypes.NpgsqlTid),
            "tid[]" => typeof(Array),
            "time with time zone" => typeof(DateTimeOffset),
            "time with time zone[]" => typeof(Array),
            "time without time zone" => typeof(TimeSpan),
            "time without time zone[]" => typeof(Array),
            "timestamp with time zone" => typeof(DateTime),
            "timestamp with time zone[]" => typeof(Array),
            "timestamp without time zone" => typeof(DateTime),
            "timestamp without time zone[]" => typeof(Array),
            "tsquery" => typeof(NpgsqlTypes.NpgsqlTsQuery),
            "tsquery[]" => typeof(Array),
            "tsrange" => typeof(NpgsqlTypes.NpgsqlRange<DateTime>),
            "tsrange[]" => typeof(Array),
            "tstzrange" => typeof(NpgsqlTypes.NpgsqlRange<DateTime>),
            "tstzrange[]" => typeof(Array),
            "tsvector" => typeof(NpgsqlTypes.NpgsqlTsVector),
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
                "box" => typeof(NpgsqlTypes.NpgsqlBox),
                "bytea" => typeof(Byte[]),
                "cid" or "oid" or "regconfig" or "regtype" or "xid" => typeof(UInt32),
                "circle" => typeof(NpgsqlTypes.NpgsqlCircle),
                "date"
#if NET
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
#if NET
                    => typeof(TimeOnly),
#else
                    or
#endif
                "interval" => typeof(TimeSpan),
                "line" => typeof(NpgsqlTypes.NpgsqlLine),
                "lseg" => typeof(NpgsqlTypes.NpgsqlLSeg),
                "macaddr" or "macaddr8" => typeof(System.Net.NetworkInformation.PhysicalAddress),
                "money" or "numeric" => typeof(Decimal),
                "path" => typeof(NpgsqlTypes.NpgsqlPath),
                "point" => typeof(NpgsqlTypes.NpgsqlPoint),
                "polygon" => typeof(NpgsqlTypes.NpgsqlPolygon),
                "real" => typeof(Single),
                "smallint" => typeof(Int16),
                "tid" => typeof(NpgsqlTypes.NpgsqlTid),
                "timetz" or "time with time zone" => typeof(DateTimeOffset),
                "tsquery" => typeof(NpgsqlTypes.NpgsqlTsQuery),
                "tsvector" => typeof(NpgsqlTypes.NpgsqlTsVector),
                "uuid" => typeof(Guid),
                _ => typeof(object),
            };
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
