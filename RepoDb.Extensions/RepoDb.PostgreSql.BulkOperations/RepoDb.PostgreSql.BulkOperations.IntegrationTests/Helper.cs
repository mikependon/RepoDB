using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<BulkOperationIdentityTable> CreateBulkOperationIdentityTables(int count,
            bool hasId = false)
        {
            var random = new Random();
            var tables = new List<BulkOperationIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new BulkOperationIdentityTable
                {
                    Id = hasId ? index : 0,
                    ColumnBigInt = random.Next(100),
                    ColumnBit = true,
                    ColumnBoolean = true,
                    ColumnChar = 'C',
                    ColumnDate = EpocDate.AddDays(random.Next(100)),
                    ColumnInteger = random.Next(100),
                    ColumnMoney = random.Next(100),
                    ColumnNumeric = random.Next(100),
                    ColumnReal = random.Next(100),
                    ColumnSerial = random.Next(100),
                    ColumnSmallInt = (short)random.Next(100),
                    ColumnSmallSerial = (short)random.Next(100),
                    ColumnText = $"Text-{index}",
                    ColumnTimestampWithoutTimeZone = EpocDate.AddDays(random.Next(100)),
                    ColumnTimestampWithTimeZone = EpocDate.AddDays(random.Next(100)),
                    ColumnTimeWithoutTimeZone = EpocDate.AddDays(random.Next(100)),
                    ColumnTimeWithTimeZone = EpocDate.AddDays(random.Next(100))
                });
            }
            return tables;
        }

        #endregion

        #region BulkOperationLightIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<BulkOperationLightIdentityTable> CreateBulkOperationLightIdentityTables(int count,
            bool hasId = false)
        {
            var random = new Random();
            var tables = new List<BulkOperationLightIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new BulkOperationLightIdentityTable
                {
                    Id = hasId ? index : 0,
                    ColumnBigInt = random.Next(100),
                    ColumnBoolean = true,
                    ColumnInteger = random.Next(100),
                    ColumnNumeric = random.Next(100),
                    ColumnReal = random.Next(100),
                    ColumnSmallInt = (short)random.Next(100),
                    ColumnText = $"Text-{index}",
                });
            }
            return tables;
        }

        #endregion

        #region BulkOperationMappedIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<BulkOperationMappedIdentityTable> CreateBulkOperationMappedIdentityTables(int count,
            bool hasId = false)
        {
            var random = new Random();
            var tables = new List<BulkOperationMappedIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new BulkOperationMappedIdentityTable
                {
                    IdMapped = hasId ? index : 0,
                    ColumnBigIntMapped = random.Next(100),
                    ColumnBooleanMapped = true,
                    ColumnIntegerMapped = random.Next(100),
                    ColumnNumericMapped = random.Next(100),
                    ColumnRealMapped = random.Next(100),
                    ColumnSmallIntMapped = (short)random.Next(100),
                    ColumnTextMapped = $"Text-{index}",
                });
            }
            return tables;
        }

        #endregion

        #region CreateBulkOperationUnmatchedIdentityTables

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<BulkOperationUnmatchedIdentityTable> CreateBulkOperationUnmatchedIdentityTables(int count,
            bool hasId = false)
        {
            var random = new Random();
            var tables = new List<BulkOperationUnmatchedIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new BulkOperationUnmatchedIdentityTable
                {
                    IdMapped = hasId ? index : 0,
                    ColumnBigIntMapped = (long)random.Next(100),
                    ColumnBooleanMapped = true,
                    ColumnIntegerMapped = random.Next(100),
                    ColumnNumericMapped = (decimal)random.Next(100),
                    ColumnRealMapped = (float)random.Next(100),
                    ColumnSmallIntMapped = (short)random.Next(100),
                    ColumnTextMapped = $"Text-{index}",
                });
            }
            return tables;
        }

        #endregion

        #region CreateBulkOperationAnonymousLightIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<dynamic> CreateBulkOperationAnonymousLightIdentityTables(int count,
            bool hasId = false)
        {
            var random = new Random();
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new
                {
                    Id = hasId ? index : 0,
                    ColumnBigInt = (long)random.Next(100),
                    ColumnBoolean = true,
                    ColumnInteger = random.Next(100),
                    ColumnNumeric = (decimal)random.Next(100),
                    ColumnReal = (float)random.Next(100),
                    ColumnSmallInt = (short)random.Next(100),
                    ColumnText = $"Text-{index}",
                });
            }
            return tables;
        }

        #endregion

        #region CreateBulkOperationAnonymousUnmatchedIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<dynamic> CreateBulkOperationAnonymousUnmatchedIdentityTables(int count,
            bool hasId = false)
        {
            var random = new Random();
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new
                {
                    IdMapped = (long)(hasId ? index : 0),
                    ColumnBigIntMapped = (long)random.Next(100),
                    ColumnBooleanMapped = true,
                    ColumnIntegerMapped = random.Next(100),
                    ColumnNumericMapped = (decimal)random.Next(100),
                    ColumnRealMapped = (float)random.Next(100),
                    ColumnSmallIntMapped = (short)random.Next(100),
                    ColumnTextMapped = $"Text-{index}",
                });
            }
            return tables;
        }

        #endregion

        #region CreateBulkOperationExpandoObjectLightIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<ExpandoObject> CreateBulkOperationExpandoObjectLightIdentityTables(int count,
            bool hasId = false)
        {
            var random = new Random();
            var tables = new List<ExpandoObject>();
            for (var i = 0; i < count; i++)
            {
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                var index = i + 1;

                expandoObject["Id"] = hasId ? index : 0;
                expandoObject["ColumnBigInt"] = (long)random.Next(100);
                expandoObject["ColumnBoolean"] = true;
                expandoObject["ColumnInteger"] = random.Next(100);
                expandoObject["ColumnNumeric"] = (decimal)random.Next(100);
                expandoObject["ColumnReal"] = (float)random.Next(100);
                expandoObject["ColumnSmallInt"] = (short)random.Next(100);
                expandoObject["ColumnText"] = $"Text -{index}";

                tables.Add((ExpandoObject)expandoObject);
            }
            return tables;
        }

        #endregion

        #region CreateBulkOperationExpandoObjectUnmatchedIdentityTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="hasId"></param>
        /// <returns></returns>
        public static List<ExpandoObject> CreateBulkOperationExpandoObjectUnmatchedIdentityTables(int count,
            bool hasId = false)
        {
            var random = new Random();
            var tables = new List<ExpandoObject>();
            for (var i = 0; i < count; i++)
            {
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                var index = i + 1;

                expandoObject["IdMapped"] = hasId ? index : 0;
                expandoObject["ColumnBigIntMapped"] = (long)random.Next(100);
                expandoObject["ColumnBooleanMapped"] = true;
                expandoObject["ColumnIntegerMapped"] = random.Next(100);
                expandoObject["ColumnNumericMapped"] = (decimal)random.Next(100);
                expandoObject["ColumnRealMapped"] = (float)random.Next(100);
                expandoObject["ColumnSmallIntMapped"] = (short)random.Next(100);
                expandoObject["ColumnTextMapped"] = $"Text -{index}";

                tables.Add((ExpandoObject)expandoObject);
            }
            return tables;
        }

        #endregion
    }
}
