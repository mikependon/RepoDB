using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.MySqlConnector.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace RepoDb.MySqlConnector.IntegrationTests
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

        #region Helpers

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
                if (value1 is byte[] b1 && value2 is byte[] b2)
                {
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
                    if (value1 is byte[] b1 && value2 is byte[] b2)
                    {
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
                    ColumnVarchar = $"ColumnVarChar:{i}",
                    ColumnInt = i,
                    ColumnDecimal2 = Convert.ToDecimal(i),
                    ColumnDateTime = EpocDate,
                    ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    ColumnBlobAsArray = Encoding.Default.GetBytes($"ColumnBlobAsArray:{i}"),
                    ColumnBinary = Encoding.Default.GetBytes($"ColumnBinary:{i}"),
                    ColumnLongBlob = Encoding.Default.GetBytes($"ColumnLongBlob:{i}"),
                    ColumnMediumBlob = Encoding.Default.GetBytes($"ColumnMediumBlob:{i}"),
                    ColumnTinyBlob = Encoding.Default.GetBytes($"ColumnTinyBlob:{i}"),
                    ColumnVarBinary = Encoding.Default.GetBytes($"ColumnVarBinary:{i}"),
                    ColumnDate = EpocDate,
                    ColumnDateTime2 = now,
                    ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay,
                    ColumnTimeStamp = now,
                    ColumnYear = Convert.ToInt16(now.Year),
                    //ColumnGeometry = Encoding.Default.GetBytes($"POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))"),
                    //ColumnLineString = Encoding.Default.GetBytes($"LINESTRING (-122.36 47.656, -122.343 47.656)"),
                    //ColumnMultiLineString = Encoding.Default.GetBytes($"ColumnMultiLineString:{i}"),
                    //ColumnMultiPoint = Encoding.Default.GetBytes($"ColumnMultiPoint:{i}"),
                    //ColumnMultiPolygon = Encoding.Default.GetBytes($"ColumnMultiPolygon:{i}"),
                    //ColumnPoint = Encoding.Default.GetBytes($"ColumnPoint:{i}"),
                    //ColumnPolygon = Encoding.Default.GetBytes($"ColumnPolygon:{i}"),
                    ColumnBigint = Convert.ToInt64(i),
                    ColumnDecimal = Convert.ToDecimal(i),
                    ColumnDouble = Convert.ToDouble(i),
                    ColumnFloat = Convert.ToSingle(i),
                    ColumnInt2 = i,
                    ColumnMediumInt = i,
                    ColumnReal = Convert.ToDouble(i),
                    ColumnSmallInt = Convert.ToInt16(i),
                    ColumnTinyInt = (SByte)i,
                    ColumnChar = "C",
                    ColumnJson = "{\"Field1\": \"Value1\", \"Field2\": \"Value2\"}",
                    ColumnNChar = "C",
                    ColumnNVarChar = $"ColumnNVarChar:{i}",
                    ColumnLongText = $"ColumnLongText:{i}",
                    ColumnMediumText = $"ColumnMediumText:{i}",
                    ColumnText = $"ColumText:{i}",
                    ColumnTinyText = $"ColumnTinyText:{i}",
                    ColumnBit = (UInt64)1
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
            table.ColumnVarchar = $"ColumnVarChar:{1}-Updated";
            table.ColumnInt = 1;
            table.ColumnDecimal2 = Convert.ToDecimal(1);
            table.ColumnDateTime = EpocDate;
            table.ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{1}-Updated");
            table.ColumnBlobAsArray = Encoding.Default.GetBytes($"ColumnBlobAsArray:{1}-Updated");
            table.ColumnBinary = Encoding.Default.GetBytes($"ColumnBinary:{1}-Updated");
            table.ColumnLongBlob = Encoding.Default.GetBytes($"ColumnLongBlob:{1}-Updated");
            table.ColumnMediumBlob = Encoding.Default.GetBytes($"ColumnMediumBlob:{1}-Updated");
            table.ColumnTinyBlob = Encoding.Default.GetBytes($"ColumnTinyBlob:{1}-Updated");
            table.ColumnVarBinary = Encoding.Default.GetBytes($"ColumnVarBinary:{1}-Updated");
            table.ColumnDate = EpocDate;
            table.ColumnDateTime2 = now;
            table.ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay;
            table.ColumnTimeStamp = now;
            table.ColumnYear = Convert.ToInt16(now.Year);
            //table.ColumnGeometry = Encoding.Default.GetBytes($"ColumnGeometry:{1}");
            //table.ColumnLineString = Encoding.Default.GetBytes($"ColumnLineString:{1}");
            //table.ColumnMultiLineString = Encoding.Default.GetBytes($"ColumnMultiLineString:{1}");
            //table.ColumnMultiPoint = Encoding.Default.GetBytes($"ColumnMultiPoint:{1}");
            //table.ColumnMultiPolygon = Encoding.Default.GetBytes($"ColumnMultiPolygon:{1}");
            //table.ColumnPoint = Encoding.Default.GetBytes($"ColumnPoint:{1}");
            //table.ColumnPolygon = Encoding.Default.GetBytes($"ColumnPolygon:{1}");
            table.ColumnBigint = Convert.ToInt64(1);
            table.ColumnDecimal = Convert.ToDecimal(1);
            table.ColumnDouble = Convert.ToDouble(1);
            table.ColumnFloat = Convert.ToSingle(1);
            table.ColumnInt2 = 1;
            table.ColumnMediumInt = 1;
            table.ColumnReal = Convert.ToDouble(1);
            table.ColumnSmallInt = Convert.ToInt16(1);
            table.ColumnTinyInt = (SByte)1;
            table.ColumnChar = "C";
            table.ColumnJson = "{\"Field\": \"Value-Updated\"}";
            table.ColumnNChar = "C";
            table.ColumnNVarChar = $"ColumnNVarChar:{1}-Updated";
            table.ColumnLongText = $"ColumnLongText:{1}-Updated";
            table.ColumnMediumText = $"ColumnMediumText:{1}-Updated";
            table.ColumnText = $"ColumText:{1}-Updated";
            table.ColumnTinyText = $"ColumnTinyText:{1}-Updated";
            table.ColumnBit = (UInt64)1;
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
                    ColumnVarchar = $"ColumnVarChar:{i}",
                    ColumnInt = i,
                    ColumnDecimal2 = Convert.ToDecimal(i),
                    ColumnDateTime = EpocDate,
                    ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    ColumnBlobAsArray = Encoding.Default.GetBytes($"ColumnBlobAsArray:{i}"),
                    ColumnBinary = Encoding.Default.GetBytes($"ColumnBinary:{i}"),
                    ColumnLongBlob = Encoding.Default.GetBytes($"ColumnLongBlob:{i}"),
                    ColumnMediumBlob = Encoding.Default.GetBytes($"ColumnMediumBlob:{i}"),
                    ColumnTinyBlob = Encoding.Default.GetBytes($"ColumnTinyBlob:{i}"),
                    ColumnVarBinary = Encoding.Default.GetBytes($"ColumnVarBinary:{i}"),
                    ColumnDate = EpocDate,
                    ColumnDateTime2 = now,
                    ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay,
                    ColumnTimeStamp = now,
                    ColumnYear = Convert.ToInt16(now.Year),
                    //ColumnGeometry = Encoding.Default.GetBytes($"POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))"),
                    //ColumnLineString = Encoding.Default.GetBytes($"LINESTRING (-122.36 47.656, -122.343 47.656)"),
                    //ColumnMultiLineString = Encoding.Default.GetBytes($"ColumnMultiLineString:{i}"),
                    //ColumnMultiPoint = Encoding.Default.GetBytes($"ColumnMultiPoint:{i}"),
                    //ColumnMultiPolygon = Encoding.Default.GetBytes($"ColumnMultiPolygon:{i}"),
                    //ColumnPoint = Encoding.Default.GetBytes($"ColumnPoint:{i}"),
                    //ColumnPolygon = Encoding.Default.GetBytes($"ColumnPolygon:{i}"),
                    ColumnBigint = Convert.ToInt64(i),
                    ColumnDecimal = Convert.ToDecimal(i),
                    ColumnDouble = Convert.ToDouble(i),
                    ColumnFloat = Convert.ToSingle(i),
                    ColumnInt2 = i,
                    ColumnMediumInt = i,
                    ColumnReal = Convert.ToDouble(i),
                    ColumnSmallInt = Convert.ToInt16(i),
                    ColumnTinyInt = (SByte)i,
                    ColumnChar = "C",
                    ColumnJson = "{\"Field1\": \"Value1\", \"Field2\": \"Value2\"}",
                    ColumnNChar = "C",
                    ColumnNVarChar = $"ColumnNVarChar:{i}",
                    ColumnLongText = $"ColumnLongText:{i}",
                    ColumnMediumText = $"ColumnMediumText:{i}",
                    ColumnText = $"ColumText:{i}",
                    ColumnTinyText = $"ColumnTinyText:{i}",
                    ColumnBit = (UInt64)1
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
            table.ColumnVarchar = $"ColumnVarChar:{1}";
            table.ColumnInt = 1;
            table.ColumnDecimal2 = Convert.ToDecimal(1);
            table.ColumnDateTime = EpocDate;
            table.ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{1}");
            table.ColumnBlobAsArray = Encoding.Default.GetBytes($"ColumnBlobAsArray:{1}");
            table.ColumnBinary = Encoding.Default.GetBytes($"ColumnBinary:{1}");
            table.ColumnLongBlob = Encoding.Default.GetBytes($"ColumnLongBlob:{1}");
            table.ColumnMediumBlob = Encoding.Default.GetBytes($"ColumnMediumBlob:{1}");
            table.ColumnTinyBlob = Encoding.Default.GetBytes($"ColumnTinyBlob:{1}");
            table.ColumnVarBinary = Encoding.Default.GetBytes($"ColumnVarBinary:{1}");
            table.ColumnDate = EpocDate;
            table.ColumnDateTime2 = now;
            table.ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay;
            table.ColumnTimeStamp = now;
            table.ColumnYear = Convert.ToInt16(now.Year);
            //table.ColumnGeometry = Encoding.Default.GetBytes($"ColumnGeometry:{1}");
            //table.ColumnLineString = Encoding.Default.GetBytes($"ColumnLineString:{1}");
            //table.ColumnMultiLineString = Encoding.Default.GetBytes($"ColumnMultiLineString:{1}");
            //table.ColumnMultiPoint = Encoding.Default.GetBytes($"ColumnMultiPoint:{1}");
            //table.ColumnMultiPolygon = Encoding.Default.GetBytes($"ColumnMultiPolygon:{1}");
            //table.ColumnPoint = Encoding.Default.GetBytes($"ColumnPoint:{1}");
            //table.ColumnPolygon = Encoding.Default.GetBytes($"ColumnPolygon:{1}");
            table.ColumnBigint = Convert.ToInt64(1);
            table.ColumnDecimal = Convert.ToDecimal(1);
            table.ColumnDouble = Convert.ToDouble(1);
            table.ColumnFloat = Convert.ToSingle(1);
            table.ColumnInt2 = 1;
            table.ColumnMediumInt = 1;
            table.ColumnReal = Convert.ToDouble(1);
            table.ColumnSmallInt = Convert.ToInt16(1);
            table.ColumnTinyInt = (SByte)1;
            table.ColumnChar = "C";
            table.ColumnJson = "{ \"Field\" : \"Value\" }";
            table.ColumnNChar = "C";
            table.ColumnNVarChar = $"ColumnNVarChar:{1}";
            table.ColumnLongText = $"ColumnLongText:{1}";
            table.ColumnMediumText = $"ColumnMediumText:{1}";
            table.ColumnText = $"ColumText:{1}";
            table.ColumnTinyText = $"ColumnTinyText:{1}";
            table.ColumnBit = (UInt64)1;
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
                item["ColumnVarchar"] = $"ColumnVarChar:{i}";
                item["ColumnInt"] = i;
                item["ColumnDecimal2"] = Convert.ToDecimal(i);
                item["ColumnDateTime"] = EpocDate;
                item["ColumnBlob"] = Encoding.Default.GetBytes($"ColumnBlob:{i}");
                item["ColumnBlobAsArray"] = Encoding.Default.GetBytes($"ColumnBlobAsArray:{i}");
                item["ColumnBinary"] = Encoding.Default.GetBytes($"ColumnBinary:{i}");
                item["ColumnLongBlob"] = Encoding.Default.GetBytes($"ColumnLongBlob:{i}");
                item["ColumnMediumBlob"] = Encoding.Default.GetBytes($"ColumnMediumBlob:{i}");
                item["ColumnTinyBlob"] = Encoding.Default.GetBytes($"ColumnTinyBlob:{i}");
                item["ColumnVarBinary"] = Encoding.Default.GetBytes($"ColumnVarBinary:{i}");
                item["ColumnDate"] = EpocDate;
                item["ColumnDateTime2"] = now;
                item["ColumnTime"] = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay;
                item["ColumnTimeStamp"] = now;
                item["ColumnYear"] = Convert.ToInt16(now.Year);
                //item["ColumnGeometry"] = Encoding.Default.GetBytes($"POLYGON ((0 0; 50 0; 50 50; 0 50; 0 0))");
                //item["ColumnLineString"] = Encoding.Default.GetBytes($"LINESTRING (-122.36 47.656; -122.343 47.656)");
                //item["ColumnMultiLineString"] = Encoding.Default.GetBytes($"ColumnMultiLineString:{i}");
                //item["ColumnMultiPoint"] = Encoding.Default.GetBytes($"ColumnMultiPoint:{i}");
                //item["ColumnMultiPolygon"] = Encoding.Default.GetBytes($"ColumnMultiPolygon:{i}");
                //item["ColumnPoint"] = Encoding.Default.GetBytes($"ColumnPoint:{i}");
                //item["ColumnPolygon"] = Encoding.Default.GetBytes($"ColumnPolygon:{i}");
                item["ColumnBigint"] = Convert.ToInt64(i);
                item["ColumnDecimal"] = Convert.ToDecimal(i);
                item["ColumnDouble"] = Convert.ToDouble(i);
                item["ColumnFloat"] = Convert.ToSingle(i);
                item["ColumnInt2"] = i;
                item["ColumnMediumInt"] = i;
                item["ColumnReal"] = Convert.ToDouble(i);
                item["ColumnSmallInt"] = Convert.ToInt16(i);
                item["ColumnTinyInt"] = (SByte)i;
                item["ColumnChar"] = "C";
                item["ColumnJson"] = "{\"Field1\": \"Value1\", \"Field2\": \"Value2\"}";
                item["ColumnNChar"] = "C";
                item["ColumnNVarChar"] = $"ColumnNVarChar:{i}";
                item["ColumnLongText"] = $"ColumnLongText:{i}";
                item["ColumnMediumText"] = $"ColumnMediumText:{i}";
                item["ColumnText"] = $"ColumText:{i}";
                item["ColumnTinyText"] = $"ColumnTinyText:{i}";
                item["ColumnBit"] = (UInt64)1;
                tables.Add((ExpandoObject)item);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateCompleteTableAsExpandoObjectProperties(ExpandoObject table)
        {
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            var item = table as IDictionary<string, object>;
            item["ColumnVarchar"] = $"ColumnVarChar:{2}";
            item["ColumnInt"] = 2;
            item["ColumnDecimal2"] = Convert.ToDecimal(2);
            item["ColumnDateTime"] = EpocDate;
            item["ColumnBlob"] = Encoding.Default.GetBytes($"ColumnBlob:{2}");
            item["ColumnBlobAsArray"] = Encoding.Default.GetBytes($"ColumnBlobAsArray:{2}");
            item["ColumnBinary"] = Encoding.Default.GetBytes($"ColumnBinary:{2}");
            item["ColumnLongBlob"] = Encoding.Default.GetBytes($"ColumnLongBlob:{2}");
            item["ColumnMediumBlob"] = Encoding.Default.GetBytes($"ColumnMediumBlob:{2}");
            item["ColumnTinyBlob"] = Encoding.Default.GetBytes($"ColumnTinyBlob:{2}");
            item["ColumnVarBinary"] = Encoding.Default.GetBytes($"ColumnVarBinary:{2}");
            item["ColumnDate"] = EpocDate;
            item["ColumnDateTime2"] = now;
            item["ColumnTime"] = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay;
            item["ColumnTimeStamp"] = now;
            item["ColumnYear"] = Convert.ToInt16(now.Year);
            //item["ColumnGeometry"] = Encoding.Default.GetBytes($"POLYGON ((0 0; 50 0; 50 50; 0 50; 0 0))");
            //item["ColumnLineString"] = Encoding.Default.GetBytes($"LINESTRING (-122.36 47.656; -122.343 47.656)");
            //item["ColumnMultiLineString"] = Encoding.Default.GetBytes($"ColumnMultiLineString:{2}");
            //item["ColumnMultiPoint"] = Encoding.Default.GetBytes($"ColumnMultiPoint:{2}");
            //item["ColumnMultiPolygon"] = Encoding.Default.GetBytes($"ColumnMultiPolygon:{2}");
            //item["ColumnPoint"] = Encoding.Default.GetBytes($"ColumnPoint:{2}");
            //item["ColumnPolygon"] = Encoding.Default.GetBytes($"ColumnPolygon:{2}");
            item["ColumnBigint"] = Convert.ToInt64(2);
            item["ColumnDecimal"] = Convert.ToDecimal(2);
            item["ColumnDouble"] = Convert.ToDouble(2);
            item["ColumnFloat"] = Convert.ToSingle(2);
            item["ColumnInt2"] = 2;
            item["ColumnMediumInt"] = 2;
            item["ColumnReal"] = Convert.ToDouble(2);
            item["ColumnSmallInt"] = Convert.ToInt16(2);
            item["ColumnTinyInt"] = (SByte)2;
            item["ColumnChar"] = "C";
            item["ColumnJson"] = "{\"Field1\": \"Value1\", \"Field2\": \"Value2\"}";
            item["ColumnNChar"] = "C";
            item["ColumnNVarChar"] = $"ColumnNVarChar:{2}";
            item["ColumnLongText"] = $"ColumnLongText:{2}";
            item["ColumnMediumText"] = $"ColumnMediumText:{2}";
            item["ColumnText"] = $"ColumText:{2}";
            item["ColumnTinyText"] = $"ColumnTinyText:{2}";
            item["ColumnBit"] = (UInt64)1;
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
                    ColumnVarchar = $"ColumnVarChar:{i}",
                    ColumnInt = i,
                    ColumnDecimal2 = Convert.ToDecimal(i),
                    ColumnDateTime = EpocDate,
                    ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    ColumnBlobAsArray = Encoding.Default.GetBytes($"ColumnBlobAsArray:{i}"),
                    ColumnBinary = Encoding.Default.GetBytes($"ColumnBinary:{i}"),
                    ColumnLongBlob = Encoding.Default.GetBytes($"ColumnLongBlob:{i}"),
                    ColumnMediumBlob = Encoding.Default.GetBytes($"ColumnMediumBlob:{i}"),
                    ColumnTinyBlob = Encoding.Default.GetBytes($"ColumnTinyBlob:{i}"),
                    ColumnVarBinary = Encoding.Default.GetBytes($"ColumnVarBinary:{i}"),
                    ColumnDate = EpocDate,
                    ColumnDateTime2 = now,
                    ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay,
                    ColumnTimeStamp = now,
                    ColumnYear = Convert.ToInt16(now.Year),
                    //ColumnGeometry = Encoding.Default.GetBytes($"POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))"),
                    //ColumnLineString = Encoding.Default.GetBytes($"LINESTRING (-122.36 47.656, -122.343 47.656)"),
                    //ColumnMultiLineString = Encoding.Default.GetBytes($"ColumnMultiLineString:{i}"),
                    //ColumnMultiPoint = Encoding.Default.GetBytes($"ColumnMultiPoint:{i}"),
                    //ColumnMultiPolygon = Encoding.Default.GetBytes($"ColumnMultiPolygon:{i}"),
                    //ColumnPoint = Encoding.Default.GetBytes($"ColumnPoint:{i}"),
                    //ColumnPolygon = Encoding.Default.GetBytes($"ColumnPolygon:{i}"),
                    ColumnBigint = Convert.ToInt64(i),
                    ColumnDecimal = Convert.ToDecimal(i),
                    ColumnDouble = Convert.ToDouble(i),
                    ColumnFloat = Convert.ToSingle(i),
                    ColumnInt2 = i,
                    ColumnMediumInt = i,
                    ColumnReal = Convert.ToDouble(i),
                    ColumnSmallInt = Convert.ToInt16(i),
                    ColumnTinyInt = (SByte)i,
                    ColumnChar = "C",
                    ColumnJson = "{\"Field1\": \"Value1\", \"Field2\": \"Value2\"}",
                    ColumnNChar = "C",
                    ColumnNVarChar = $"ColumnNVarChar:{i}",
                    ColumnLongText = $"ColumnLongText:{i}",
                    ColumnMediumText = $"ColumnMediumText:{i}",
                    ColumnText = $"ColumText:{i}",
                    ColumnTinyText = $"ColumnTinyText:{i}",
                    ColumnBit = (UInt64)1
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
            table.ColumnVarchar = $"ColumnVarChar:{1}";
            table.ColumnInt = 1;
            table.ColumnDecimal2 = Convert.ToDecimal(1);
            table.ColumnDateTime = EpocDate;
            table.ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{1}");
            table.ColumnBlobAsArray = Encoding.Default.GetBytes($"ColumnBlobAsArray:{1}");
            table.ColumnBinary = Encoding.Default.GetBytes($"ColumnBinary:{1}");
            table.ColumnLongBlob = Encoding.Default.GetBytes($"ColumnLongBlob:{1}");
            table.ColumnMediumBlob = Encoding.Default.GetBytes($"ColumnMediumBlob:{1}");
            table.ColumnTinyBlob = Encoding.Default.GetBytes($"ColumnTinyBlob:{1}");
            table.ColumnVarBinary = Encoding.Default.GetBytes($"ColumnVarBinary:{1}");
            table.ColumnDate = EpocDate;
            table.ColumnDateTime2 = now;
            table.ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay;
            table.ColumnTimeStamp = now;
            table.ColumnYear = Convert.ToInt16(now.Year);
            //table.ColumnGeometry = Encoding.Default.GetBytes($"ColumnGeometry:{1}");
            //table.ColumnLineString = Encoding.Default.GetBytes($"ColumnLineString:{1}");
            //table.ColumnMultiLineString = Encoding.Default.GetBytes($"ColumnMultiLineString:{1}");
            //table.ColumnMultiPoint = Encoding.Default.GetBytes($"ColumnMultiPoint:{1}");
            //table.ColumnMultiPolygon = Encoding.Default.GetBytes($"ColumnMultiPolygon:{1}");
            //table.ColumnPoint = Encoding.Default.GetBytes($"ColumnPoint:{1}");
            //table.ColumnPolygon = Encoding.Default.GetBytes($"ColumnPolygon:{1}");
            table.ColumnBigint = Convert.ToInt64(1);
            table.ColumnDecimal = Convert.ToDecimal(1);
            table.ColumnDouble = Convert.ToDouble(1);
            table.ColumnFloat = Convert.ToSingle(1);
            table.ColumnInt2 = 1;
            table.ColumnMediumInt = 1;
            table.ColumnReal = Convert.ToDouble(1);
            table.ColumnSmallInt = Convert.ToInt16(1);
            table.ColumnTinyInt = (SByte)1;
            table.ColumnChar = "C";
            table.ColumnJson = "{\"Field\": \"Value\"}";
            table.ColumnNChar = "C";
            table.ColumnNVarChar = $"ColumnNVarChar:{1}";
            table.ColumnLongText = $"ColumnLongText:{1}";
            table.ColumnMediumText = $"ColumnMediumText:{1}";
            table.ColumnText = $"ColumText:{1}";
            table.ColumnTinyText = $"ColumnTinyText:{1}";
            table.ColumnBit = (UInt64)1;
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
                    ColumnVarchar = $"ColumnVarChar:{i}",
                    ColumnInt = i,
                    ColumnDecimal2 = Convert.ToDecimal(i),
                    ColumnDateTime = EpocDate,
                    ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    ColumnBlobAsArray = Encoding.Default.GetBytes($"ColumnBlobAsArray:{i}"),
                    ColumnBinary = Encoding.Default.GetBytes($"ColumnBinary:{i}"),
                    ColumnLongBlob = Encoding.Default.GetBytes($"ColumnLongBlob:{i}"),
                    ColumnMediumBlob = Encoding.Default.GetBytes($"ColumnMediumBlob:{i}"),
                    ColumnTinyBlob = Encoding.Default.GetBytes($"ColumnTinyBlob:{i}"),
                    ColumnVarBinary = Encoding.Default.GetBytes($"ColumnVarBinary:{i}"),
                    ColumnDate = EpocDate,
                    ColumnDateTime2 = now,
                    ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay,
                    ColumnTimeStamp = now,
                    ColumnYear = Convert.ToInt16(now.Year),
                    //ColumnGeometry = Encoding.Default.GetBytes($"POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))"),
                    //ColumnLineString = Encoding.Default.GetBytes($"LINESTRING (-122.36 47.656, -122.343 47.656)"),
                    //ColumnMultiLineString = Encoding.Default.GetBytes($"ColumnMultiLineString:{i}"),
                    //ColumnMultiPoint = Encoding.Default.GetBytes($"ColumnMultiPoint:{i}"),
                    //ColumnMultiPolygon = Encoding.Default.GetBytes($"ColumnMultiPolygon:{i}"),
                    //ColumnPoint = Encoding.Default.GetBytes($"ColumnPoint:{i}"),
                    //ColumnPolygon = Encoding.Default.GetBytes($"ColumnPolygon:{i}"),
                    ColumnBigint = Convert.ToInt64(i),
                    ColumnDecimal = Convert.ToDecimal(i),
                    ColumnDouble = Convert.ToDouble(i),
                    ColumnFloat = Convert.ToSingle(i),
                    ColumnInt2 = i,
                    ColumnMediumInt = i,
                    ColumnReal = Convert.ToDouble(i),
                    ColumnSmallInt = Convert.ToInt16(i),
                    ColumnTinyInt = (SByte)i,
                    ColumnChar = "C",
                    ColumnJson = "{\"Field1\": \"Value1\", \"Field2\": \"Value2\"}",
                    ColumnNChar = "C",
                    ColumnNVarChar = $"ColumnNVarChar:{i}",
                    ColumnLongText = $"ColumnLongText:{i}",
                    ColumnMediumText = $"ColumnMediumText:{i}",
                    ColumnText = $"ColumText:{i}",
                    ColumnTinyText = $"ColumnTinyText:{i}",
                    ColumnBit = (UInt64)1
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
            table.ColumnVarchar = $"ColumnVarChar:{1}";
            table.ColumnInt = 1;
            table.ColumnDecimal2 = Convert.ToDecimal(1);
            table.ColumnDateTime = EpocDate;
            table.ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{1}");
            table.ColumnBlobAsArray = Encoding.Default.GetBytes($"ColumnBlobAsArray:{1}");
            table.ColumnBinary = Encoding.Default.GetBytes($"ColumnBinary:{1}");
            table.ColumnLongBlob = Encoding.Default.GetBytes($"ColumnLongBlob:{1}");
            table.ColumnMediumBlob = Encoding.Default.GetBytes($"ColumnMediumBlob:{1}");
            table.ColumnTinyBlob = Encoding.Default.GetBytes($"ColumnTinyBlob:{1}");
            table.ColumnVarBinary = Encoding.Default.GetBytes($"ColumnVarBinary:{1}");
            table.ColumnDate = EpocDate;
            table.ColumnDateTime2 = now;
            table.ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay;
            table.ColumnTimeStamp = now;
            table.ColumnYear = Convert.ToInt16(now.Year);
            //table.ColumnGeometry = Encoding.Default.GetBytes($"ColumnGeometry:{1}");
            //table.ColumnLineString = Encoding.Default.GetBytes($"ColumnLineString:{1}");
            //table.ColumnMultiLineString = Encoding.Default.GetBytes($"ColumnMultiLineString:{1}");
            //table.ColumnMultiPoint = Encoding.Default.GetBytes($"ColumnMultiPoint:{1}");
            //table.ColumnMultiPolygon = Encoding.Default.GetBytes($"ColumnMultiPolygon:{1}");
            //table.ColumnPoint = Encoding.Default.GetBytes($"ColumnPoint:{1}");
            //table.ColumnPolygon = Encoding.Default.GetBytes($"ColumnPolygon:{1}");
            table.ColumnBigint = Convert.ToInt64(1);
            table.ColumnDecimal = Convert.ToDecimal(1);
            table.ColumnDouble = Convert.ToDouble(1);
            table.ColumnFloat = Convert.ToSingle(1);
            table.ColumnInt2 = 1;
            table.ColumnMediumInt = 1;
            table.ColumnReal = Convert.ToDouble(1);
            table.ColumnSmallInt = Convert.ToInt16(1);
            table.ColumnTinyInt = (SByte)1;
            table.ColumnChar = "C";
            table.ColumnJson = "{ \"Field\" : \"Value\" }";
            table.ColumnNChar = "C";
            table.ColumnNVarChar = $"ColumnNVarChar:{1}";
            table.ColumnLongText = $"ColumnLongText:{1}";
            table.ColumnMediumText = $"ColumnMediumText:{1}";
            table.ColumnText = $"ColumText:{1}";
            table.ColumnTinyText = $"ColumnTinyText:{1}";
            table.ColumnBit = (UInt64)1;
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
                item["Id"] = (long)(i + 1);
                item["ColumnVarchar"] = $"ColumnVarChar:{i}";
                item["ColumnInt"] = i;
                item["ColumnDecimal2"] = Convert.ToDecimal(i);
                item["ColumnDateTime"] = EpocDate;
                item["ColumnBlob"] = Encoding.Default.GetBytes($"ColumnBlob:{i}");
                item["ColumnBlobAsArray"] = Encoding.Default.GetBytes($"ColumnBlobAsArray:{i}");
                item["ColumnBinary"] = Encoding.Default.GetBytes($"ColumnBinary:{i}");
                item["ColumnLongBlob"] = Encoding.Default.GetBytes($"ColumnLongBlob:{i}");
                item["ColumnMediumBlob"] = Encoding.Default.GetBytes($"ColumnMediumBlob:{i}");
                item["ColumnTinyBlob"] = Encoding.Default.GetBytes($"ColumnTinyBlob:{i}");
                item["ColumnVarBinary"] = Encoding.Default.GetBytes($"ColumnVarBinary:{i}");
                item["ColumnDate"] = EpocDate;
                item["ColumnDateTime2"] = now;
                item["ColumnTime"] = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay;
                item["ColumnTimeStamp"] = now;
                item["ColumnYear"] = Convert.ToInt16(now.Year);
                //item["ColumnGeometry"] = Encoding.Default.GetBytes($"POLYGON ((0 0; 50 0; 50 50; 0 50; 0 0))");
                //item["ColumnLineString"] = Encoding.Default.GetBytes($"LINESTRING (-122.36 47.656; -122.343 47.656)");
                //item["ColumnMultiLineString"] = Encoding.Default.GetBytes($"ColumnMultiLineString:{i}");
                //item["ColumnMultiPoint"] = Encoding.Default.GetBytes($"ColumnMultiPoint:{i}");
                //item["ColumnMultiPolygon"] = Encoding.Default.GetBytes($"ColumnMultiPolygon:{i}");
                //item["ColumnPoint"] = Encoding.Default.GetBytes($"ColumnPoint:{i}");
                //item["ColumnPolygon"] = Encoding.Default.GetBytes($"ColumnPolygon:{i}");
                item["ColumnBigint"] = Convert.ToInt64(i);
                item["ColumnDecimal"] = Convert.ToDecimal(i);
                item["ColumnDouble"] = Convert.ToDouble(i);
                item["ColumnFloat"] = Convert.ToSingle(i);
                item["ColumnInt2"] = i;
                item["ColumnMediumInt"] = i;
                item["ColumnReal"] = Convert.ToDouble(i);
                item["ColumnSmallInt"] = Convert.ToInt16(i);
                item["ColumnTinyInt"] = (SByte)i;
                item["ColumnChar"] = "C";
                item["ColumnJson"] = "{\"Field1\": \"Value1\", \"Field2\": \"Value2\"}";
                item["ColumnNChar"] = "C";
                item["ColumnNVarChar"] = $"ColumnNVarChar:{i}";
                item["ColumnLongText"] = $"ColumnLongText:{i}";
                item["ColumnMediumText"] = $"ColumnMediumText:{i}";
                item["ColumnText"] = $"ColumText:{i}";
                item["ColumnTinyText"] = $"ColumnTinyText:{i}";
                item["ColumnBit"] = (UInt64)1;
                tables.Add((ExpandoObject)item);
            }
            return tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateNonIdentityCompleteTableAsExpandoObjectProperties(ExpandoObject table)
        {
            var now = DateTime.SpecifyKind(
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff")),
                    DateTimeKind.Unspecified);
            var item = table as IDictionary<string, object>;
            item["ColumnVarchar"] = $"ColumnVarChar:{2}";
            item["ColumnInt"] = 2;
            item["ColumnDecimal2"] = Convert.ToDecimal(2);
            item["ColumnDateTime"] = EpocDate;
            item["ColumnBlob"] = Encoding.Default.GetBytes($"ColumnBlob:{2}");
            item["ColumnBlobAsArray"] = Encoding.Default.GetBytes($"ColumnBlobAsArray:{2}");
            item["ColumnBinary"] = Encoding.Default.GetBytes($"ColumnBinary:{2}");
            item["ColumnLongBlob"] = Encoding.Default.GetBytes($"ColumnLongBlob:{2}");
            item["ColumnMediumBlob"] = Encoding.Default.GetBytes($"ColumnMediumBlob:{2}");
            item["ColumnTinyBlob"] = Encoding.Default.GetBytes($"ColumnTinyBlob:{2}");
            item["ColumnVarBinary"] = Encoding.Default.GetBytes($"ColumnVarBinary:{2}");
            item["ColumnDate"] = EpocDate;
            item["ColumnDateTime2"] = now;
            item["ColumnTime"] = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay;
            item["ColumnTimeStamp"] = now;
            item["ColumnYear"] = Convert.ToInt16(now.Year);
            //item["ColumnGeometry"] = Encoding.Default.GetBytes($"POLYGON ((0 0; 50 0; 50 50; 0 50; 0 0))");
            //item["ColumnLineString"] = Encoding.Default.GetBytes($"LINESTRING (-122.36 47.656; -122.343 47.656)");
            //item["ColumnMultiLineString"] = Encoding.Default.GetBytes($"ColumnMultiLineString:{2}");
            //item["ColumnMultiPoint"] = Encoding.Default.GetBytes($"ColumnMultiPoint:{2}");
            //item["ColumnMultiPolygon"] = Encoding.Default.GetBytes($"ColumnMultiPolygon:{2}");
            //item["ColumnPoint"] = Encoding.Default.GetBytes($"ColumnPoint:{2}");
            //item["ColumnPolygon"] = Encoding.Default.GetBytes($"ColumnPolygon:{2}");
            item["ColumnBigint"] = Convert.ToInt64(2);
            item["ColumnDecimal"] = Convert.ToDecimal(2);
            item["ColumnDouble"] = Convert.ToDouble(2);
            item["ColumnFloat"] = Convert.ToSingle(2);
            item["ColumnInt2"] = 2;
            item["ColumnMediumInt"] = 2;
            item["ColumnReal"] = Convert.ToDouble(2);
            item["ColumnSmallInt"] = Convert.ToInt16(2);
            item["ColumnTinyInt"] = (SByte)2;
            item["ColumnChar"] = "C";
            item["ColumnJson"] = "{\"Field1\": \"Value1\", \"Field2\": \"Value2\"}";
            item["ColumnNChar"] = "C";
            item["ColumnNVarChar"] = $"ColumnNVarChar:{2}";
            item["ColumnLongText"] = $"ColumnLongText:{2}";
            item["ColumnMediumText"] = $"ColumnMediumText:{2}";
            item["ColumnText"] = $"ColumText:{2}";
            item["ColumnTinyText"] = $"ColumnTinyText:{2}";
            item["ColumnBit"] = (UInt64)1;
        }

        #endregion
    }
}
