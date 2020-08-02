using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace RepoDb.SqLite.IntegrationTests
{
    public static class Helper
    {
        public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

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
                        else if (propertyType == typeof(string) && value2 is DateTime)
                        {
                            value1 = DateTime.Parse(value1?.ToString());
                        }
                        Assert.AreEqual(Convert.ChangeType(value1, propertyType), Convert.ChangeType(value2, propertyType),
                            $"Assert failed for '{property.Name}'. The values are '{value1}' and '{value2}'.");
                    }
                }
            });
        }

        #region SdsCompleteTable

        /// <summary>
        /// Creates a list of <see cref="SdsCompleteTable"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="SdsCompleteTable"/> objects.</returns>
        public static List<SdsCompleteTable> CreateSdsCompleteTables(int count)
        {
            var tables = new List<SdsCompleteTable>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new SdsCompleteTable
                {
                    ColumnBigInt = i,
                    ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    ColumnBoolean = true,
                    ColumnChar = "C",
                    ColumnDate = EpocDate,
                    ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnDecimal = Convert.ToInt64(i),
                    ColumnDouble = Convert.ToDouble(i),
                    ColumnInt = i,
                    ColumnInteger = i,
                    ColumnNone = "N",
                    ColumnNumeric = Convert.ToInt64(i),
                    ColumnReal = (float)i,
                    ColumnString = $"ColumnString:{i}",
                    ColumnText = $"ColumnText:{i}",
                    ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnVarChar = $"ColumnVarChar:{i}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Update the properties of <see cref="SdsCompleteTable"/> instance.
        /// </summary>
        /// <param name="table">The instance to be updated.</param>
        public static void UpdateSdsCompleteTableProperties(SdsCompleteTable table)
        {
            table.ColumnBigInt = long.MaxValue;
            table.ColumnBlob = Encoding.UTF32.GetBytes(Guid.NewGuid().ToString());
            table.ColumnBoolean = true;
            table.ColumnChar = char.Parse("C").ToString();
            table.ColumnDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).Date;
            table.ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            table.ColumnDecimal = Randomizer.Next(1000000);
            table.ColumnDouble = Convert.ToDouble(Randomizer.Next(1000000));
            table.ColumnInt = Randomizer.Next(1000000);
            table.ColumnInteger = Convert.ToInt64(Randomizer.Next(1000000));
            table.ColumnNumeric = Randomizer.Next(1000000);
            table.ColumnReal = Convert.ToSingle(Randomizer.Next(1000000));
            table.ColumnString = $"{table.ColumnString} - Updated with {Guid.NewGuid().ToString()}";
            table.ColumnText = $"{table.ColumnText} - Updated with {Guid.NewGuid().ToString()}";
            table.ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            table.ColumnVarChar = $"{table.ColumnVarChar} - Updated with {Guid.NewGuid().ToString()}";
        }

        /// <summary>
        /// Creates a list of <see cref="SdsCompleteTable"/> objects represented as dynamics.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="SdsCompleteTable"/> objects represented as dynamics.</returns>
        public static List<dynamic> CreateSdsCompleteTablesAsDynamics(int count)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new
                {
                    Id = (long)(i + 1),
                    ColumnBigInt = (long)i,
                    ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    ColumnBoolean = true,
                    ColumnChar = "C",
                    ColumnDate = EpocDate,
                    ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnDecimal = Convert.ToDecimal(i),
                    ColumnDouble = Convert.ToDouble(i),
                    ColumnInt = i,
                    ColumnInteger = (long)i,
                    ColumnNone = "N",
                    ColumnNumeric = Convert.ToDecimal(i),
                    ColumnReal = (float)i,
                    ColumnString = $"ColumnString:{i}",
                    ColumnText = $"ColumnText:{i}",
                    ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnVarChar = $"ColumnVarChar:{i}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Update the properties of <see cref="SdsCompleteTable"/> instance represented asy dynamic.
        /// </summary>
        /// <param name="table">The instance to be updated.</param>
        public static void UpdateSdsCompleteTableAsDynamicProperties(dynamic table)
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
            table.ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            table.ColumnVarChar = $"{table.ColumnVarChar} - Updated with {Guid.NewGuid().ToString()}";
        }

        #endregion

        #region SdsNonIdentityCompleteTable

        /// <summary>
        /// Creates a list of <see cref="SdsNonIdentityCompleteTable"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="SdsNonIdentityCompleteTable"/> objects.</returns>
        public static List<SdsNonIdentityCompleteTable> CreateSdsNonIdentityCompleteTables(int count)
        {
            var tables = new List<SdsNonIdentityCompleteTable>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new SdsNonIdentityCompleteTable
                {
                    Id = (long)(i + 1),
                    ColumnBigInt = (long)i,
                    ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    ColumnBoolean = true,
                    ColumnChar = "C",
                    ColumnDate = EpocDate,
                    ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnDecimal = i,
                    ColumnDouble = Convert.ToDouble(i),
                    ColumnInt = i,
                    ColumnInteger = (long)i,
                    ColumnNone = "N",
                    ColumnNumeric = i,
                    ColumnReal = (float)i,
                    ColumnString = $"ColumnString:{i}",
                    ColumnText = $"ColumnText:{i}",
                    ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnVarChar = $"ColumnVarChar:{i}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Update the properties of <see cref="SdsNonIdentityCompleteTable"/> instance.
        /// </summary>
        /// <param name="table">The instance to be updated.</param>
        public static void UpdateSdsNonIdentityCompleteTableProperties(SdsNonIdentityCompleteTable table)
        {
            table.ColumnBigInt = long.MaxValue;
            table.ColumnBlob = Encoding.UTF32.GetBytes(Guid.NewGuid().ToString());
            table.ColumnBoolean = true;
            table.ColumnChar = char.Parse("C").ToString();
            table.ColumnDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).Date;
            table.ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            table.ColumnDecimal = Randomizer.Next(1000000);
            table.ColumnDouble = Convert.ToDouble(Randomizer.Next(1000000));
            table.ColumnInt = Randomizer.Next(1000000);
            table.ColumnInteger = Convert.ToInt64(Randomizer.Next(1000000));
            table.ColumnNumeric = Randomizer.Next(1000000);
            table.ColumnReal = Convert.ToSingle(Randomizer.Next(1000000));
            table.ColumnString = $"{table.ColumnString} - Updated with {Guid.NewGuid().ToString()}";
            table.ColumnText = $"{table.ColumnText} - Updated with {Guid.NewGuid().ToString()}";
            table.ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            table.ColumnVarChar = $"{table.ColumnVarChar} - Updated with {Guid.NewGuid().ToString()}";
        }

        /// <summary>
        /// Creates a list of <see cref="SdsNonIdentityCompleteTable"/> objects represented as dynamics.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="SdsNonIdentityCompleteTable"/> objects represented as dynamics.</returns>
        public static List<dynamic> CreateSdsNonIdentityCompleteTablesAsDynamics(int count)
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
                    ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnVarChar = $"ColumnVarChar:{i}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Update the properties of <see cref="SdsNonIdentityCompleteTable"/> instance represented asy dynamic.
        /// </summary>
        /// <param name="table">The instance to be updated.</param>
        public static void UpdateSdsNonIdentityCompleteTableAsDynamicProperties(dynamic table)
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
            table.ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            table.ColumnVarChar = $"{table.ColumnVarChar} - Updated with {Guid.NewGuid().ToString()}";
        }

        #endregion

        #region MdsCompleteTable

        /// <summary>
        /// Creates a list of <see cref="MdsCompleteTable"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="MdsCompleteTable"/> objects.</returns>
        public static List<MdsCompleteTable> CreateMdsCompleteTables(int count)
        {
            var tables = new List<MdsCompleteTable>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new MdsCompleteTable
                {
                    ColumnBigInt = i,
                    ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    ColumnBoolean = "true",
                    ColumnChar = "C",
                    ColumnDate = EpocDate.ToString(DATE_FORMAT),
                    ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).ToString(DATE_FORMAT),
                    ColumnDecimal = Convert.ToInt64(i),
                    ColumnDouble = Convert.ToDouble(i),
                    ColumnInt = i,
                    ColumnInteger = i,
                    ColumnNone = "N",
                    ColumnNumeric = Convert.ToInt64(i),
                    ColumnReal = (float)i,
                    ColumnString = $"ColumnString:{i}",
                    ColumnText = $"ColumnText:{i}",
                    ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).ToString(DATE_FORMAT),
                    ColumnVarChar = $"ColumnVarChar:{i}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Update the properties of <see cref="MdsCompleteTable"/> instance.
        /// </summary>
        /// <param name="table">The instance to be updated.</param>
        public static void UpdateMdsCompleteTableProperties(MdsCompleteTable table)
        {
            table.ColumnBigInt = long.MaxValue;
            table.ColumnBlob = Encoding.UTF32.GetBytes(Guid.NewGuid().ToString());
            table.ColumnBoolean = "true";
            table.ColumnChar = char.Parse("C").ToString();
            table.ColumnDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).Date.ToString(DATE_FORMAT);
            table.ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).ToString(DATE_FORMAT);
            table.ColumnDecimal = Randomizer.Next(1000000);
            table.ColumnDouble = Convert.ToDouble(Randomizer.Next(1000000));
            table.ColumnInt = Randomizer.Next(1000000);
            table.ColumnInteger = Convert.ToInt64(Randomizer.Next(1000000));
            table.ColumnNumeric = Randomizer.Next(1000000);
            table.ColumnReal = Convert.ToSingle(Randomizer.Next(1000000));
            table.ColumnString = $"{table.ColumnString} - Updated with {Guid.NewGuid().ToString()}";
            table.ColumnText = $"{table.ColumnText} - Updated with {Guid.NewGuid().ToString()}";
            table.ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).ToString(DATE_FORMAT);
            table.ColumnVarChar = $"{table.ColumnVarChar} - Updated with {Guid.NewGuid().ToString()}";
        }

        /// <summary>
        /// Creates a list of <see cref="MdsCompleteTable"/> objects represented as dynamics.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="MdsCompleteTable"/> objects represented as dynamics.</returns>
        public static List<dynamic> CreateMdsCompleteTablesAsDynamics(int count)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new
                {
                    Id = (long)(i + 1),
                    ColumnBigInt = (long)i,
                    ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    ColumnBoolean = "true",
                    ColumnChar = "C",
                    ColumnDate = EpocDate,
                    ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnDecimal = Convert.ToDecimal(i),
                    ColumnDouble = Convert.ToDouble(i),
                    ColumnInt = i,
                    ColumnInteger = (long)i,
                    ColumnNone = "N",
                    ColumnNumeric = Convert.ToDecimal(i),
                    ColumnReal = (float)i,
                    ColumnString = $"ColumnString:{i}",
                    ColumnText = $"ColumnText:{i}",
                    ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnVarChar = $"ColumnVarChar:{i}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Update the properties of <see cref="MdsCompleteTable"/> instance represented asy dynamic.
        /// </summary>
        /// <param name="table">The instance to be updated.</param>
        public static void UpdateMdsCompleteTableAsDynamicProperties(dynamic table)
        {
            table.ColumnBigInt = long.MaxValue;
            table.ColumnBlob = Encoding.UTF32.GetBytes(Guid.NewGuid().ToString());
            table.ColumnBoolean = "true";
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
            table.ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            table.ColumnVarChar = $"{table.ColumnVarChar} - Updated with {Guid.NewGuid().ToString()}";
        }

        #endregion

        #region MdsNonIdentityCompleteTable

        /// <summary>
        /// Creates a list of <see cref="MdsNonIdentityCompleteTable"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="MdsNonIdentityCompleteTable"/> objects.</returns>
        public static List<MdsNonIdentityCompleteTable> CreateMdsNonIdentityCompleteTables(int count)
        {
            var tables = new List<MdsNonIdentityCompleteTable>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new MdsNonIdentityCompleteTable
                {
                    Id = (long)(i + 1),
                    ColumnBigInt = (long)i,
                    ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    ColumnBoolean = "true",
                    ColumnChar = "C",
                    ColumnDate = EpocDate.ToString(DATE_FORMAT),
                    ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).ToString(DATE_FORMAT),
                    ColumnDecimal = i,
                    ColumnDouble = Convert.ToDouble(i),
                    ColumnInt = i,
                    ColumnInteger = (long)i,
                    ColumnNone = "N",
                    ColumnNumeric = i,
                    ColumnReal = (float)i,
                    ColumnString = $"ColumnString:{i}",
                    ColumnText = $"ColumnText:{i}",
                    ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).ToString(DATE_FORMAT),
                    ColumnVarChar = $"ColumnVarChar:{i}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Update the properties of <see cref="MdsNonIdentityCompleteTable"/> instance.
        /// </summary>
        /// <param name="table">The instance to be updated.</param>
        public static void UpdateMdsNonIdentityCompleteTableProperties(MdsNonIdentityCompleteTable table)
        {
            table.ColumnBigInt = long.MaxValue;
            table.ColumnBlob = Encoding.UTF32.GetBytes(Guid.NewGuid().ToString());
            table.ColumnBoolean = "true";
            table.ColumnChar = char.Parse("C").ToString();
            table.ColumnDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).Date.ToString(DATE_FORMAT);
            table.ColumnDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).ToString(DATE_FORMAT);
            table.ColumnDecimal = Randomizer.Next(1000000);
            table.ColumnDouble = Convert.ToDouble(Randomizer.Next(1000000));
            table.ColumnInt = Randomizer.Next(1000000);
            table.ColumnInteger = Convert.ToInt64(Randomizer.Next(1000000));
            table.ColumnNumeric = Randomizer.Next(1000000);
            table.ColumnReal = Convert.ToSingle(Randomizer.Next(1000000));
            table.ColumnString = $"{table.ColumnString} - Updated with {Guid.NewGuid().ToString()}";
            table.ColumnText = $"{table.ColumnText} - Updated with {Guid.NewGuid().ToString()}";
            table.ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified).ToString(DATE_FORMAT);
            table.ColumnVarChar = $"{table.ColumnVarChar} - Updated with {Guid.NewGuid().ToString()}";
        }

        /// <summary>
        /// Creates a list of <see cref="MdsNonIdentityCompleteTable"/> objects represented as dynamics.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="MdsNonIdentityCompleteTable"/> objects represented as dynamics.</returns>
        public static List<dynamic> CreateMdsNonIdentityCompleteTablesAsDynamics(int count)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                tables.Add(new
                {
                    Id = (long)(i + 1),
                    ColumnBigInt = i,
                    ColumnBlob = Encoding.Default.GetBytes($"ColumnBlob:{i}"),
                    ColumnBoolean = "true",
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
                    ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    ColumnVarChar = $"ColumnVarChar:{i}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Update the properties of <see cref="MdsNonIdentityCompleteTable"/> instance represented asy dynamic.
        /// </summary>
        /// <param name="table">The instance to be updated.</param>
        public static void UpdateMdsNonIdentityCompleteTableAsDynamicProperties(dynamic table)
        {
            table.ColumnBigInt = long.MaxValue;
            table.ColumnBlob = Encoding.UTF32.GetBytes(Guid.NewGuid().ToString());
            table.ColumnBoolean = "true";
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
            table.ColumnTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            table.ColumnVarChar = $"{table.ColumnVarChar} - Updated with {Guid.NewGuid().ToString()}";
        }

        #endregion
    }
}
