using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests
{
    /// <summary>
    /// A helper class for the integration testing.
    /// </summary>
    public static class Helper
    {
        static Helper()
        {
            StatementBuilder = StatementBuilderMapper.Get<SqlConnection>();
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
                if (propertyOfType1.Name == "Id")
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
        /// <returns></returns>
        public static List<BulkOperationIdentityTable> CreateBulkOperationIdentityTables(int count)
        {
            var tables = new List<BulkOperationIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new BulkOperationIdentityTable
                {
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime = EpocDate.AddDays(index),
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = index,
                    ColumnFloat = index,
                    ColumnInt = index,
                    ColumnNVarChar = $"NVARCHAR{index}"
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static BulkOperationIdentityTable CreateBulkOperationIdentityTable()
        {
            var random = new Random();
            return new BulkOperationIdentityTable
            {
                RowGuid = Guid.NewGuid(),
                ColumnBit = true,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToSingle(random.Next(int.MinValue, int.MaxValue)),
                ColumnInt = random.Next(int.MinValue, int.MaxValue),
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
                table.ColumnBit = true;
                table.ColumnDateTime = EpocDate.AddDays(random.Next(100));
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue));
                table.ColumnFloat = Convert.ToSingle(random.Next(int.MinValue, int.MaxValue));
                //table.ColumnInt = random.Next(int.MinValue, int.MaxValue);
                table.ColumnNVarChar = $"{table.ColumnNVarChar}-Updated";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<ExpandoObject> CreateBulkOperationExpandoObjectIdentityTables(int count)
        {
            var tables = new List<ExpandoObject>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                var item = new ExpandoObject() as IDictionary<string, object>;
                item["RowGuid"] = Guid.NewGuid();
                item["ColumnBit"] = true;
                item["ColumnDateTime"] = EpocDate.AddDays(index);
                item["ColumnDateTime2"] = DateTime.UtcNow;
                item["ColumnDecimal"] = index;
                item["ColumnFloat"] = index;
                item["ColumnInt"] = index;
                item["ColumnNVarChar"] = $"NVARCHAR{index}";
                tables.Add((ExpandoObject)item);
            }
            return tables;
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
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime = EpocDate.AddDays(index),
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = index,
                    ColumnFloat = index,
                    ColumnInt = index,
                    ColumnNVarChar = $"NVARCHAR{index}",
                    ExtraField = $"ExtraField{index}",
                    IdentityTables = new[]
                    {
                        CreateBulkOperationIdentityTable(),
                        CreateBulkOperationIdentityTable()
                    }
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
                ColumnBit = true,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToSingle(random.Next(int.MinValue, int.MaxValue)),
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
                table.ColumnBit = true;
                table.ColumnDateTime = EpocDate.AddDays(random.Next(100));
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue));
                table.ColumnFloat = Convert.ToSingle(random.Next(int.MinValue, int.MaxValue));
                //table.ColumnInt = random.Next(int.MinValue, int.MaxValue);
                table.ColumnNVarChar = $"{table.ColumnNVarChar}-Updated";
                table.ExtraField = $"{table.ExtraField}-Updated";
            }
        }

        #endregion
    }
}
