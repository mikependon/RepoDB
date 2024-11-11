using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Enumerations;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests
{
    /// <summary>
    /// A helper class for the integration testing.
    /// </summary>
    public static class Helper
    {
        static Helper()
        {
            StatementBuilder = StatementBuilderMapper.Get<NpgsqlConnection>();
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

        #region Asserts

        #region TEntity

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="items1"></param>
        /// <param name="items2"></param>
        /// <param name="selector"></param>
        /// <param name="assertItemsCount"></param>
        /// <returns></returns>
        public static int AssertEntitiesEquality<T1, T2>(IEnumerable<T1> items1,
            IEnumerable<T2> items2,
            Func<T1, T2, bool> selector,
            bool assertItemsCount = true)
        {
            if (assertItemsCount)
            {
                Assert.AreEqual(items1.Count(), items2.Count(), "Count is not equal.");
            }

            var result = 0;

            for (var i = 0; i < items1.Count(); i++)
            {
                var item1 = items1.ElementAt(i);
                var item2 = items2.FirstOrDefault(item => selector(item1, item));
                Assert.IsNotNull(item2, $"Item '{i}' from the 1st list has no equivalent item from the 2nd list.");

                AssertEntityEquality(item1, item2);

                result++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        public static void AssertEntityEquality<T1, T2>(T1 item1,
            T2 item2)
        {
            var t1Properties = item1.GetType().GetProperties();
            var t2Properties = item2.GetType().GetProperties();

            foreach (var t1Property in t1Properties)
            {
                if (t1Property.Name == "Id" || t1Property.Name == "IdMapped")
                {
                    continue;
                }

                var t2Property = t2Properties.FirstOrDefault(p => IsMatchingProperty(p.Name, t1Property.Name));
                Assert.IsNotNull(t2Property, $"Property '{t1Property.Name}' is not found from the 2nd list.");

                var value1 = t1Property.GetValue(item1);
                var value2 = t2Property.GetValue(item2);

                Assert.AreEqual(value1, value2, $"Property: {t1Property.Name}");
            }
        }

        #endregion

        #region IDictionary<string, object>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items1"></param>
        /// <param name="items2"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static int AssertExpandoObjectsEquality(IEnumerable<dynamic> items1,
            IEnumerable<dynamic> items2,
            Func<dynamic, dynamic, bool> selector,
            bool assertItemsCount = true)
        {
            if (assertItemsCount)
            {
                Assert.AreEqual(items1.Count(), items2.Count(), "Count is not equal.");
            }

            var result = 0;

            for (var i = 0; i < items1.Count(); i++)
            {
                var item1 = items1.ElementAt(i);
                var item2 = items2.FirstOrDefault(item => selector((IDictionary<string, object>)item1, item));
                Assert.IsNotNull(item2, $"Item '{i}' from the 1st list has no equivalent item from the 2nd list.");

                AssertExpandoObjectEquality(item1, item2);

                result++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        public static void AssertExpandoObjectEquality(dynamic item1,
            dynamic item2)
        {
            var obj1Dictionary = item1 as IDictionary<string, object>;
            var obj2Dictionary = item2 as IDictionary<string, object>;

            AssertDictionaryEquality(obj1Dictionary, obj2Dictionary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items1"></param>
        /// <param name="items2"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static int AssertDictionariesEquality(IEnumerable<IDictionary<string, object>> items1,
            IEnumerable<IDictionary<string, object>> items2,
            Func<IDictionary<string, object>, IDictionary<string, object>, bool> selector)
        {
            Assert.AreEqual(items1.Count(), items2.Count(), "Count is not equal.");

            var result = 0;

            for (var i = 0; i < items1.Count(); i++)
            {
                var item1 = items1.ElementAt(i);
                var item2 = items2.FirstOrDefault(item => selector(item1, item));
                Assert.IsNotNull(item2, $"Item '{i}' from the 1st list has no equivalent item from the 2nd list.");

                AssertDictionaryEquality(item1, item2);

                result++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        public static void AssertDictionaryEquality(IDictionary<string, object> dict1,
            IDictionary<string, object> dict2)
        {
            foreach (var kvp1 in dict1)
            {
                if (kvp1.Key == "Id" || kvp1.Key == "IdMapped")
                {
                    continue;
                }

                object kvp2Value = null;

                foreach (var kvp2 in dict2)
                {
                    if (IsMatchingProperty(kvp1.Key, kvp2.Key))
                    {
                        kvp2Value = kvp2.Value;
                        break;
                    }
                }

                Assert.IsNotNull(kvp2Value, $"Property '{kvp1.Key}' is not found from the 2nd list.");

                Assert.AreEqual(kvp1.Value, kvp2Value, $"Property: {kvp1.Key}");
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static bool IsMatchingProperty(string p1,
            string p2) =>
            string.Equals(p1, p2, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(p1, $"{p2}Mapped", StringComparison.OrdinalIgnoreCase) ||
            string.Equals($"{p1}Mapped", p2, StringComparison.OrdinalIgnoreCase);

        #endregion

        #region Entity

        #region BulkOperationIdentityTable

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="count"></param>
        ///// <param name="hasId"></param>
        ///// <param name="addToKey"></param>
        ///// <returns></returns>
        //public static List<BulkOperationIdentityTable> CreateBulkOperationIdentityTables(int count,
        //    bool hasId = false,
        //    long addToKey = 0)
        //{
        //    var tables = new List<BulkOperationIdentityTable>();
        //    for (var i = 0; i < count; i++)
        //    {
        //        var index = i + 1;
        //        tables.Add(new BulkOperationIdentityTable
        //        {
        //            Id = hasId ? index + addToKey : 0,
        //            ColumnBigInt = index,
        //            ColumnBit = true,
        //            ColumnBoolean = true,
        //            ColumnChar = 'C',
        //            ColumnDate = EpocDate.AddDays(index),
        //            ColumnInteger = index,
        //            ColumnMoney = index,
        //            ColumnNumeric = index,
        //            ColumnReal = index,
        //            ColumnSerial = index,
        //            ColumnSmallInt = (short)index,
        //            ColumnSmallSerial = (short)index,
        //            ColumnText = $"Text-{index}",
        //            ColumnTimestampWithoutTimeZone = EpocDate.AddDays(index),
        //            ColumnTimestampWithTimeZone = EpocDate.AddDays(index),
        //            ColumnTimeWithoutTimeZone = EpocDate.AddDays(index),
        //            ColumnTimeWithTimeZone = EpocDate.AddDays(index)
        //        });
        //    }
        //    return tables;
        //}

        #endregion

        #region BulkOperationLightIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static List<BulkOperationLightIdentityTable> CreateBulkOperationLightIdentityTables(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<BulkOperationLightIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new BulkOperationLightIdentityTable
                {
                    Id = hasId ? index + addToKey : 0,
                    ColumnBigInt = index,
                    ColumnBoolean = true,
                    ColumnInteger = index,
                    ColumnNumeric = index,
                    ColumnReal = index,
                    ColumnSmallInt = (short)index,
                    ColumnText = $"Text-{index}",
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<BulkOperationLightIdentityTable> UpdateBulkOperationLightIdentityTables(List<BulkOperationLightIdentityTable> data)
        {
            foreach (var item in data)
            {
                /*item.ColumnBigInt += 100;
                item.ColumnInteger += 100;*/
                item.ColumnBoolean = false;
                item.ColumnNumeric += 100;
                item.ColumnReal += 100;
                item.ColumnSmallInt += 100;
                item.ColumnText += " (Updated)";
            }
            return data;
        }

        #endregion

        #region BulkOperationMappedIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static List<BulkOperationMappedIdentityTable> CreateBulkOperationMappedIdentityTables(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<BulkOperationMappedIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new BulkOperationMappedIdentityTable
                {
                    IdMapped = hasId ? index + addToKey : 0,
                    ColumnBigIntMapped = index,
                    ColumnBooleanMapped = true,
                    ColumnIntegerMapped = index,
                    ColumnNumericMapped = index,
                    ColumnRealMapped = index,
                    ColumnSmallIntMapped = (short)index,
                    ColumnTextMapped = $"Text-{index}",
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<BulkOperationMappedIdentityTable> UpdateBulkOperationMappedIdentityTables(List<BulkOperationMappedIdentityTable> data)
        {
            foreach (var item in data)
            {
                /*item.ColumnBigIntMapped += 100;
                item.ColumnIntegerMapped += 100;*/
                item.ColumnBooleanMapped = false;
                item.ColumnNumericMapped += 100;
                item.ColumnRealMapped += 100;
                item.ColumnSmallIntMapped += 100;
                item.ColumnTextMapped += " (Updated)";
            }
            return data;
        }

        #endregion

        #region BulkOperationUnmatchedIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static List<BulkOperationUnmatchedIdentityTable> CreateBulkOperationUnmatchedIdentityTables(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<BulkOperationUnmatchedIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new BulkOperationUnmatchedIdentityTable
                {
                    IdMapped = hasId ? index + addToKey : 0,
                    ColumnBigIntMapped = (long)index,
                    ColumnBooleanMapped = true,
                    ColumnIntegerMapped = index,
                    ColumnNumericMapped = (decimal)index,
                    ColumnRealMapped = (float)index,
                    ColumnSmallIntMapped = (short)index,
                    ColumnTextMapped = $"Text-{index}",
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<BulkOperationUnmatchedIdentityTable> UpdateBulkOperationUnmatchedIdentityTables(List<BulkOperationUnmatchedIdentityTable> data)
        {
            foreach (var item in data)
            {
                /*item.ColumnBigIntMapped += 100;
                item.ColumnIntegerMapped += 100;*/
                item.ColumnBooleanMapped = false;
                item.ColumnNumericMapped += 100;
                item.ColumnRealMapped += 100;
                item.ColumnSmallIntMapped += 100;
                item.ColumnTextMapped += " (Updated)";
            }
            return data;
        }

        #endregion

        #region EnumTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static List<EnumTable> CreateEnumTables(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<EnumTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new EnumTable
                {
                    Id = (long)(hasId ? index + addToKey : 0),
                    ColumnEnumHand = Hands.Right,
                    ColumnEnumInt = Hands.Left,
                    ColumnEnumText = Hands.Unidentified
                });
            }
            return tables;
        }

        #endregion

        #endregion

        #region Anonymous

        #region BulkOperationAnonymousLightIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static List<dynamic> CreateBulkOperationAnonymousLightIdentityTables(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new
                {
                    Id = (long)(hasId ? index + addToKey : 0),
                    ColumnBigInt = (long)index,
                    ColumnBoolean = true,
                    ColumnInteger = index,
                    ColumnNumeric = (decimal)index,
                    ColumnReal = (float)index,
                    ColumnSmallInt = (short)index,
                    ColumnText = $"Text-{index}",
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static List<dynamic> UpdateBulkOperationAnonymousLightIdentityTables(List<dynamic> data)
        {
            var tables = new List<dynamic>();
            foreach (var item in data)
            {
                tables.Add(new
                {
                    Id = (long)item.Id,
                    ColumnBigInt = (long)item.ColumnBigInt,
                    ColumnInteger = (int)item.ColumnInteger,
                    ColumnBoolean = false,
                    ColumnNumeric = (decimal)(item.ColumnNumeric + 100),
                    ColumnReal = (float)(item.ColumnReal + 100),
                    ColumnSmallInt = (short)(item.ColumnSmallInt + 100),
                    ColumnText = $"{item.ColumnText} (Updated)",
                });
            }
            return tables;
        }

        #endregion

        #region BulkOperationAnonymousUnmatchedIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static List<dynamic> CreateBulkOperationAnonymousUnmatchedIdentityTables(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new
                {
                    IdMapped = (long)(hasId ? index + addToKey : 0),
                    ColumnBigIntMapped = (long)index,
                    ColumnBooleanMapped = true,
                    ColumnIntegerMapped = index,
                    ColumnNumericMapped = (decimal)index,
                    ColumnRealMapped = (float)index,
                    ColumnSmallIntMapped = (short)index,
                    ColumnTextMapped = $"Text-{index}",
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<dynamic> UpdateBulkOperationAnonymousUnmatchedIdentityTables(List<dynamic> data)
        {
            var tables = new List<dynamic>();
            foreach (var item in data)
            {
                tables.Add(new
                {
                    IdMapped = (long)item.IdMapped,
                    ColumnBigIntMapped = (long)item.ColumnBigIntMapped,
                    ColumnIntegerMapped = (int)item.ColumnIntegerMapped,
                    ColumnBooleanMapped = false,
                    ColumnNumericMapped = (decimal)(item.ColumnNumericMapped + 100),
                    ColumnRealMapped = (float)(item.ColumnRealMapped + 100),
                    ColumnSmallIntMapped = (short)(item.ColumnSmallIntMapped + 100),
                    ColumnTextMapped = $"{item.ColumnTextMapped} (Updated)",
                });
            }
            return tables;
        }

        #endregion

        #region EnumTableAnonymousTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static List<dynamic> CreateEnumTableAnonymousTables(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new
                {
                    Id = (long)(hasId ? index + addToKey : 0),
                    ColumnEnumHand = Hands.Right,
                    ColumnEnumInt = Hands.Left,
                    ColumnEnumText = Hands.Unidentified
                });
            }
            return tables;
        }

        #endregion

        #endregion

        #region ExpandoObject

        #region BulkOperationExpandoObjectLightIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static List<dynamic> CreateBulkOperationExpandoObjectLightIdentityTables(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                var index = i + 1;
                expandoObject["Id"] = (long)(hasId ? index + addToKey : 0);
                expandoObject["ColumnBigInt"] = (long)index;
                expandoObject["ColumnBoolean"] = true;
                expandoObject["ColumnInteger"] = index;
                expandoObject["ColumnNumeric"] = (decimal)index;
                expandoObject["ColumnReal"] = (float)index;
                expandoObject["ColumnSmallInt"] = (short)index;
                expandoObject["ColumnText"] = $"Text-{index}";
                tables.Add((ExpandoObject)expandoObject);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static List<dynamic> UpdateBulkOperationExpandoObjectLightIdentityTables(List<dynamic> data)
        {
            foreach (ExpandoObject item in data)
            {
                var dictionary = item as IDictionary<string, object>;
                /*dictionary["ColumnBigInt"] = (long)dictionary["ColumnBigInt"];
                dictionary["ColumnInteger"] = (int)dictionary["ColumnInteger"];*/
                dictionary["ColumnBoolean"] = false;
                dictionary["ColumnNumeric"] = (decimal)((decimal)dictionary["ColumnNumeric"] + 100);
                dictionary["ColumnReal"] = (float)((float)dictionary["ColumnReal"] + 100);
                dictionary["ColumnSmallInt"] = (short)((short)dictionary["ColumnSmallInt"] + 100);
                dictionary["ColumnText"] = $"{dictionary["ColumnText"]} (Updated)";
            }
            return data;
        }

        #endregion

        #region BulkOperationExpandoObjectUnmatchedIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static List<dynamic> CreateBulkOperationExpandoObjectUnmatchedIdentityTables(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                var index = i + 1;
                expandoObject["IdMapped"] = (long)(hasId ? index + addToKey : 0);
                expandoObject["ColumnBigIntMapped"] = (long)index;
                expandoObject["ColumnBooleanMapped"] = true;
                expandoObject["ColumnIntegerMapped"] = index;
                expandoObject["ColumnNumericMapped"] = (decimal)index;
                expandoObject["ColumnRealMapped"] = (float)index;
                expandoObject["ColumnSmallIntMapped"] = (short)index;
                expandoObject["ColumnTextMapped"] = $"Text-{index}";
                tables.Add((ExpandoObject)expandoObject);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static List<dynamic> UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(List<dynamic> data)
        {
            foreach (ExpandoObject item in data)
            {
                var dictionary = item as IDictionary<string, object>;
                /*dictionary["ColumnBigIntMapped"] = (long)dictionary["ColumnBigIntMapped"]);
                dictionary["ColumnIntegerMapped"] = (int)dictionary["ColumnIntegerMapped"]);*/
                dictionary["ColumnBooleanMapped"] = false;
                dictionary["ColumnNumericMapped"] = (decimal)((decimal)dictionary["ColumnNumericMapped"] + 100);
                dictionary["ColumnRealMapped"] = (float)((float)dictionary["ColumnRealMapped"] + 100);
                dictionary["ColumnSmallIntMapped"] = (short)((short)dictionary["ColumnSmallIntMapped"] + 100);
                dictionary["ColumnTextMapped"] = $"{dictionary["ColumnTextMapped"]} (Updated)";
            }
            return data;
        }

        #endregion

        #region EnumTableExpandoObjectTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static List<dynamic> CreateEnumTableExpandoObjectTables(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                var index = i + 1;
                expandoObject["Id"] = (long)(hasId ? index + addToKey : 0);
                expandoObject["ColumnEnumHand"] = Hands.Right;
                expandoObject["ColumnEnumInt"] = (int?)null; // Hands.Left;
                expandoObject["ColumnEnumText"] = (string)null; // Hands.Unidentified;
                tables.Add((ExpandoObject)expandoObject);
            }
            return tables;
        }

        #endregion

        #endregion

        #region DataTable

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<TEntity>(string tableName,
            IEnumerable<TEntity> entities)
            where TEntity : class
        {
            var table = new DataTable() { TableName = tableName ?? ClassMappedNameCache.Get<TEntity>() };
            var properties = PropertyCache.Get(entities?.First()?.GetType() ?? typeof(TEntity));

            // Columns
            foreach (var property in properties)
            {
                table.Columns.Add(property.PropertyInfo.Name, property.PropertyInfo.PropertyType.GetUnderlyingType());
            }

            // Rows
            foreach (var entity in entities)
            {
                var row = table.NewRow();

                foreach (var property in properties)
                {
                    var value = property.PropertyInfo.GetValue(entity);
                    row[property.PropertyInfo.Name] = value == null ? DBNull.Value : value;
                }

                table.Rows.Add(row);
            }

            // Return
            return table;
        }

        #region BulkOperationDataTableLightIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static DataTable CreateBulkOperationDataTableLightIdentityTables(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = CreateBulkOperationLightIdentityTables(count, hasId, addToKey);
            return ToDataTable("BulkOperationIdentityTable", tables);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable UpdateBulkOperationDataTableLightIdentityTables(List<BulkOperationLightIdentityTable> data) =>
            ToDataTable("BulkOperationIdentityTable", UpdateBulkOperationLightIdentityTables(data));

        #endregion

        #region BulkOperationDataTableUnmatchedIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static DataTable CreateBulkOperationDataTableUnmatchedIdentityTables(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = CreateBulkOperationUnmatchedIdentityTables(count, hasId, addToKey);
            return ToDataTable("BulkOperationIdentityTable", tables);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable UpdateBulkOperationDataTableUnmatchedIdentityTables(List<BulkOperationUnmatchedIdentityTable> data) =>
            ToDataTable("BulkOperationIdentityTable", UpdateBulkOperationUnmatchedIdentityTables(data));

        #endregion

        #region EnumTableDataTableTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <param name="addToKey"></param>
        /// <returns></returns>
        public static DataTable CreateEnumTableDataTable(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = CreateEnumTables(count, hasId, addToKey);
            return ToDataTable("EnumTable", tables);
        }

        #endregion

        #endregion
    }
}
