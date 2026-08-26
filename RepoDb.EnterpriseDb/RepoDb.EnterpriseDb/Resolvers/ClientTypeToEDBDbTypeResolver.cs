using EDBTypes;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the .NET CLR Type into its equivalent <see cref="EDBDbType"/>.
    /// </summary>
    public class ClientTypeToEDBDbTypeResolver : IResolver<Type, EDBDbType?>
    {
        /// <summary>
        /// Returns the equivalent <see cref="EDBDbType"/> based from the .NET CLR Type.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <returns>The equivalent <see cref="EDBDbType"/>.</returns>
        public virtual EDBDbType? Resolve(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The type must not be null.");
            }

            if (type == typeof(EDBBox))
            {
                return EDBDbType.Box;
            }
            else if (type == typeof(EDBCircle))
            {
                return EDBDbType.Circle;
            }
            else if (type == typeof(EDBLine))
            {
                return EDBDbType.Line;
            }
            else if (type == typeof(EDBLogSequenceNumber))
            {
                return EDBDbType.PgLsn;
            }
            else if (type == typeof(EDBLSeg))
            {
                return EDBDbType.LSeg;
            }
            else if (type == typeof(EDBPath))
            {
                return EDBDbType.Path;
            }
            else if (type == typeof(EDBPoint))
            {
                return EDBDbType.Point;
            }
            else if (type == typeof(EDBPolygon))
            {
                return EDBDbType.Polygon;
            }
            else if (type == typeof(EDBRange<DateTime>) ||
                type == typeof(EDBRange<Decimal>) ||
                type == typeof(EDBRange<Int32>) ||
                type == typeof(EDBRange<Int64>))
            {
                return EDBDbType.Unknown;
            }
            else if (type == typeof(EDBTid))
            {
                return EDBDbType.Tid;
            }
            else if (type == typeof(EDBTsQuery))
            {
                return EDBDbType.TsQuery;
            }
            else if (type == typeof(EDBTsVector))
            {
                return EDBDbType.TsVector;
            }
            else if (type == typeof(Array))
            {
                return EDBDbType.Unknown;
            }
            else if (type == typeof(Boolean))
            {
                return EDBDbType.Boolean;
            }
            else if (type == typeof(Byte[]))
            {
                return EDBDbType.Bytea;
            }
            else if (type == typeof(Char))
            {
                return EDBDbType.InternalChar;
            }
            else if (type == typeof(System.Collections.BitArray))
            {
                return EDBDbType.Bit;
            }
            else if (type == typeof(DateTime))
            {
                return EDBDbType.Timestamp;
            }
            else if (type == typeof(DateTimeOffset))
            {
                return EDBDbType.TimestampTz;
            }
            #if NET6_0_OR_GREATER
            else if (type == typeof(DateOnly))
            {
                return EDBDbType.Date;
            }
            else if (type == typeof(TimeOnly))
            {
                return EDBDbType.Time;
            }
            #endif
            else if (type == typeof(Decimal))
            {
                return EDBDbType.Money;
            }
            else if (type == typeof(Double))
            {
                return EDBDbType.Double;
            }
            else if (type == typeof(Guid))
            {
                return EDBDbType.Uuid;
            }
            else if (type == typeof(Int16))
            {
                return EDBDbType.Smallint;
            }
            else if (type == typeof(Int32))
            {
                return EDBDbType.Integer;
            }
            else if (type == typeof(Int64))
            {
                return EDBDbType.Bigint;
            }
            else if (type == typeof(System.Net.IPAddress))
            {
                return EDBDbType.Inet;
            }
            else if (type == typeof(System.Net.NetworkInformation.PhysicalAddress))
            {
                return EDBDbType.MacAddr;
            }
            else if (type == typeof(Single))
            {
                return EDBDbType.Real;
            }
            else if (type == typeof(String))
            {
                return EDBDbType.Char;
            }
            else if (type == typeof(TimeSpan))
            {
                return EDBDbType.Interval;
            }
            else if (type == typeof(UInt32))
            {
                return EDBDbType.Cid;
            }
            else if (type == typeof(ValueTuple<System.Net.IPAddress, Int32>))
            {
                return EDBDbType.Cidr;
            }

            throw new InvalidOperationException($"The type '{type.FullName}' could not be resolved to '{typeof(EDBDbType).FullName}'.");
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
        //                    var edbDbType = Enum.GetName(typeof(EDBDbType), providerType);
        //                    if (edbDbType == null)
        //                    {
        //                        continue;
        //                    }
        //                    if (mappedType.ContainsKey(dataType))
        //                    {
        //                        continue;
        //                    }
        //                    mappedType.Add(dataType, edbDbType);
        //                }
        //                var keys = mappedType.Keys.ToArray().OrderBy(e => e.FullName);
        //                foreach (var key in keys)
        //                {
        //                    builder.AppendLine($"else if (type == typeof({key.FullName}))");
        //                    builder.AppendLine("{");
        //                    builder.AppendLine($"   return EDBDbType.{mappedType[key]};");
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
