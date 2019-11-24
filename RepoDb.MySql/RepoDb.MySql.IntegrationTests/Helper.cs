using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.MySql.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace RepoDb.MySql.IntegrationTests
{
    public static class Helper
    {
        static Helper()
        {
            EpocDate = new DateTime(1970, 1, 1, 0, 0, 0);
        }

        /// <summary>
        /// Gets the value of the Epoc date.
        /// </summary>
        public static DateTime EpocDate { get; }

        /// <summary>
        /// Gets the current <see cref="Random"/> object in used.
        /// </summary>
        public static Random Randomizer => new Random(1);

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
                    for (var i = 0; i < b1.Length; i++)
                    {
                        var v1 = b1[i];
                        var v2 = b2[i];
                        Assert.AreEqual(v1, v2,
                            $"Assert failed for '{propertyOfType1.Name}'. The values are '{value1} ({propertyOfType1.PropertyType.FullName})' and '{value2} ({propertyOfType2.PropertyType.FullName})'.");
                    }
                }
                else
                {
                    value1 = Flatten(value1);
                    value2 = Flatten(value2);
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
                        for (var i = 0; i < b1.Length; i++)
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
                        value1 = Flatten(value1);
                        value2 = Flatten(value2);
                        Assert.AreEqual(Convert.ChangeType(value1, propertyType), Convert.ChangeType(value2, propertyType),
                            $"Assert failed for '{property.Name}'. The values are '{value1}' and '{value2}'.");
                    }
                }
            });
        }

        /// <summary>
        /// Flatten the object value.
        /// </summary>
        /// <param name="value">The value to be flattened.</param>
        /// <returns>The flattened value.</returns>
        private static object Flatten(object value)
        {
            if (value is DateTime)
            {
                return ((DateTime)value).Flatten();
            }
            else if (value is DateTime?)
            {
                return new DateTime?(((DateTime?)value).Value.Flatten());
            }
            else if (value is TimeSpan)
            {
                return ((TimeSpan)value).Flatten();
            }
            else if (value is TimeSpan?)
            {
                return new TimeSpan?(((TimeSpan?)value).Value.Flatten());
            }
            return value;
        }

        #region CompleteTable

        /// <summary>
        /// Creates a list of <see cref="CompleteTable"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="CompleteTable"/> objects.</returns>
        public static List<CompleteTable> CreateCompleteTables(int count)
        {
            var tables = new List<CompleteTable>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new CompleteTable
                {
                    ColumnVarchar = $"ColumnVarChar:{i}",
                    ColumnInt = i,
                    ColumnDecimal2 = Convert.ToDecimal(i),
                    ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    //ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    //ColumnBlobAsArray = Encoding.Default.GetBytes($"ColumnBlobAsArray:{i}"),
                    //ColumnBinary = Encoding.Default.GetBytes($"ColumnBinary:{i}"),
                    //ColumnLongBlob = Encoding.Default.GetBytes($"ColumnLongBlob:{i}"),
                    //ColumnMediumBlob = Encoding.Default.GetBytes($"ColumnMediumBlob:{i}"),
                    //ColumnTinyBlob = Encoding.Default.GetBytes($"ColumnTinyBlob:{i}"),
                    //ColumnVarBinary = Encoding.Default.GetBytes($"ColumnVarBinary:{i}"),
                    ColumnDate = EpocDate,
                    //ColumnDateTime2 = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).TimeOfDay,
                    //ColumnTimeStamp = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnYear = Convert.ToInt16(DateTime.UtcNow.Year),
                    //ColumnGeometry = Encoding.Default.GetBytes($"ColumnGeometry:{i}"),
                    //ColumnLineString = Encoding.Default.GetBytes($"ColumnLineString:{i}"),
                    //ColumnMultiLineString = Encoding.Default.GetBytes($"ColumnMultiLineString:{i}"),
                    //ColumnMultiPoint = Encoding.Default.GetBytes($"ColumnMultiPoint:{i}"),
                    //ColumnMultiPolygon = Encoding.Default.GetBytes($"ColumnMultiPolygon:{i}"),
                    //ColumnPoint = Encoding.Default.GetBytes($"ColumnPoint:{i}"),
                    //ColumnPolygon = Encoding.Default.GetBytes($"ColumnPolygon:{i}"),
                    ColumnBigint = i,
                    ColumnDecimal = Convert.ToDecimal(i),
                    ColumnDouble = Convert.ToDouble(i),
                    ColumnFloat = Convert.ToSingle(i),
                    ColumnInt2 = i,
                    ColumnMediumInt = i,
                    ColumnReal = Convert.ToDouble(i),
                    ColumnSmallInt = Convert.ToInt16(i),
                    //ColumnTinyInt = (SByte)i,
                    ColumnChar = "C",
                    ColumnJson = "{\"Field1\": \"Value1\", \"Field2\": \"Value2\"}",
                    ColumnNChar = "C",
                    ColumnNVarChar = $"ColumnNVarChar:{i}",
                    ColumnLongText = $"ColumnLongText:{i}",
                    ColumnMediumText = $"ColumnMediumText:{i}",
                    ColumnText = $"ColumText:{i}",
                    ColumnTinyText = $"ColumnTinyText:{i}",
                    //ColumnBit = (UInt64)i
                });
            }
            return tables;
        }

        /// <summary>
        /// Update the properties of <see cref="CompleteTable"/> instance.
        /// </summary>
        /// <param name="table">The instance to be updated.</param>
        public static void UpdateCompleteTableProperties(CompleteTable table)
        {
            table.ColumnVarchar = $"table.ColumnVarChar:{1000000}";
            table.ColumnInt = 1000000;
            table.ColumnDecimal2 = Convert.ToDecimal(1000000);
            table.ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            //table.ColumnBlob = Encoding.Default.GetBytes($"table.ColumnBlob:{1000000}");
            //table.ColumnBlobAsArray = Encoding.Default.GetBytes($"table.ColumnBlobAsArray:{1000000}");
            //table.ColumnBinary = Encoding.Default.GetBytes($"table.ColumnBinary:{1000000}");
            //table.ColumnLongBlob = Encoding.Default.GetBytes($"table.ColumnLongBlob:{1000000}");
            //table.ColumnMediumBlob = Encoding.Default.GetBytes($"table.ColumnMediumBlob:{1000000}");
            //table.ColumnTinyBlob = Encoding.Default.GetBytes($"table.ColumnTinyBlob:{1000000}");
            //table.ColumnVarBinary = Encoding.Default.GetBytes($"table.ColumnVarBinary:{1000000}");
            table.ColumnDate = EpocDate;
            table.ColumnDateTime2 = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            table.ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).TimeOfDay;
            table.ColumnTimeStamp = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            table.ColumnYear = Convert.ToInt16(DateTime.UtcNow.Year);
            //table.ColumnGeometry = Encoding.Default.GetBytes($"table.ColumnGeometry:{1000000}");
            //table.ColumnLineString = Encoding.Default.GetBytes($"table.ColumnLineString:{1000000}");
            //table.ColumnMultiLineString = Encoding.Default.GetBytes($"table.ColumnMultiLineString:{1000000}");
            //table.ColumnMultiPoint = Encoding.Default.GetBytes($"table.ColumnMultiPoint:{1000000}");
            //table.ColumnMultiPolygon = Encoding.Default.GetBytes($"table.ColumnMultiPolygon:{1000000}");
            //table.ColumnPoint = Encoding.Default.GetBytes($"table.ColumnPoint:{1000000}");
            //table.ColumnPolygon = Encoding.Default.GetBytes($"table.ColumnPolygon:{1000000}");
            table.ColumnBigint = 1000000;
            table.ColumnDecimal = Convert.ToDecimal(1000000);
            table.ColumnDouble = Convert.ToDouble(1000000);
            table.ColumnFloat = Convert.ToSingle(1000000);
            table.ColumnInt2 = 1000000;
            table.ColumnMediumInt = 1000000;
            table.ColumnReal = Convert.ToDouble(1000000);
            table.ColumnSmallInt = Convert.ToInt16(1000000);
            //table.ColumnTinyInt = (SByte)1;
            table.ColumnChar = "C";
            table.ColumnJson = "{ \"Field\" : \"Value\" }";
            table.ColumnNChar = "C";
            table.ColumnNVarChar = $"table.ColumnNVarChar:{1000000}";
            table.ColumnLongText = $"table.ColumnLongText:{1000000}";
            table.ColumnMediumText = $"table.ColumnMediumText:{1000000}";
            table.ColumnText = $"ColumText:{1000000}";
            table.ColumnTinyText = $"table.ColumnTinyText:{1000000}";
            //table.ColumnBit = (UInt64)1000000;
        }

        /// <summary>
        /// Creates a list of <see cref="CompleteTable"/> objects represented as dynamics.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="CompleteTable"/> objects represented as dynamics.</returns>
        public static List<dynamic> CreateCompleteTablesAsDynamics(int count)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new
                {
                });
            }
            return tables;
        }

        /// <summary>
        /// Update the properties of <see cref="CompleteTable"/> instance represented asy dynamic.
        /// </summary>
        /// <param name="table">The instance to be updated.</param>
        public static void UpdateCompleteTableAsDynamicProperties(dynamic table)
        {
        }

        #endregion

        #region NonIdentityCompleteTable

        /// <summary>
        /// Creates a list of <see cref="NonIdentityCompleteTable"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="NonIdentityCompleteTable"/> objects.</returns>
        public static List<NonIdentityCompleteTable> CreateNonIdentityCompleteTables(int count)
        {
            var tables = new List<NonIdentityCompleteTable>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new NonIdentityCompleteTable
                {
                });
            }
            return tables;
        }

        /// <summary>
        /// Update the properties of <see cref="NonIdentityCompleteTable"/> instance.
        /// </summary>
        /// <param name="table">The instance to be updated.</param>
        public static void UpdateNonIdentityCompleteTableProperties(NonIdentityCompleteTable table)
        {
        }

        /// <summary>
        /// Creates a list of <see cref="NonIdentityCompleteTable"/> objects represented as dynamics.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="NonIdentityCompleteTable"/> objects represented as dynamics.</returns>
        public static List<dynamic> CreateNonIdentityCompleteTablesAsDynamics(int count)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new
                {
                    Id = (long)(i + 1),
                    ColumnBigInt = i,
                    ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    ColumnBoolean = true,
                    ColumnChar = "C",
                    ColumnDate = EpocDate,
                    ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnDecimal = Convert.ToDecimal(i),
                    ColumnDouble = Convert.ToDouble(i),
                    ColumnInt = i,
                    ColumnInteger = i,
                    ColumnNone = "N",
                    ColumnNumeric = Convert.ToDecimal(i),
                    ColumnReal = (float)i,
                    ColumnString = $"ColumnString:{i}",
                    ColumnText = $"ColumnText:{i}",
                    ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).TimeOfDay,
                    ColumnVarChar = $"ColumnVarChar:{i}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Update the properties of <see cref="NonIdentityCompleteTable"/> instance represented asy dynamic.
        /// </summary>
        /// <param name="table">The instance to be updated.</param>
        public static void UpdateNonIdentityCompleteTableAsDynamicProperties(dynamic table)
        {
            table.ColumnBigInt = long.MaxValue;
            table.ColumnBlob = Encoding.UTF32.GetBytes(Guid.NewGuid().ToString());
            table.ColumnBoolean = true;
            table.ColumnChar = char.Parse("C").ToString();
            table.ColumnDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).Date;
            table.ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            table.ColumnDecimal = Convert.ToDecimal(Randomizer.Next(1000000));
            table.ColumnDouble = Convert.ToDouble(Randomizer.Next(1000000));
            table.ColumnInt = Randomizer.Next(1000000);
            table.ColumnInteger = Convert.ToInt64(Randomizer.Next(1000000));
            table.ColumnNumeric = Convert.ToDecimal(Randomizer.Next(1000000));
            table.ColumnReal = Convert.ToSingle(Randomizer.Next(1000000));
            table.ColumnString = $"{table.ColumnString} - Updated with {Guid.NewGuid().ToString()}";
            table.ColumnText = $"{table.ColumnText} - Updated with {Guid.NewGuid().ToString()}";
            table.ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).TimeOfDay;
            table.ColumnVarChar = $"{table.ColumnVarChar} - Updated with {Guid.NewGuid().ToString()}";
        }

        #endregion
    }
}
