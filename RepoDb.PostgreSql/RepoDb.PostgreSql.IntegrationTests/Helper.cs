using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace RepoDb.PostgreSql.IntegrationTests
{
    public static class Helper
    {
        static Helper()
        {
            EpocDate = new DateTime(1970, 1, 1, 0, 0, 0);
        }

        #region Properties

        /// <summary>
        /// Gets the value of the Epoc date.
        /// </summary>
        public static DateTime EpocDate { get; }

        /// <summary>
        /// Gets the current <see cref="Random"/> object in used.
        /// </summary>
        public static Random Randomizer => new(1);

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
                if (value1 is Array array1 && value2 is Array array2)
                {
                    for (var i = 0; i < Math.Min(array1.Length, array2.Length); i++)
                    {
                        var v1 = array1.GetValue(i);
                        var v2 = array2.GetValue(i);
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
                    if (value1 is Array array1 && value2 is Array array2)
                    {
                        for (var i = 0; i < Math.Min(array1.Length, array2.Length); i++)
                        {
                            var v1 = array1.GetValue(i);
                            var v2 = array2.GetValue(i);
                            Assert.AreEqual(v1, v2,
                                $"Assert failed for '{property.Name}'. The values are '{v1}' and '{v2}'.");
                        }
                    }
                    else
                    {
                        var propertyType = property.PropertyType.GetUnderlyingType();
                        if (propertyType == typeof(TimeSpan) && value2 is DateTime dateTime)
                        {
                            value2 = dateTime.TimeOfDay;
                        }
                        Assert.AreEqual(Convert.ChangeType(value1, propertyType), Convert.ChangeType(value2, propertyType),
                            $"Assert failed for '{property.Name}'. The values are '{value1}' and '{value2}'.");
                    }
                }
            });
        }

        #endregion

        #region CompleteTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<CompleteTable> CreateCompleteTables(int count)
        {
            var tables = new List<CompleteTable>();
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            for (var i = 0; i < count; i++)
            {
                tables.Add(new CompleteTable
                {
                    Id = (i + 1),
                    ColumnBigInt = Convert.ToInt64(i),
                    ColumnBigIntAsArray = new long[] { 1, 2, 3, 4, 5 },
                    ColumnBigSerial = Convert.ToInt64(i),
                    //ColumnBit = true,
                    ColumnBoolean = true,
                    ColumnChar = 'C',
                    ColumnCharacter = "C",
                    ColumnCharacterVarying = "ColumnCharacterVarying",
                    ColumnDate = now.Date,
                    ColumnDateAsArray = new[] { now.Date, now.Date, now.Date },
                    ColumnInteger = Convert.ToInt32(i),
                    ColumnIntegerAsArray = new[] { 1, 2, 3, 4, 5 },
                    ColumnInterval = now.TimeOfDay,
                    ColumnIntervalAsArray = new[] { now.TimeOfDay, now.TimeOfDay, now.TimeOfDay },
                    //ColumnJson = "{\"field1\": 1, \"field2\": 2}",
                    ColumnMoney = Convert.ToDecimal(i),
                    ColumnName = $"ColumnName{i}",
                    ColumnReal = Convert.ToSingle(i),
                    ColumnSmallInt = Convert.ToInt16(i),
                    ColumnText = $"ColumnText{i}",
                    ColumnTimestampWithTimeZone = now,
                    ColumnTimestampWithoutTimeZone = now
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateCompleteTableProperties(CompleteTable table)
        {
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            table.ColumnBigInt = Convert.ToInt64(2);
            table.ColumnBigIntAsArray = new long[] { 1, 2, 3, 4, 5 };
            table.ColumnBigSerial = Convert.ToInt64(2);
            //table.ColumnBit = true;
            table.ColumnBoolean = (Randomizer.Next() % 2 != 0);
            table.ColumnChar = 'C';
            table.ColumnCharacter = "C";
            table.ColumnCharacterVarying = "ColumnCharacterVarying";
            table.ColumnDate = now.Date;
            table.ColumnDateAsArray = new[] { now.Date, now.Date, now.Date };
            table.ColumnInteger = Convert.ToInt32(2);
            table.ColumnIntegerAsArray = new[] { 1, 2, 3, 4, 5 };
            table.ColumnInterval = now.TimeOfDay;
            table.ColumnIntervalAsArray = new[] { now.TimeOfDay, now.TimeOfDay, now.TimeOfDay };
            //table.ColumnJson = "{\"field1\": 1, \"field2\": 2}";
            table.ColumnMoney = Convert.ToDecimal(2);
            table.ColumnName = $"{table.ColumnName} (Updated)";
            table.ColumnReal = Convert.ToSingle(2);
            table.ColumnSmallInt = Convert.ToInt16(2);
            table.ColumnText = $"{table.ColumnText} (Updated)";
            table.ColumnTimestampWithTimeZone = now;
            table.ColumnTimestampWithoutTimeZone = now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<dynamic> CreateCompleteTablesAsDynamics(int count)
        {
            var tables = new List<dynamic>();
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            for (var i = 0; i < count; i++)
            {
                tables.Add(new
                {
                    Id = (long)(i + 1),
                    ColumnBigInt = Convert.ToInt64(i),
                    ColumnBigIntAsArray = new long[] { 1, 2, 3, 4, 5 },
                    ColumnBigSerial = Convert.ToInt64(i),
                    //ColumnBit = true,
                    ColumnBoolean = true,
                    ColumnChar = 'C',
                    ColumnCharacter = "C",
                    ColumnCharacterVarying = "ColumnCharacterVarying",
                    ColumnDate = now.Date,
                    ColumnDateAsArray = new[] { now.Date, now.Date, now.Date },
                    ColumnInteger = Convert.ToInt32(i),
                    ColumnIntegerAsArray = new[] { 1, 2, 3, 4, 5 },
                    ColumnInterval = now.TimeOfDay,
                    ColumnIntervalAsArray = new[] { now.TimeOfDay, now.TimeOfDay, now.TimeOfDay },
                    //ColumnJson = "{\"field1\": 1, \"field2\": 2}",
                    ColumnMoney = Convert.ToDecimal(i),
                    ColumnName = $"ColumnName{i}",
                    ColumnReal = Convert.ToSingle(i),
                    ColumnSmallInt = Convert.ToInt16(i),
                    ColumnText = $"ColumnText{i}",
                    ColumnTimestampWithTimeZone = now,
                    ColumnTimestampWithoutTimeZone = now
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateCompleteTableAsDynamicProperties(dynamic table)
        {
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            table.ColumnBigInt = Convert.ToInt64(2);
            table.ColumnBigIntAsArray = new long[] { 1, 2, 3, 4, 5 };
            table.ColumnBigSerial = Convert.ToInt64(2);
            //table.ColumnBit = true;
            table.ColumnBoolean = (Randomizer.Next() % 2 != 0);
            table.ColumnChar = 'C';
            table.ColumnCharacter = "C";
            table.ColumnCharacterVarying = "ColumnCharacterVarying";
            table.ColumnDate = now.Date;
            table.ColumnDateAsArray = new[] { now.Date, now.Date, now.Date };
            table.ColumnInteger = Convert.ToInt32(2);
            table.ColumnIntegerAsArray = new[] { 1, 2, 3, 4, 5 };
            table.ColumnInterval = now.TimeOfDay;
            table.ColumnIntervalAsArray = new[] { now.TimeOfDay, now.TimeOfDay, now.TimeOfDay };
            //table.ColumnJson = "{\"field1\": 1, \"field2\": 2}";
            table.ColumnMoney = Convert.ToDecimal(2);
            table.ColumnName = $"{table.ColumnName} (Updated)";
            table.ColumnReal = Convert.ToSingle(2);
            table.ColumnSmallInt = Convert.ToInt16(2);
            table.ColumnText = $"{table.ColumnText} (Updated)";
            table.ColumnTimestampWithTimeZone = now;
            table.ColumnTimestampWithoutTimeZone = now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<ExpandoObject> CreateCompleteTablesAsExpandoObjects(int count)
        {
            var tables = new List<ExpandoObject>();
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            for (var i = 0; i < count; i++)
            {
                var item = new ExpandoObject() as IDictionary<string, object>;
                item["Id"] = (long)(i + 1);
                item["ColumnBigInt"] = Convert.ToInt64(i);
                item["ColumnBigIntAsArray"] = new long[] { 1, 2, 3, 4, 5 };
                item["ColumnBigSerial"] = Convert.ToInt64(i);
                //item["ColumnBit"] = true;
                item["ColumnBoolean"] = true;
                item["ColumnChar"] = 'C';
                item["ColumnCharacter"] = "C";
                item["ColumnCharacterVarying"] = "ColumnCharacterVarying";
                item["ColumnDate"] = now.Date;
                item["ColumnDateAsArray"] = new[] { now.Date, now.Date, now.Date };
                item["ColumnInteger"] = Convert.ToInt32(i);
                item["ColumnIntegerAsArray"] = new[] { 1, 2, 3, 4, 5 };
                item["ColumnInterval"] = now.TimeOfDay;
                item["ColumnIntervalAsArray"] = new[] { now.TimeOfDay, now.TimeOfDay, now.TimeOfDay };
                //item["ColumnJson"] = "{\"field1\": 1, \"field2\": 2}";
                item["ColumnMoney"] = Convert.ToDecimal(i);
                item["ColumnName"] = $"ColumnName{i}";
                item["ColumnReal"] = Convert.ToSingle(i);
                item["ColumnSmallInt"] = Convert.ToInt16(i);
                item["ColumnText"] = $"ColumnText{i}";
                item["ColumnTimestampWithTimeZone"] = now;
                item["ColumnTimestampWithoutTimeZone"] = now;
                tables.Add((ExpandoObject)item);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateCompleteTableAsExpandoObjectProperties(CompleteTable table)
        {
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            var item = table as IDictionary<string, object>;
            item["ColumnBigInt"] = Convert.ToInt64(2);
            item["ColumnBigIntAsArray"] = new long[] { 1, 2, 3, 4, 5 };
            item["ColumnBigSerial"] = Convert.ToInt64(2);
            //item["ColumnBit"] = true;
            item["ColumnBoolean"] = true;
            item["ColumnChar"] = 'C';
            item["ColumnCharacter"] = "C";
            item["ColumnCharacterVarying"] = "ColumnCharacterVarying";
            item["ColumnDate"] = now.Date;
            item["ColumnDateAsArray"] = new[] { now.Date, now.Date, now.Date };
            item["ColumnInteger"] = Convert.ToInt32(2);
            item["ColumnIntegerAsArray"] = new[] { 1, 2, 3, 4, 5 };
            item["ColumnInterval"] = now.TimeOfDay;
            item["ColumnIntervalAsArray"] = new[] { now.TimeOfDay, now.TimeOfDay, now.TimeOfDay };
            //item["ColumnJson"] = "{\"field1\": 1, \"field2\": 2}";
            item["ColumnMoney"] = Convert.ToDecimal(2);
            item["ColumnName"] = $"ColumnName-{Guid.NewGuid()}-Updated";
            item["ColumnReal"] = Convert.ToSingle(2);
            item["ColumnSmallInt"] = Convert.ToInt16(2);
            item["ColumnText"] = $"ColumnText-{Guid.NewGuid()}-Updated";
            item["ColumnTimestampWithTimeZone"] = now;
            item["ColumnTimestampWithoutTimeZone"] = now;
        }

        #endregion

        #region NonIdentityCompleteTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<NonIdentityCompleteTable> CreateNonIdentityCompleteTables(int count)
        {
            var tables = new List<NonIdentityCompleteTable>();
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            for (var i = 0; i < count; i++)
            {
                tables.Add(new NonIdentityCompleteTable
                {
                    Id = (i + 1),
                    ColumnBigInt = Convert.ToInt64(i),
                    ColumnBigIntAsArray = new long[] { 1, 2, 3, 4, 5 },
                    ColumnBigSerial = Convert.ToInt64(i),
                    //ColumnBit = true,
                    ColumnBoolean = true,
                    ColumnChar = 'C',
                    ColumnCharacter = "C",
                    ColumnCharacterVarying = "ColumnCharacterVarying",
                    ColumnDate = now.Date,
                    ColumnDateAsArray = new[] { now.Date, now.Date, now.Date },
                    ColumnInteger = Convert.ToInt32(i),
                    ColumnIntegerAsArray = new[] { 1, 2, 3, 4, 5 },
                    ColumnInterval = now.TimeOfDay,
                    ColumnIntervalAsArray = new[] { now.TimeOfDay, now.TimeOfDay, now.TimeOfDay },
                    //ColumnJson = "{\"field1\": 1, \"field2\": 2}",
                    ColumnMoney = Convert.ToDecimal(i),
                    ColumnName = $"ColumnName{i}",
                    ColumnReal = Convert.ToSingle(i),
                    ColumnSmallInt = Convert.ToInt16(i),
                    ColumnText = $"ColumnText{i}",
                    ColumnTimestampWithTimeZone = now,
                    ColumnTimestampWithoutTimeZone = now
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateNonIdentityCompleteTableProperties(NonIdentityCompleteTable table)
        {
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            table.ColumnBigInt = Convert.ToInt64(2);
            table.ColumnBigIntAsArray = new long[] { 1, 2, 3, 4, 5 };
            table.ColumnBigSerial = Convert.ToInt64(2);
            //table.ColumnBit = true;
            table.ColumnBoolean = (Randomizer.Next() % 2 != 0);
            table.ColumnChar = 'C';
            table.ColumnCharacter = "C";
            table.ColumnCharacterVarying = "ColumnCharacterVarying";
            table.ColumnDate = now.Date;
            table.ColumnDateAsArray = new[] { now.Date, now.Date, now.Date };
            table.ColumnInteger = Convert.ToInt32(2);
            table.ColumnIntegerAsArray = new[] { 1, 2, 3, 4, 5 };
            table.ColumnInterval = now.TimeOfDay;
            table.ColumnIntervalAsArray = new[] { now.TimeOfDay, now.TimeOfDay, now.TimeOfDay };
            //table.ColumnJson = "{\"field1\": 1, \"field2\": 2}";
            table.ColumnMoney = Convert.ToDecimal(2);
            table.ColumnName = $"{table.ColumnName}(Updated) - {Guid.NewGuid()}";
            table.ColumnReal = Convert.ToSingle(2);
            table.ColumnSmallInt = Convert.ToInt16(2);
            table.ColumnText = $"{table.ColumnText} (Updated)";
            table.ColumnTimestampWithTimeZone = now;
            table.ColumnTimestampWithoutTimeZone = now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<dynamic> CreateNonIdentityCompleteTablesAsDynamics(int count)
        {
            var tables = new List<dynamic>();
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            for (var i = 0; i < count; i++)
            {
                tables.Add(new
                {
                    Id = (long)(i + 1),
                    ColumnBigInt = Convert.ToInt64(i),
                    ColumnBigIntAsArray = new long[] { 1, 2, 3, 4, 5 },
                    ColumnBigSerial = Convert.ToInt64(i),
                    //ColumnBit = true,
                    ColumnBoolean = true,
                    ColumnChar = 'C',
                    ColumnCharacter = "C",
                    ColumnCharacterVarying = "ColumnCharacterVarying",
                    ColumnDate = now.Date,
                    ColumnDateAsArray = new[] { now.Date, now.Date, now.Date },
                    ColumnInteger = Convert.ToInt32(i),
                    ColumnIntegerAsArray = new[] { 1, 2, 3, 4, 5 },
                    ColumnInterval = now.TimeOfDay,
                    ColumnIntervalAsArray = new[] { now.TimeOfDay, now.TimeOfDay, now.TimeOfDay },
                    //ColumnJson = "{\"field1\": 1, \"field2\": 2}",
                    ColumnMoney = Convert.ToDecimal(i),
                    ColumnName = $"ColumnName{i}",
                    ColumnReal = Convert.ToSingle(i),
                    ColumnSmallInt = Convert.ToInt16(i),
                    ColumnText = $"ColumnText{i}",
                    ColumnTimestampWithTimeZone = now,
                    ColumnTimestampWithoutTimeZone = now
                });
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateNonIdentityCompleteTableAsDynamicProperties(dynamic table)
        {
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            table.ColumnBigInt = Convert.ToInt64(2);
            table.ColumnBigIntAsArray = new long[] { 1, 2, 3, 4, 5 };
            table.ColumnBigSerial = Convert.ToInt64(2);
            //table.ColumnBit = true;
            table.ColumnBoolean = (Randomizer.Next() % 2 != 0);
            table.ColumnChar = 'C';
            table.ColumnCharacter = "C";
            table.ColumnCharacterVarying = "ColumnCharacterVarying";
            table.ColumnDate = now.Date;
            table.ColumnDateAsArray = new[] { now.Date, now.Date, now.Date };
            table.ColumnInteger = Convert.ToInt32(2);
            table.ColumnIntegerAsArray = new[] { 1, 2, 3, 4, 5 };
            table.ColumnInterval = now.TimeOfDay;
            table.ColumnIntervalAsArray = new[] { now.TimeOfDay, now.TimeOfDay, now.TimeOfDay };
            //table.ColumnJson = "{\"field1\": 1, \"field2\": 2}";
            table.ColumnMoney = Convert.ToDecimal(2);
            table.ColumnName = $"{table.ColumnName} (Updated)";
            table.ColumnReal = Convert.ToSingle(2);
            table.ColumnSmallInt = Convert.ToInt16(2);
            table.ColumnText = $"{table.ColumnText} (Updated)";
            table.ColumnTimestampWithTimeZone = now;
            table.ColumnTimestampWithoutTimeZone = now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<ExpandoObject> CreateNonIdentityCompleteTablesAsExpandoObjects(int count)
        {
            var tables = new List<ExpandoObject>();
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            for (var i = 0; i < count; i++)
            {
                var item = new ExpandoObject() as IDictionary<string, object>;
                item["Id"] = (i + 1);
                item["ColumnBigInt"] = Convert.ToInt64(i);
                item["ColumnBigIntAsArray"] = new long[] { 1, 2, 3, 4, 5 };
                item["ColumnBigSerial"] = Convert.ToInt64(i);
                //item["ColumnBit"] = true;
                item["ColumnBoolean"] = true;
                item["ColumnChar"] = 'C';
                item["ColumnCharacter"] = "C";
                item["ColumnCharacterVarying"] = "ColumnCharacterVarying";
                item["ColumnDate"] = now.Date;
                item["ColumnDateAsArray"] = new[] { now.Date, now.Date, now.Date };
                item["ColumnInteger"] = Convert.ToInt32(i);
                item["ColumnIntegerAsArray"] = new[] { 1, 2, 3, 4, 5 };
                item["ColumnInterval"] = now.TimeOfDay;
                item["ColumnIntervalAsArray"] = new[] { now.TimeOfDay, now.TimeOfDay, now.TimeOfDay };
                //item["ColumnJson"] = "{\"field1\": 1, \"field2\": 2}";
                item["ColumnMoney"] = Convert.ToDecimal(i);
                item["ColumnName"] = $"ColumnName{i}";
                item["ColumnReal"] = Convert.ToSingle(i);
                item["ColumnSmallInt"] = Convert.ToInt16(i);
                item["ColumnText"] = $"ColumnText{i}";
                item["ColumnTimestampWithTimeZone"] = now;
                item["ColumnTimestampWithoutTimeZone"] = now;
                tables.Add((ExpandoObject)item);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateNonIdentityCompleteTableAsExpandoObjectProperties(CompleteTable table)
        {
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            var item = table as IDictionary<string, object>;
            item["ColumnBigInt"] = Convert.ToInt64(2);
            item["ColumnBigIntAsArray"] = new long[] { 1, 2, 3, 4, 5 };
            item["ColumnBigSerial"] = Convert.ToInt64(2);
            //item["ColumnBit"] = true;
            item["ColumnBoolean"] = true;
            item["ColumnChar"] = 'C';
            item["ColumnCharacter"] = "C";
            item["ColumnCharacterVarying"] = "ColumnCharacterVarying";
            item["ColumnDate"] = now.Date;
            item["ColumnDateAsArray"] = new[] { now.Date, now.Date, now.Date };
            item["ColumnInteger"] = Convert.ToInt32(2);
            item["ColumnIntegerAsArray"] = new[] { 1, 2, 3, 4, 5 };
            item["ColumnInterval"] = now.TimeOfDay;
            item["ColumnIntervalAsArray"] = new[] { now.TimeOfDay, now.TimeOfDay, now.TimeOfDay };
            //item["ColumnJson"] = "{\"field1\": 1, \"field2\": 2}";
            item["ColumnMoney"] = Convert.ToDecimal(2);
            item["ColumnName"] = $"ColumnName-{Guid.NewGuid()}-Updated";
            item["ColumnReal"] = Convert.ToSingle(2);
            item["ColumnSmallInt"] = Convert.ToInt16(2);
            item["ColumnText"] = $"ColumnText-{Guid.NewGuid()}-Updated";
            item["ColumnTimestampWithTimeZone"] = now;
            item["ColumnTimestampWithoutTimeZone"] = now;
        }

        #endregion
    }
}
