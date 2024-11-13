using System;
using NpgsqlTypes;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the .NET CLR Type into its equivalent <see cref="NpgsqlDbType"/>.
    /// </summary>
    public class ClientTypeToNpgsqlDbTypeResolver : IResolver<Type, NpgsqlDbType?>
    {
        /// <summary>
        /// Returns the equivalent <see cref="NpgsqlDbType"/> based from the .NET CLR Type.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <returns>The equivalent <see cref="NpgsqlDbType"/>.</returns>
        public virtual NpgsqlDbType? Resolve(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The type must not be null.");
            }

            if (type == typeof(NpgsqlBox))
            {
                return NpgsqlDbType.Box;
            }
            else if (type == typeof(NpgsqlCircle))
            {
                return NpgsqlDbType.Circle;
            }
            else if (type == typeof(NpgsqlLine))
            {
                return NpgsqlDbType.Line;
            }
            else if (type == typeof(NpgsqlLogSequenceNumber))
            {
                return NpgsqlDbType.PgLsn;
            }
            else if (type == typeof(NpgsqlLSeg))
            {
                return NpgsqlDbType.LSeg;
            }
            else if (type == typeof(NpgsqlPath))
            {
                return NpgsqlDbType.Path;
            }
            else if (type == typeof(NpgsqlPoint))
            {
                return NpgsqlDbType.Point;
            }
            else if (type == typeof(NpgsqlPolygon))
            {
                return NpgsqlDbType.Polygon;
            }
            else if (type == typeof(NpgsqlRange<DateTime>) ||
                type == typeof(NpgsqlRange<Decimal>) ||
                type == typeof(NpgsqlRange<Int32>) ||
                type == typeof(NpgsqlRange<Int64>))
            {
                return NpgsqlDbType.Unknown;
            }
            else if (type == typeof(NpgsqlTid))
            {
                return NpgsqlDbType.Tid;
            }
            else if (type == typeof(NpgsqlTsQuery))
            {
                return NpgsqlDbType.TsQuery;
            }
            else if (type == typeof(NpgsqlTsVector))
            {
                return NpgsqlDbType.TsVector;
            }
            else if (type == typeof(Array))
            {
                return NpgsqlDbType.Unknown;
            }
            else if (type == typeof(Boolean))
            {
                return NpgsqlDbType.Boolean;
            }
            else if (type == typeof(Byte[]))
            {
                return NpgsqlDbType.Bytea;
            }
            else if (type == typeof(Char))
            {
                return NpgsqlDbType.InternalChar;
            }
            else if (type == typeof(System.Collections.BitArray))
            {
                return NpgsqlDbType.Bit;
            }
            else if (type == typeof(DateTime))
            {
                return NpgsqlDbType.Timestamp;
            }
            else if (type == typeof(DateTimeOffset))
            {
                return NpgsqlDbType.TimestampTz;
            }
#if NET
            else if (type == typeof(DateOnly))
            {
                return NpgsqlDbType.Date;
            }
            else if (type == typeof(TimeOnly))
            {
                return NpgsqlDbType.Time;
            }
#endif
            else if (type == typeof(Decimal))
            {
                return NpgsqlDbType.Money;
            }
            else if (type == typeof(Double))
            {
                return NpgsqlDbType.Double;
            }
            else if (type == typeof(Guid))
            {
                return NpgsqlDbType.Uuid;
            }
            else if (type == typeof(Int16))
            {
                return NpgsqlDbType.Smallint;
            }
            else if (type == typeof(Int32))
            {
                return NpgsqlDbType.Integer;
            }
            else if (type == typeof(Int64))
            {
                return NpgsqlDbType.Bigint;
            }
            else if (type == typeof(System.Net.IPAddress))
            {
                return NpgsqlDbType.Inet;
            }
            else if (type == typeof(System.Net.NetworkInformation.PhysicalAddress))
            {
                return NpgsqlDbType.MacAddr;
            }
            else if (type == typeof(Single))
            {
                return NpgsqlDbType.Real;
            }
            else if (type == typeof(String))
            {
                return NpgsqlDbType.Char;
            }
            else if (type == typeof(TimeSpan))
            {
                return NpgsqlDbType.Interval;
            }
            else if (type == typeof(UInt32))
            {
                return NpgsqlDbType.Cid;
            }
            else if (type == typeof(ValueTuple<System.Net.IPAddress, Int32>))
            {
                return NpgsqlDbType.Cidr;
            }

            throw new InvalidOperationException($"The type '{type.FullName}' could not be resolved to '{typeof(NpgsqlDbType).FullName}'.");
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
        //                var schemaTable = reader.GetSchemaTable();
        //                var mappedType = new Dictionary<Type, string>();
        //                var builder = new StringBuilder();
        //                foreach (DataRow row in schemaTable.Rows)
        //                {
        //                    if (row.IsNull("DataType"))
        //                    {
        //                        continue;
        //                    }
        //                    var dataType = (Type)row["DataType"];
        //                    var providerType = Convert.ToInt32(row["ProviderType"]);
        //                    var npgsqlDbType = Enum.GetName(typeof(NpgsqlDbType), providerType);
        //                    if (npgsqlDbType == null)
        //                    {
        //                        continue;
        //                    }
        //                    if (mappedType.ContainsKey(dataType))
        //                    {
        //                        continue;
        //                    }
        //                    mappedType.Add(dataType, npgsqlDbType);
        //                }
        //                var keys = mappedType.Keys.ToArray().OrderBy(e => e.FullName);
        //                foreach (var key in keys)
        //                {
        //                    builder.AppendLine($"else if (type == typeof({key.FullName}))");
        //                    builder.AppendLine("{");
        //                    builder.AppendLine($"   return NpgsqlDbType.{mappedType[key]};");
        //                    builder.AppendLine("}");
        //                }
        //                var extracted = builder.ToString();
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}
