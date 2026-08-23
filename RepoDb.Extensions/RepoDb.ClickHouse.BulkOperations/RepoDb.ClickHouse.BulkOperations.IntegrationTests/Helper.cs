using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.ClickHouse.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ClickHouse.Driver.ADO;
using System.Threading;

namespace RepoDb.ClickHouse.BulkOperations.IntegrationTests
{
    /// <summary>
    /// A helper class for the integration testing.
    /// </summary>
    public static class Helper
    {
        static Helper()
        {
            StatementBuilder = StatementBuilderMapper.Get<ClickHouseConnection>();
            EpocDate = new DateTime(1970, 1, 1, 0, 0, 0);
        }

        #region Properties

        /// <summary>
        /// Gets the instance of <see cref="IStatementBuilder"/> object.
        /// </summary>
        public static IStatementBuilder StatementBuilder { get; }

        /// <summary>
        /// Gets the value of the Epoc date.
        /// </summary>
        public static DateTime EpocDate { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static DateTime UtcNowMicroseconds() =>
            new DateTime(DateTime.UtcNow.Ticks / 10 * 10, DateTimeKind.Utc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        public static void StopMerges(ClickHouseConnection connection,
            string tableName)
        {
            connection.ExecuteNonQuery($"SYSTEM STOP MERGES `{tableName}`;");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        public static void StartMerges(ClickHouseConnection connection,
            string tableName)
        {
            connection.ExecuteNonQuery($"SYSTEM START MERGES `{tableName}`;");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        public static void SetupAsyncInsert(ClickHouseConnection connection)
        {
            connection.ExecuteNonQuery("SET async_insert = 1;");
            connection.ExecuteNonQuery("SET wait_for_async_insert = 1;");
        }

        /// <summary>
        /// Asserts the properties equality of 2 types.
        /// </summary>
        /// <typeparam name="T1">The type of first object.</typeparam>
        /// <typeparam name="T2">The type of second object.</typeparam>
        /// <param name="t1">The instance of first object.</param>
        /// <param name="t2">The instance of second object.</param>
        public static void AssertPropertiesEquality<T1, T2>(T1 t1, T2 t2)
        {
            var propertiesOfType1 = typeof(T1).GetProperties();
            var propertiesOfType2 = typeof(T2).GetProperties();
            propertiesOfType1.AsList().ForEach(propertyOfType1 =>
            {
                if (propertyOfType1.Name == "Id" || propertyOfType1.Name == "IdMapped")
                {
                    return;
                }
                var propertyOfType2 = propertiesOfType2.FirstOrDefault(p => p.Name == propertyOfType1.Name);
                if (propertyOfType2 == null)
                {
                    return;
                }
                var value1 = propertyOfType1.GetValue(t1);
                var value2 = propertyOfType2.GetValue(t2);
                if (value1 is byte[] && value2 is byte[])
                {
                    var b1 = (byte[])value1;
                    var b2 = (byte[])value2;
                    for (var i = 0; i < Math.Min(b1.Length, b2.Length); i++)
                    {
                        var v1 = b1[i];
                        var v2 = b2[i];
                        Assert.AreEqual(v1, v2,
                            $"Assert failed for '{propertyOfType1.Name}'. The values are '{value1} ({propertyOfType1.PropertyType.FullName})' and '{value2} ({propertyOfType2.PropertyType.FullName})'.");
                    }
                }
                else
                {
                    Assert.AreEqual(value1, value2,
                        $"Assert failed for '{propertyOfType1.Name}'. The values are '{value1} ({propertyOfType1.PropertyType.FullName})' and '{value2} ({propertyOfType2.PropertyType.FullName})'.");
                }
            });
        }

        /// <summary>
        /// Asserts the members equality of 2 object and <see cref="ExpandoObject"/>.
        /// </summary>
        /// <typeparam name="T">The type of first object.</typeparam>
        /// <param name="obj">The instance of first object.</param>
        /// <param name="expandoObj">The instance of second object.</param>
        public static void AssertMembersEquality(object obj, object expandoObj)
        {
            var dictionary = new ExpandoObject() as IDictionary<string, object>;
            foreach (var property in expandoObj.GetType().GetProperties())
            {
                dictionary.Add(property.Name, property.GetValue(expandoObj));
            }
            AssertMembersEquality(obj, dictionary);
        }

        /// <summary>
        /// Asserts the members equality of 2 object and <see cref="ExpandoObject"/>.
        /// </summary>
        /// <typeparam name="T">The type of first object.</typeparam>
        /// <param name="obj">The instance of first object.</param>
        /// <param name="expandoObj">The instance of second object.</param>
        public static void AssertMembersEquality(object obj, ExpandoObject expandoObj)
        {
            var dictionary = expandoObj as IDictionary<string, object>;
            AssertMembersEquality(obj, dictionary);
        }

        /// <summary>
        /// Asserts the members equality of 2 objects.
        /// </summary>
        /// <typeparam name="T">The type of first object.</typeparam>
        /// <param name="obj">The instance of first object.</param>
        /// <param name="dictionary">The instance of second object.</param>
        public static void AssertMembersEquality(object obj, IDictionary<string, object> dictionary)
        {
            var properties = obj.GetType().GetProperties();
            properties.AsList().ForEach(property =>
            {
                if (property.Name == "Id")
                {
                    return;
                }
                if (dictionary.ContainsKey(property.Name))
                {
                    var value1 = property.GetValue(obj);
                    var value2 = dictionary[property.Name];
                    if (value1 is byte[] && value2 is byte[])
                    {
                        var b1 = (byte[])value1;
                        var b2 = (byte[])value2;
                        for (var i = 0; i < Math.Min(b1.Length, b2.Length); i++)
                        {
                            var v1 = b1[i];
                            var v2 = b2[i];
                            Assert.AreEqual(v1, v2,
                                $"Assert failed for '{property.Name}'. The values are '{v1}' and '{v2}'.");
                        }
                    }
                    else
                    {
                        var propertyType = property.PropertyType.GetUnderlyingType();
                        if (propertyType == typeof(TimeSpan) && value2 is DateTime)
                        {
                            value2 = ((DateTime)value2).TimeOfDay;
                        }
                        Assert.AreEqual(Convert.ChangeType(value1, propertyType), Convert.ChangeType(value2, propertyType),
                            $"Assert failed for '{property.Name}'. The values are '{value1}' and '{value2}'.");
                    }
                }
            });
        }

        #endregion

        #region BulkOperationIdentityTable

        /*
         * Actual Class
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<BulkOperationIdentityTable> CreateBulkOperationIdentityTables(int count)
        {
            var random = new Random();
            var tables = new List<BulkOperationIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new BulkOperationIdentityTable
                {
                    Id = DateTime.UtcNow.Ticks,
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = 1,
                    ColumnDateTime = EpocDate.AddDays(random.Next(100)),
                    ColumnDateTime2 = UtcNowMicroseconds(),
                    ColumnDecimal = random.Next(100),
                    ColumnFloat = random.Next(100),
                    ColumnInt = random.Next(100),
                    ColumnNVarChar = $"NVARCHAR{random.Next(100)}"
                });
                Thread.Sleep(1);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static BulkOperationIdentityTable CreateBulkOperationIdentityTable()
        {
            var random = new Random();
            return new BulkOperationIdentityTable
            {
                Id = random.Next(1000),
                RowGuid = Guid.NewGuid(),
                ColumnBit = 1,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = UtcNowMicroseconds(),
                ColumnDecimal = Convert.ToDecimal(random.Next(100)),
                ColumnFloat = Convert.ToDouble(random.Next(100)),
                ColumnInt = random.Next(100),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static void UpdateBulkOperationIdentityTables(List<BulkOperationIdentityTable> tables)
        {
            var random = new Random();
            foreach (var table in tables)
            {
                //table.RowGuid = Guid.NewGuid();
                table.ColumnBit = 1;
                table.ColumnDateTime = EpocDate.AddDays(random.Next(100));
                table.ColumnDateTime2 = UtcNowMicroseconds();
                table.ColumnDecimal = Convert.ToDecimal(random.Next(100));
                table.ColumnFloat = Convert.ToDouble(random.Next(100));
                //table.ColumnInt = random.Next(100);
                table.ColumnNVarChar = $"{table.ColumnNVarChar}-Updated";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static void UpdateBulkOperationIdentityTable(BulkOperationIdentityTable table)
        {
            var random = new Random();
            //table.RowGuid = Guid.NewGuid();
            table.ColumnBit = 1;
            table.ColumnDateTime = EpocDate.AddDays(random.Next(100));
            table.ColumnDateTime2 = UtcNowMicroseconds();
            table.ColumnDecimal = Convert.ToDecimal(random.Next(100));
            table.ColumnFloat = Convert.ToDouble(random.Next(100));
            //table.ColumnInt = random.Next(100);
            table.ColumnNVarChar = $"{table.ColumnNVarChar}-Updated";
        }

        #endregion

        #region BulkOperationNonIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<BulkOperationNonIdentityTable> CreateBulkOperationNonIdentityTables(int count)
        {
            var random = new Random();
            var tables = new List<BulkOperationNonIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new BulkOperationNonIdentityTable
                {
                    Id = DateTime.UtcNow.Ticks,
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = 1,
                    ColumnDateTime = EpocDate.AddDays(random.Next(100)),
                    ColumnDateTime2 = UtcNowMicroseconds(),
                    ColumnDecimal = random.Next(100),
                    ColumnFloat = random.Next(100),
                    ColumnInt = random.Next(100),
                    ColumnNVarChar = $"NVARCHAR{random.Next(100)}"
                });
                Thread.Sleep(1);
            }
            return tables;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static BulkOperationNonIdentityTable CreateBulkOperationNonIdentityTable()
        {
            var random = new Random();
            return new BulkOperationNonIdentityTable
            {
                Id = DateTime.UtcNow.Ticks,
                RowGuid = Guid.NewGuid(),
                ColumnBit = 1,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = UtcNowMicroseconds(),
                ColumnDecimal = Convert.ToDecimal(random.Next(100)),
                ColumnFloat = Convert.ToDouble(random.Next(100)),
                ColumnInt = random.Next(100),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tables"></param>
        public static void UpdateBulkOperationNonIdentityTables(List<BulkOperationNonIdentityTable> tables)
        {
            var random = new Random();
            foreach (var table in tables)
            {
                //table.RowGuid = Guid.NewGuid();
                table.ColumnBit = 1;
                table.ColumnDateTime = EpocDate.AddDays(random.Next(100));
                table.ColumnDateTime2 = UtcNowMicroseconds();
                table.ColumnDecimal = Convert.ToDecimal(random.Next(100));
                table.ColumnFloat = Convert.ToDouble(random.Next(100));
                //table.ColumnInt = random.Next(100);
                table.ColumnNVarChar = $"{table.ColumnNVarChar}-Updated";
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateBulkOperationNonIdentityTable(BulkOperationNonIdentityTable table)
        {
            var random = new Random();
            //table.RowGuid = Guid.NewGuid();
            table.ColumnBit = 1;
            table.ColumnDateTime = EpocDate.AddDays(random.Next(100));
            table.ColumnDateTime2 = UtcNowMicroseconds();
            table.ColumnDecimal = Convert.ToDecimal(random.Next(100));
            table.ColumnFloat = Convert.ToDouble(random.Next(100));
            //table.ColumnInt = random.Next(100);
            table.ColumnNVarChar = $"{table.ColumnNVarChar}-Updated";
        }

        #endregion

        #region BulkOperationNonIdentityTable

        /*
         * Anonymous Objects
         */

        /// <summary>
        ///
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<dynamic> CreateBulkOperationAnonymousObjectNonIdentityTables(int count)
        {
            var random = new Random();
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new
                {
                    Id = DateTime.UtcNow.Ticks,
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = (byte)1,
                    ColumnDateTime = EpocDate.AddDays(random.Next(100)),
                    ColumnDateTime2 = UtcNowMicroseconds(),
                    ColumnDecimal = Convert.ToDecimal(random.Next(100)),
                    ColumnFloat = Convert.ToDouble(random.Next(100)),
                    ColumnInt = random.Next(100),
                    ColumnNVarChar = $"NVARCHAR{random.Next(100)}"
                });
                Thread.Sleep(1);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static dynamic CreateBulkOperationAnonymousObjectNonIdentityTable()
        {
            var random = new Random();
            return new
            {
                Id = DateTime.UtcNow.Ticks,
                RowGuid = Guid.NewGuid(),
                ColumnBit = (byte)1,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = UtcNowMicroseconds(),
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToDouble(random.Next(int.MinValue, int.MaxValue)),
                ColumnInt = random.Next(int.MinValue, int.MaxValue),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static List<dynamic> UpdateBulkOperationAnonymousObjectNonIdentityTables(List<dynamic> tables)
        {
            var random = new Random();
            var list = new List<dynamic>();
            foreach (var table in tables)
            {
                list.Add(new
                {
                    Id = table.Id,
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = (byte)1,
                    ColumnDateTime = EpocDate.AddDays(random.Next(100)),
                    ColumnDateTime2 = UtcNowMicroseconds(),
                    ColumnDecimal = Convert.ToDecimal(random.Next(100)),
                    ColumnFloat = Convert.ToDouble(random.Next(100)),
                    ColumnInt = random.Next(100),
                    ColumnNVarChar = $"NVARCHAR{random.Next(100)}-Updated"
                });
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static dynamic UpdateBulkOperationAnonymousObjectNonIdentityTable(dynamic table)
        {
            var random = new Random();
            return new
            {
                Id = table.Id,
                //RowGuid = Guid.NewGuid(),
                ColumnBit = (byte)1,
                ColumnDateTime = EpocDate.AddDays(random.Next(100)),
                ColumnDateTime2 = UtcNowMicroseconds(),
                ColumnDecimal = Convert.ToDecimal(random.Next(100)),
                ColumnFloat = Convert.ToDouble(random.Next(100)),
                //ColumnInt = random.Next(100),
                ColumnNVarChar = $"NVARCHAR{random.Next(100)}-Updated"
            };
        }

        /*
         * IDictionary<string, object>
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<ExpandoObject> CreateBulkOperationExpandoObjectNonIdentityTables(int count)
        {
            var random = new Random();
            var tables = new List<ExpandoObject>();
            for (var i = 0; i < count; i++)
            {
                var item = new ExpandoObject() as IDictionary<string, object>;
                item["Id"] = DateTime.UtcNow.Ticks;
                item["RowGuid"] = Guid.NewGuid();
                item["ColumnBit"] = (byte)1;
                item["ColumnDateTime"] = EpocDate.AddDays(random.Next(100));
                item["ColumnDateTime2"] = UtcNowMicroseconds();
                item["ColumnDecimal"] = random.Next(100);
                item["ColumnFloat"] = random.Next(100);
                item["ColumnInt"] = random.Next(100);
                item["ColumnNVarChar"] = $"NVARCHAR{DateTime.UtcNow.Ticks}";
                tables.Add((ExpandoObject)item);
                Thread.Sleep(1);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public static List<ExpandoObject> UpdateBulkOperationExpandoObjectNonIdentityTables(List<ExpandoObject> tables)
        {
            var random = new Random();
            foreach (var table in tables)
            {
                var item = table as IDictionary<string, object>;
                //item["RowGuid"] = Guid.NewGuid();
                item["ColumnBit"] = (byte)1;
                item["ColumnDateTime"] = EpocDate.AddDays(random.Next(100));
                item["ColumnDateTime2"] = UtcNowMicroseconds();
                item["ColumnDecimal"] = random.Next(100);
                item["ColumnFloat"] = random.Next(100);
                //item["ColumnInt"] = random.Next(100);
                item["ColumnNVarChar"] = $"{item["ColumnNVarChar"]}-Updated";
                tables.Add((ExpandoObject)item);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ExpandoObject CreateBulkOperationExpandoObjectNonIdentityTable()
        {
            var random = new Random();
            var item = new ExpandoObject() as IDictionary<string, object>;
            item["Id"] = DateTime.UtcNow.Ticks;
            //item["RowGuid"] = Guid.NewGuid();
            item["ColumnBit"] = (byte)1;
            item["ColumnDateTime"] = EpocDate.AddDays(random.Next(100));
            item["ColumnDateTime2"] = UtcNowMicroseconds();
            item["ColumnDecimal"] = random.Next(100);
            item["ColumnFloat"] = random.Next(100);
            //item["ColumnInt"] = random.Next(100);
            item["ColumnNVarChar"] = $"NVARCHAR{random.Next(100)}";
            return (ExpandoObject)item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateBulkOperationExpandoObjectNonIdentityTable(ExpandoObject table)
        {
            var random = new Random();
            var item = table as IDictionary<string, object>;
            //item["RowGuid"] = Guid.NewGuid();
            item["ColumnBit"] = (byte)1;
            item["ColumnDateTime"] = EpocDate.AddDays(random.Next(100));
            item["ColumnDateTime2"] = UtcNowMicroseconds();
            item["ColumnDecimal"] = random.Next(100);
            item["ColumnFloat"] = random.Next(100);
            //item["ColumnInt"] = random.Next(100);
            item["ColumnNVarChar"] = $"{item["ColumnNVarChar"]}-Updated";
        }

        #endregion

        #region BulkOperationMappedNonIdentityTable

        /*
         * Actual Class
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<BulkOperationMappedNonIdentityTable> CreateBulkOperationMappedNonIdentityTables(int count)
        {
            var random = new Random();
            var tables = new List<BulkOperationMappedNonIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new BulkOperationMappedNonIdentityTable
                {
                    IdMapped = DateTime.UtcNow.Ticks,
                    RowGuidMapped = Guid.NewGuid(),
                    ColumnBitMapped = 1,
                    ColumnDateTimeMapped = EpocDate.AddDays(random.Next(100)),
                    ColumnDateTime2Mapped = UtcNowMicroseconds(),
                    ColumnDecimalMapped = random.Next(100),
                    ColumnFloatMapped = random.Next(100),
                    ColumnIntMapped = random.Next(100),
                    ColumnNVarCharMapped = $"NVARCHAR{random.Next(100)}"
                });
                Thread.Sleep(1);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static BulkOperationMappedNonIdentityTable CreateBulkOperationMappedNonIdentityTable()
        {
            var random = new Random();
            return new BulkOperationMappedNonIdentityTable
            {
                IdMapped = DateTime.UtcNow.Ticks,
                RowGuidMapped = Guid.NewGuid(),
                ColumnBitMapped = 1,
                ColumnDateTimeMapped = EpocDate,
                ColumnDateTime2Mapped = UtcNowMicroseconds(),
                ColumnDecimalMapped = Convert.ToDecimal(random.Next(100)),
                ColumnFloatMapped = Convert.ToDouble(random.Next(100)),
                ColumnIntMapped = random.Next(100),
                ColumnNVarCharMapped = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static void UpdateBulkOperationMappedNonIdentityTables(List<BulkOperationMappedNonIdentityTable> tables)
        {
            var random = new Random();
            foreach (var table in tables)
            {
                //table.RowGuid = Guid.NewGuid();
                table.ColumnBitMapped = 1;
                table.ColumnDateTimeMapped = EpocDate.AddDays(random.Next(100));
                table.ColumnDateTime2Mapped = UtcNowMicroseconds();
                table.ColumnDecimalMapped = Convert.ToDecimal(random.Next(100));
                table.ColumnFloatMapped = Convert.ToDouble(random.Next(100));
                //table.UnmatchedColumnInt = random.Next(100);
                table.ColumnNVarCharMapped = $"{table.ColumnNVarCharMapped}-Updated";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static void UpdateBulkOperationMappedNonIdentityTable(BulkOperationMappedNonIdentityTable table)
        {
            var random = new Random();
            //table.RowGuid = Guid.NewGuid();
            table.ColumnBitMapped = 1;
            table.ColumnDateTimeMapped = EpocDate.AddDays(random.Next(100));
            table.ColumnDateTime2Mapped = UtcNowMicroseconds();
            table.ColumnDecimalMapped = Convert.ToDecimal(random.Next(100));
            table.ColumnFloatMapped = Convert.ToDouble(random.Next(100));
            //table.UnmatchedColumnInt = random.Next(100);
            table.ColumnNVarCharMapped = $"{table.ColumnNVarCharMapped}-Updated";
        }

        /*
         * Anonymous Objects
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<dynamic> CreateBulkOperationAnonymousObjectMappedNonIdentityTables(int count)
        {
            var random = new Random();
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new
                {
                    Id = DateTime.UtcNow.Ticks,
                    RowGuid = Guid.NewGuid(),
                    UnmatchedColumnBit = (byte)1,
                    UnmatchedColumnDateTime = EpocDate.AddDays(random.Next(100)),
                    UnmatchedColumnDateTime2 = UtcNowMicroseconds(),
                    UnmatchedColumnDecimal = Convert.ToDecimal(random.Next(100)),
                    UnmatchedColumnFloat = Convert.ToDouble(random.Next(100)),
                    UnmatchedColumnInt = random.Next(100),
                    UnmatchedColumnNVarChar = $"NVARCHAR{random.Next(100)}"
                });
                Thread.Sleep(1);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static dynamic CreateBulkOperationAnonymousObjectMappedNonIdentityTable()
        {
            var random = new Random();
            return new
            {
                Id = DateTime.UtcNow.Ticks,
                RowGuid = Guid.NewGuid(),
                UnmatchedColumnBit = (byte)1,
                UnmatchedColumnDateTime = EpocDate,
                UnmatchedColumnDateTime2 = UtcNowMicroseconds(),
                UnmatchedColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                UnmatchedColumnFloat = Convert.ToDouble(random.Next(int.MinValue, int.MaxValue)),
                UnmatchedColumnInt = random.Next(int.MinValue, int.MaxValue),
                UnmatchedColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static List<dynamic> UpdateBulkOperationAnonymousObjectMappedNonIdentityTables(List<dynamic> tables)
        {
            var random = new Random();
            var list = new List<dynamic>();
            foreach (var table in tables)
            {
                list.Add(new
                {
                    Id = table.Id,
                    RowGuid = Guid.NewGuid(),
                    UnmatchedColumnBit = (byte)1,
                    UnmatchedColumnDateTime = EpocDate.AddDays(random.Next(100)),
                    UnmatchedColumnDateTime2 = UtcNowMicroseconds(),
                    UnmatchedColumnDecimal = Convert.ToDecimal(random.Next(100)),
                    UnmatchedColumnFloat = Convert.ToDouble(random.Next(100)),
                    UnmatchedColumnInt = random.Next(100),
                    UnmatchedColumnNVarChar = $"NVARCHAR{random.Next(100)}-Updated"
                });
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static dynamic UpdateBulkOperationAnonymousObjectMappedNonIdentityTable(dynamic table)
        {
            var random = new Random();
            return new
            {
                Id = table.Id,
                //RowGuid = Guid.NewGuid(),
                UnmatchedColumnBit = (byte)1,
                UnmatchedColumnDateTime = EpocDate.AddDays(random.Next(100)),
                UnmatchedColumnDateTime2 = UtcNowMicroseconds(),
                UnmatchedColumnDecimal = Convert.ToDecimal(random.Next(100)),
                UnmatchedColumnFloat = Convert.ToDouble(random.Next(100)),
                //UnmatchedColumnInt = random.Next(100),
                UnmatchedColumnNVarChar = $"NVARCHAR{random.Next(100)}-Updated"
            };
        }

        /*
         * IDictionary<string, object>
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<ExpandoObject> CreateBulkOperationExpandoObjectMappedNonIdentityTables(int count)
        {
            var random = new Random();
            var tables = new List<ExpandoObject>();
            for (var i = 0; i < count; i++)
            {
                var item = new ExpandoObject() as IDictionary<string, object>;
                item["Id"] = DateTime.UtcNow.Ticks;
                item["RowGuid"] = Guid.NewGuid();
                item["UnmatchedColumnBit"] = (byte)1;
                item["UnmatchedColumnDateTime"] = EpocDate.AddDays(random.Next(100));
                item["UnmatchedColumnDateTime2"] = UtcNowMicroseconds();
                item["UnmatchedColumnDecimal"] = random.Next(100);
                item["UnmatchedColumnFloat"] = random.Next(100);
                item["UnmatchedColumnInt"] = random.Next(100);
                item["UnmatchedColumnNVarChar"] = $"NVARCHAR{DateTime.UtcNow.Ticks}";
                tables.Add((ExpandoObject)item);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public static List<ExpandoObject> UpdateBulkOperationExpandoObjectMappedNonIdentityTables(List<ExpandoObject> tables)
        {
            var random = new Random();
            foreach (var table in tables)
            {
                var item = table as IDictionary<string, object>;
                //item["RowGuid"] = Guid.NewGuid();
                item["UnmatchedColumnBit"] = (byte)1;
                item["UnmatchedColumnDateTime"] = EpocDate.AddDays(random.Next(100));
                item["UnmatchedColumnDateTime2"] = UtcNowMicroseconds();
                item["UnmatchedColumnDecimal"] = random.Next(100);
                item["UnmatchedColumnFloat"] = random.Next(100);
                //item["UnmatchedColumnInt"] = random.Next(100);
                item["UnmatchedColumnNVarChar"] = $"{item["UnmatchedColumnNVarChar"]}-Updated";
                tables.Add((ExpandoObject)item);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static ExpandoObject CreateBulkOperationExpandoObjectMappedNonIdentityTable()
        {
            var random = new Random();
            var item = new ExpandoObject() as IDictionary<string, object>;
            item["Id"] = DateTime.UtcNow.Ticks;
            //item["RowGuid"] = Guid.NewGuid();
            item["UnmatchedColumnBit"] = (byte)1;
            item["UnmatchedColumnDateTime"] = EpocDate.AddDays(random.Next(100));
            item["UnmatchedColumnDateTime2"] = UtcNowMicroseconds();
            item["UnmatchedColumnDecimal"] = random.Next(100);
            item["UnmatchedColumnFloat"] = random.Next(100);
            //item["UnmatchedColumnInt"] = random.Next(100);
            item["UnmatchedColumnNVarChar"] = $"NVARCHAR{random.Next(100)}";
            return (ExpandoObject)item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateBulkOperationExpandoObjectMappedNonIdentityTable(ExpandoObject table)
        {
            var random = new Random();
            var item = table as IDictionary<string, object>;
            //item["RowGuid"] = Guid.NewGuid();
            item["UnmatchedColumnBit"] = (byte)1;
            item["UnmatchedColumnDateTime"] = EpocDate.AddDays(random.Next(100));
            item["UnmatchedColumnDateTime2"] = UtcNowMicroseconds();
            item["UnmatchedColumnDecimal"] = random.Next(100);
            item["UnmatchedColumnFloat"] = random.Next(100);
            //item["UnmatchedColumnInt"] = random.Next(100);
            item["UnmatchedColumnNVarChar"] = $"{item["UnmatchedColumnNVarChar"]}-Updated";
        }

        #endregion

        #region WithExtraFieldsBulkOperationNonIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<WithExtraFieldsBulkOperationNonIdentityTable> CreateWithExtraFieldsBulkOperationNonIdentityTables(int count)
        {
            var tables = new List<WithExtraFieldsBulkOperationNonIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new WithExtraFieldsBulkOperationNonIdentityTable
                {
                    Id = index,
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = 1,
                    ColumnDateTime = EpocDate.AddDays(index),
                    ColumnDateTime2 = UtcNowMicroseconds(),
                    ColumnDecimal = index,
                    ColumnFloat = index,
                    ColumnInt = index,
                    ColumnNVarChar = $"NVARCHAR{index}",
                    ExtraField = $"ExtraField{index}",
                    NonIdentityTables = new[]
                    {
                        CreateBulkOperationNonIdentityTable(),
                        CreateBulkOperationNonIdentityTable()
                    }
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WithExtraFieldsBulkOperationNonIdentityTable CreateWithExtraFieldsBulkOperationNonIdentityTable()
        {
            var random = new Random();
            return new WithExtraFieldsBulkOperationNonIdentityTable
            {
                RowGuid = Guid.NewGuid(),
                ColumnBit = 1,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = UtcNowMicroseconds(),
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToDouble(random.Next(int.MinValue, int.MaxValue)),
                ColumnInt = random.Next(int.MinValue, int.MaxValue),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static void UpdateWithExtraFieldsBulkOperationNonIdentityTables(List<WithExtraFieldsBulkOperationNonIdentityTable> tables)
        {
            var random = new Random();
            foreach (var table in tables)
            {
                //table.RowGuid = Guid.NewGuid();
                table.ColumnBit = 1;
                table.ColumnDateTime = EpocDate.AddDays(random.Next(100));
                table.ColumnDateTime2 = UtcNowMicroseconds();
                table.ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue));
                table.ColumnFloat = Convert.ToDouble(random.Next(int.MinValue, int.MaxValue));
                //table.ColumnInt = random.Next(int.MinValue, int.MaxValue);
                table.ColumnNVarChar = $"{table.ColumnNVarChar}-Updated";
                table.ExtraField = $"{table.ExtraField}-Updated";
            }
        }

        #endregion

        #region BulkOperationIdentityTable

        /*
         * Anonymous Objects
         */

        /// <summary>
        ///
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<dynamic> CreateBulkOperationAnonymousObjectIdentityTables(int count)
        {
            var random = new Random();
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new
                {
                    Id = DateTime.UtcNow.Ticks,
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = (byte)1,
                    ColumnDateTime = EpocDate.AddDays(random.Next(100)),
                    ColumnDateTime2 = UtcNowMicroseconds(),
                    ColumnDecimal = Convert.ToDecimal(random.Next(100)),
                    ColumnFloat = Convert.ToDouble(random.Next(100)),
                    ColumnInt = random.Next(100),
                    ColumnNVarChar = $"NVARCHAR{random.Next(100)}"
                });
                Thread.Sleep(1);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static dynamic CreateBulkOperationAnonymousObjectIdentityTable()
        {
            var random = new Random();
            return new
            {
                Id = DateTime.UtcNow.Ticks,
                RowGuid = Guid.NewGuid(),
                ColumnBit = (byte)1,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = UtcNowMicroseconds(),
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToDouble(random.Next(int.MinValue, int.MaxValue)),
                ColumnInt = random.Next(int.MinValue, int.MaxValue),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static List<dynamic> UpdateBulkOperationAnonymousObjectIdentityTables(List<dynamic> tables)
        {
            var random = new Random();
            var list = new List<dynamic>();
            foreach (var table in tables)
            {
                list.Add(new
                {
                    Id = table.Id,
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = (byte)1,
                    ColumnDateTime = EpocDate.AddDays(random.Next(100)),
                    ColumnDateTime2 = UtcNowMicroseconds(),
                    ColumnDecimal = Convert.ToDecimal(random.Next(100)),
                    ColumnFloat = Convert.ToDouble(random.Next(100)),
                    ColumnInt = random.Next(100),
                    ColumnNVarChar = $"NVARCHAR{random.Next(100)}-Updated"
                });
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static dynamic UpdateBulkOperationAnonymousObjectIdentityTable(dynamic table)
        {
            var random = new Random();
            return new
            {
                Id = table.Id,
                //RowGuid = Guid.NewGuid(),
                ColumnBit = (byte)1,
                ColumnDateTime = EpocDate.AddDays(random.Next(100)),
                ColumnDateTime2 = UtcNowMicroseconds(),
                ColumnDecimal = Convert.ToDecimal(random.Next(100)),
                ColumnFloat = Convert.ToDouble(random.Next(100)),
                //ColumnInt = random.Next(100),
                ColumnNVarChar = $"NVARCHAR{random.Next(100)}-Updated"
            };
        }

        /*
         * IDictionary<string, object>
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<ExpandoObject> CreateBulkOperationExpandoObjectIdentityTables(int count)
        {
            var random = new Random();
            var tables = new List<ExpandoObject>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                var item = new ExpandoObject() as IDictionary<string, object>;
                item["Id"] = DateTime.UtcNow.Ticks;
                item["RowGuid"] = Guid.NewGuid();
                item["ColumnBit"] = (byte)1;
                item["ColumnDateTime"] = EpocDate.AddDays(random.Next(100));
                item["ColumnDateTime2"] = UtcNowMicroseconds();
                item["ColumnDecimal"] = random.Next(100);
                item["ColumnFloat"] = random.Next(100);
                item["ColumnInt"] = random.Next(100);
                item["ColumnNVarChar"] = $"NVARCHAR{DateTime.UtcNow.Ticks}";
                tables.Add((ExpandoObject)item);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public static List<ExpandoObject> UpdateBulkOperationExpandoObjectIdentityTables(List<ExpandoObject> tables)
        {
            var random = new Random();
            foreach (var table in tables)
            {
                var item = table as IDictionary<string, object>;
                //item["RowGuid"] = Guid.NewGuid();
                item["ColumnBit"] = (byte)1;
                item["ColumnDateTime"] = EpocDate.AddDays(random.Next(100));
                item["ColumnDateTime2"] = UtcNowMicroseconds();
                item["ColumnDecimal"] = random.Next(100);
                item["ColumnFloat"] = random.Next(100);
                //item["ColumnInt"] = random.Next(100);
                item["ColumnNVarChar"] = $"{item["ColumnNVarChar"]}-Updated";
                tables.Add((ExpandoObject)item);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static ExpandoObject CreateBulkOperationExpandoObjectIdentityTable()
        {
            var random = new Random();
            var item = new ExpandoObject() as IDictionary<string, object>;
            item["Id"] = DateTime.UtcNow.Ticks;
            //item["RowGuid"] = Guid.NewGuid();
            item["ColumnBit"] = (byte)1;
            item["ColumnDateTime"] = EpocDate.AddDays(random.Next(100));
            item["ColumnDateTime2"] = UtcNowMicroseconds();
            item["ColumnDecimal"] = random.Next(100);
            item["ColumnFloat"] = random.Next(100);
            //item["ColumnInt"] = random.Next(100);
            item["ColumnNVarChar"] = $"NVARCHAR{random.Next(100)}";
            return (ExpandoObject)item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateBulkOperationExpandoObjectIdentityTable(ExpandoObject table)
        {
            var random = new Random();
            var item = table as IDictionary<string, object>;
            //item["RowGuid"] = Guid.NewGuid();
            item["ColumnBit"] = (byte)1;
            item["ColumnDateTime"] = EpocDate.AddDays(random.Next(100));
            item["ColumnDateTime2"] = UtcNowMicroseconds();
            item["ColumnDecimal"] = random.Next(100);
            item["ColumnFloat"] = random.Next(100);
            //item["ColumnInt"] = random.Next(100);
            item["ColumnNVarChar"] = $"{item["ColumnNVarChar"]}-Updated";
        }

        #endregion

        #region BulkOperationMappedIdentityTable

        /*
         * Actual Class
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<BulkOperationMappedIdentityTable> CreateBulkOperationMappedIdentityTables(int count)
        {
            var random = new Random();
            var tables = new List<BulkOperationMappedIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new BulkOperationMappedIdentityTable
                {
                    IdMapped = DateTime.UtcNow.Ticks,
                    RowGuidMapped = Guid.NewGuid(),
                    ColumnBitMapped = 1,
                    ColumnDateTimeMapped = EpocDate.AddDays(random.Next(100)),
                    ColumnDateTime2Mapped = UtcNowMicroseconds(),
                    ColumnDecimalMapped = random.Next(100),
                    ColumnFloatMapped = random.Next(100),
                    ColumnIntMapped = random.Next(100),
                    ColumnNVarCharMapped = $"NVARCHAR{random.Next(100)}"
                });
                Thread.Sleep(1);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static BulkOperationMappedIdentityTable CreateBulkOperationMappedIdentityTable()
        {
            var random = new Random();
            return new BulkOperationMappedIdentityTable
            {
                IdMapped = DateTime.UtcNow.Ticks,
                RowGuidMapped = Guid.NewGuid(),
                ColumnBitMapped = 1,
                ColumnDateTimeMapped = EpocDate,
                ColumnDateTime2Mapped = UtcNowMicroseconds(),
                ColumnDecimalMapped = Convert.ToDecimal(random.Next(100)),
                ColumnFloatMapped = Convert.ToDouble(random.Next(100)),
                ColumnIntMapped = random.Next(100),
                ColumnNVarCharMapped = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static void UpdateBulkOperationMappedIdentityTables(List<BulkOperationMappedIdentityTable> tables)
        {
            var random = new Random();
            foreach (var table in tables)
            {
                //table.RowGuid = Guid.NewGuid();
                table.ColumnBitMapped = 1;
                table.ColumnDateTimeMapped = EpocDate.AddDays(random.Next(100));
                table.ColumnDateTime2Mapped = UtcNowMicroseconds();
                table.ColumnDecimalMapped = Convert.ToDecimal(random.Next(100));
                table.ColumnFloatMapped = Convert.ToDouble(random.Next(100));
                //table.UnmatchedColumnInt = random.Next(100);
                table.ColumnNVarCharMapped = $"{table.ColumnNVarCharMapped}-Updated";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static void UpdateBulkOperationMappedIdentityTable(BulkOperationMappedIdentityTable table)
        {
            var random = new Random();
            //table.RowGuid = Guid.NewGuid();
            table.ColumnBitMapped = 1;
            table.ColumnDateTimeMapped = EpocDate.AddDays(random.Next(100));
            table.ColumnDateTime2Mapped = UtcNowMicroseconds();
            table.ColumnDecimalMapped = Convert.ToDecimal(random.Next(100));
            table.ColumnFloatMapped = Convert.ToDouble(random.Next(100));
            //table.UnmatchedColumnInt = random.Next(100);
            table.ColumnNVarCharMapped = $"{table.ColumnNVarCharMapped}-Updated";
        }

        /*
         * Anonymous Objects
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<dynamic> CreateBulkOperationAnonymousObjectMappedIdentityTables(int count)
        {
            var random = new Random();
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new
                {
                    Id = DateTime.UtcNow.Ticks,
                    RowGuid = Guid.NewGuid(),
                    UnmatchedColumnBit = (byte)1,
                    UnmatchedColumnDateTime = EpocDate.AddDays(random.Next(100)),
                    UnmatchedColumnDateTime2 = UtcNowMicroseconds(),
                    UnmatchedColumnDecimal = Convert.ToDecimal(random.Next(100)),
                    UnmatchedColumnFloat = Convert.ToDouble(random.Next(100)),
                    UnmatchedColumnInt = random.Next(100),
                    UnmatchedColumnNVarChar = $"NVARCHAR{random.Next(100)}"
                });
                Thread.Sleep(1);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static dynamic CreateBulkOperationAnonymousObjectMappedIdentityTable()
        {
            var random = new Random();
            return new
            {
                Id = DateTime.UtcNow.Ticks,
                RowGuid = Guid.NewGuid(),
                UnmatchedColumnBit = (byte)1,
                UnmatchedColumnDateTime = EpocDate,
                UnmatchedColumnDateTime2 = UtcNowMicroseconds(),
                UnmatchedColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                UnmatchedColumnFloat = Convert.ToDouble(random.Next(int.MinValue, int.MaxValue)),
                UnmatchedColumnInt = random.Next(int.MinValue, int.MaxValue),
                UnmatchedColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static List<dynamic> UpdateBulkOperationAnonymousObjectMappedIdentityTables(List<dynamic> tables)
        {
            var random = new Random();
            var list = new List<dynamic>();
            foreach (var table in tables)
            {
                list.Add(new
                {
                    Id = table.Id,
                    RowGuid = Guid.NewGuid(),
                    UnmatchedColumnBit = (byte)1,
                    UnmatchedColumnDateTime = EpocDate.AddDays(random.Next(100)),
                    UnmatchedColumnDateTime2 = UtcNowMicroseconds(),
                    UnmatchedColumnDecimal = Convert.ToDecimal(random.Next(100)),
                    UnmatchedColumnFloat = Convert.ToDouble(random.Next(100)),
                    UnmatchedColumnInt = random.Next(100),
                    UnmatchedColumnNVarChar = $"NVARCHAR{random.Next(100)}-Updated"
                });
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static dynamic UpdateBulkOperationAnonymousObjectMappedIdentityTable(dynamic table)
        {
            var random = new Random();
            return new
            {
                Id = table.Id,
                //RowGuid = Guid.NewGuid(),
                UnmatchedColumnBit = (byte)1,
                UnmatchedColumnDateTime = EpocDate.AddDays(random.Next(100)),
                UnmatchedColumnDateTime2 = UtcNowMicroseconds(),
                UnmatchedColumnDecimal = Convert.ToDecimal(random.Next(100)),
                UnmatchedColumnFloat = Convert.ToDouble(random.Next(100)),
                //UnmatchedColumnInt = random.Next(100),
                UnmatchedColumnNVarChar = $"NVARCHAR{random.Next(100)}-Updated"
            };
        }

        /*
         * IDictionary<string, object>
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<ExpandoObject> CreateBulkOperationExpandoObjectMappedIdentityTables(int count
            )
        {
            var random = new Random();
            var tables = new List<ExpandoObject>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                var item = new ExpandoObject() as IDictionary<string, object>;
                item["Id"] = DateTime.UtcNow.Ticks;
                item["RowGuid"] = Guid.NewGuid();
                item["UnmatchedColumnBit"] = (byte)1;
                item["UnmatchedColumnDateTime"] = EpocDate.AddDays(random.Next(100));
                item["UnmatchedColumnDateTime2"] = UtcNowMicroseconds();
                item["UnmatchedColumnDecimal"] = random.Next(100);
                item["UnmatchedColumnFloat"] = random.Next(100);
                item["UnmatchedColumnInt"] = random.Next(100);
                item["UnmatchedColumnNVarChar"] = $"NVARCHAR{DateTime.UtcNow.Ticks}";
                tables.Add((ExpandoObject)item);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public static List<ExpandoObject> UpdateBulkOperationExpandoObjectMappedIdentityTables(List<ExpandoObject> tables)
        {
            var random = new Random();
            foreach (var table in tables)
            {
                var item = table as IDictionary<string, object>;
                //item["RowGuid"] = Guid.NewGuid();
                item["UnmatchedColumnBit"] = (byte)1;
                item["UnmatchedColumnDateTime"] = EpocDate.AddDays(random.Next(100));
                item["UnmatchedColumnDateTime2"] = UtcNowMicroseconds();
                item["UnmatchedColumnDecimal"] = random.Next(100);
                item["UnmatchedColumnFloat"] = random.Next(100);
                //item["UnmatchedColumnInt"] = random.Next(100);
                item["UnmatchedColumnNVarChar"] = $"{item["UnmatchedColumnNVarChar"]}-Updated";
                tables.Add((ExpandoObject)item);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static ExpandoObject CreateBulkOperationExpandoObjectMappedIdentityTable()
        {
            var random = new Random();
            var item = new ExpandoObject() as IDictionary<string, object>;
            item["Id"] = DateTime.UtcNow.Ticks;
            //item["RowGuid"] = Guid.NewGuid();
            item["UnmatchedColumnBit"] = (byte)1;
            item["UnmatchedColumnDateTime"] = EpocDate.AddDays(random.Next(100));
            item["UnmatchedColumnDateTime2"] = UtcNowMicroseconds();
            item["UnmatchedColumnDecimal"] = random.Next(100);
            item["UnmatchedColumnFloat"] = random.Next(100);
            //item["UnmatchedColumnInt"] = random.Next(100);
            item["UnmatchedColumnNVarChar"] = $"NVARCHAR{random.Next(100)}";
            return (ExpandoObject)item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateBulkOperationExpandoObjectMappedIdentityTable(ExpandoObject table)
        {
            var random = new Random();
            var item = table as IDictionary<string, object>;
            //item["RowGuid"] = Guid.NewGuid();
            item["UnmatchedColumnBit"] = (byte)1;
            item["UnmatchedColumnDateTime"] = EpocDate.AddDays(random.Next(100));
            item["UnmatchedColumnDateTime2"] = UtcNowMicroseconds();
            item["UnmatchedColumnDecimal"] = random.Next(100);
            item["UnmatchedColumnFloat"] = random.Next(100);
            //item["UnmatchedColumnInt"] = random.Next(100);
            item["UnmatchedColumnNVarChar"] = $"{item["UnmatchedColumnNVarChar"]}-Updated";
        }

        #endregion

        #region WithExtraFieldsBulkOperationIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<WithExtraFieldsBulkOperationIdentityTable> CreateWithExtraFieldsBulkOperationIdentityTables(int count)
        {
            var tables = new List<WithExtraFieldsBulkOperationIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new WithExtraFieldsBulkOperationIdentityTable
                {
                    Id = index,
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = 1,
                    ColumnDateTime = EpocDate.AddDays(index),
                    ColumnDateTime2 = UtcNowMicroseconds(),
                    ColumnDecimal = index,
                    ColumnFloat = index,
                    ColumnInt = index,
                    ColumnNVarChar = $"NVARCHAR{index}",
                    ExtraField = $"ExtraField{index}"
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WithExtraFieldsBulkOperationIdentityTable CreateWithExtraFieldsBulkOperationIdentityTable()
        {
            var random = new Random();
            return new WithExtraFieldsBulkOperationIdentityTable
            {
                RowGuid = Guid.NewGuid(),
                ColumnBit = 1,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = UtcNowMicroseconds(),
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToDouble(random.Next(int.MinValue, int.MaxValue)),
                ColumnInt = random.Next(int.MinValue, int.MaxValue),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public static void UpdateWithExtraFieldsBulkOperationIdentityTables(List<WithExtraFieldsBulkOperationIdentityTable> tables)
        {
            var random = new Random();
            foreach (var table in tables)
            {
                //table.RowGuid = Guid.NewGuid();
                table.ColumnBit = 1;
                table.ColumnDateTime = EpocDate.AddDays(random.Next(100));
                table.ColumnDateTime2 = UtcNowMicroseconds();
                table.ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue));
                table.ColumnFloat = Convert.ToDouble(random.Next(int.MinValue, int.MaxValue));
                //table.ColumnInt = random.Next(int.MinValue, int.MaxValue);
                table.ColumnNVarChar = $"{table.ColumnNVarChar}-Updated";
                table.ExtraField = $"{table.ExtraField}-Updated";
            }
        }

        #endregion
    }
}
