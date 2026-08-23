using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.Extensions;
using RepoDb.ClickHouse.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;

namespace RepoDb.ClickHouse.IntegrationTests
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
                else if (value1 is DateTime d1 && value2 is DateTime d2)
                {
                    AssertDateTimeEqualityWithTolerance(d1, d2, propertyOfType1.Name);
                }
                else
                {
                    Assert.AreEqual(value1, value2,
                        $"Assert failed for '{propertyOfType1.Name}'. The values are '{value1} ({propertyOfType1.PropertyType.FullName})' and '{value2} ({propertyOfType2.PropertyType.FullName})'.");
                }
            });
        }

        /// <summary>
        /// Asserts that two <see cref="DateTime"/> values are equal to within a small tolerance, instead of
        /// bit-for-bit (tick) equality.
        /// </summary>
        /// <remarks>
        /// ClickHouse.Driver's HTTP parameter formatter always renders a <c>DateTime64(N)</c> parameter's
        /// fractional-seconds text at a fixed 7-digit (100ns-tick) precision, regardless of the target column's
        /// actual declared scale (see <c>HttpParameterFormatter.Format</c>'s <c>"yyyy-MM-dd HH:mm:ss.fffffff"</c>
        /// DateTime64Type branch) - it does not truncate to the column's own scale before sending. ClickHouse's
        /// own server-side parsing of that over-precise literal against a coarser <c>DateTime64(N)</c> column can
        /// then introduce a sub-millisecond rounding artifact - most visible on <c>ALTER TABLE ... UPDATE</c>
        /// mutations, which additionally re-<c>CAST</c> the value through the column's declared type on top of the
        /// parameter's own declared type (two independent roundings instead of one). None of the values these
        /// test fixtures generate carry meaningful precision below a millisecond (see Helper.CreateCompleteTables
        /// et al., which explicitly truncate "now" to <c>.fff</c> before use), so a few milliseconds of slack here
        /// absorbs that known, external rounding quirk without masking a genuine mismatch (wrong day/hour/etc.
        /// would still fail by many orders of magnitude more than this tolerance).
        /// </remarks>
        /// <param name="expected">The expected <see cref="DateTime"/> value.</param>
        /// <param name="actual">The actual <see cref="DateTime"/> value.</param>
        /// <param name="propertyName">The name of the property being compared, for the failure message.</param>
        private static void AssertDateTimeEqualityWithTolerance(DateTime expected,
            DateTime actual,
            string propertyName)
        {
            var differenceInMilliseconds = Math.Abs((expected - actual).TotalMilliseconds);
            Assert.IsTrue(differenceInMilliseconds < 5,
                $"Assert failed for '{propertyName}'. The values are '{expected:O}' and '{actual:O}' " +
                $"(differ by {differenceInMilliseconds}ms).");
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

                        if (propertyType == typeof(DateTime) && value1 is DateTime dv1 && value2 is DateTime dv2)
                        {
                            AssertDateTimeEqualityWithTolerance(dv1, dv2, property.Name);
                        }
                        else
                        {
                            Assert.AreEqual(Convert.ChangeType(value1, propertyType), Convert.ChangeType(value2, propertyType),
                                $"Assert failed for '{property.Name}'. The values are '{value1}' and '{value2}'.");
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Polls ClickHouse's <c>system.mutations</c> table until every mutation queued for the given
        /// table has finished (or the timeout elapses). ClickHouse's <c>ALTER TABLE ... UPDATE/DELETE</c>
        /// are asynchronous mutations applied by background merges - reading the result of an Update
        /// immediately after issuing it is not guaranteed to observe the change without waiting for this.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="timeout">The maximum time to wait. Defaults to 30 seconds.</param>
        public static void WaitForMutations(ClickHouseConnection connection,
            string tableName,
            TimeSpan? timeout = null)
        {
            var deadline = DateTime.UtcNow.Add(timeout ?? TimeSpan.FromSeconds(30));

            while (DateTime.UtcNow < deadline)
            {
                var pending = connection.ExecuteScalar<long>(
                    "SELECT count(*) FROM system.mutations WHERE database = @Database AND table = @Table AND is_done = 0;",
                    new { Database = connection.Database, Table = tableName });

                if (pending == 0)
                {
                    return;
                }

                Thread.Sleep(100);
            }

            Assert.Fail($"Timed out waiting for pending mutations on table '{tableName}' to complete.");
        }

        /// <summary>
        /// Pauses ClickHouse's background merge scheduler for the given table. <c>ReplacingMergeTree</c>
        /// (used by CompleteTable/NonIdentityCompleteTable - see Setup.Database's DDL) only de-duplicates
        /// rows that share the same sort key when the server gets around to merging their parts together;
        /// that happens on its own schedule, asynchronously, and is not guaranteed to happen - or not to
        /// happen - within any particular window. Tests that assert an exact physical row count right
        /// after inserting duplicate-key rows (e.g. Merge/MergeAll "...AddsRow(s)InsteadOfDeduping") are
        /// otherwise racing the background merge scheduler: a merge that happens to run between the insert
        /// and the count collapses some or all of the duplicates first, so the observed count is whatever
        /// portion of the duplicates got merged away by the time the query runs (this driver's own
        /// `system.parts`-backed row count reflects only currently-active, i.e. already-merged, parts, so
        /// it is subject to the identical race and does not help here). Call this before creating any
        /// duplicate-key rows and call <see cref="StartMerges"/> once done asserting, so the row count is
        /// pinned at "every insert is still a separate, un-merged part" for the whole test.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        public static void StopMerges(ClickHouseConnection connection,
            string tableName)
        {
            connection.ExecuteNonQuery($"SYSTEM STOP MERGES `{tableName}`;");
        }

        /// <summary>
        /// Resumes the background merge scheduler for the given table after <see cref="StopMerges"/>.
        /// Always call this (e.g. from <c>[TestCleanup]</c>) even if the test failed, so a stopped
        /// scheduler doesn't stick around and stall unrelated tests - including mutation-based ones,
        /// since ClickHouse applies <c>ALTER ... UPDATE/DELETE</c> mutations (see <see cref="WaitForMutations"/>)
        /// through the same background merge mechanism this pauses.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        public static void StartMerges(ClickHouseConnection connection,
            string tableName)
        {
            connection.ExecuteNonQuery($"SYSTEM START MERGES `{tableName}`;");
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
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                    DateTimeKind.Unspecified);
            for (var i = 0; i < count; i++)
            {
                tables.Add(new CompleteTable
                {
                    Id = (i + 1),
                    ColumnVarchar = $"ColumnVarChar:{i}",
                    ColumnInt = i,
                    ColumnDecimal2 = Convert.ToDecimal(i),
                    ColumnDateTime = EpocDate,
                    ColumnBlob = $"ColumnBlob:{i}",
                    ColumnBlobAsArray = $"ColumnBlobAsArray:{i}",
                    ColumnBinary = $"ColumnBinary:{i}",
                    ColumnLongBlob = $"ColumnLongBlob:{i}",
                    ColumnMediumBlob = $"ColumnMediumBlob:{i}",
                    ColumnTinyBlob = $"ColumnTinyBlob:{i}",
                    ColumnVarBinary = $"ColumnVarBinary:{i}",
                    ColumnDate = EpocDate,
                    ColumnDateTime2 = now,
                    ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay.ToString(),
                    ColumnTimeStamp = now,
                    ColumnYear = Convert.ToInt16(now.Year),
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
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                    DateTimeKind.Unspecified);
            table.ColumnVarchar = $"ColumnVarChar:{1}-Updated";
            table.ColumnInt = 1;
            table.ColumnDecimal2 = Convert.ToDecimal(1);
            table.ColumnDateTime = EpocDate;
            table.ColumnBlob = $"ColumnBlob:{1}-Updated";
            table.ColumnBlobAsArray = $"ColumnBlobAsArray:{1}-Updated";
            table.ColumnBinary = $"ColumnBinary:{1}-Updated";
            table.ColumnLongBlob = $"ColumnLongBlob:{1}-Updated";
            table.ColumnMediumBlob = $"ColumnMediumBlob:{1}-Updated";
            table.ColumnTinyBlob = $"ColumnTinyBlob:{1}-Updated";
            table.ColumnVarBinary = $"ColumnVarBinary:{1}-Updated";
            table.ColumnDate = EpocDate;
            table.ColumnDateTime2 = now;
            table.ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay.ToString();
            table.ColumnTimeStamp = now;
            table.ColumnYear = Convert.ToInt16(now.Year);
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
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
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
                    ColumnBlob = $"ColumnBlob:{i}",
                    ColumnBlobAsArray = $"ColumnBlobAsArray:{i}",
                    ColumnBinary = $"ColumnBinary:{i}",
                    ColumnLongBlob = $"ColumnLongBlob:{i}",
                    ColumnMediumBlob = $"ColumnMediumBlob:{i}",
                    ColumnTinyBlob = $"ColumnTinyBlob:{i}",
                    ColumnVarBinary = $"ColumnVarBinary:{i}",
                    ColumnDate = EpocDate,
                    ColumnDateTime2 = now,
                    ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay.ToString(),
                    ColumnTimeStamp = now,
                    ColumnYear = Convert.ToInt16(now.Year),
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
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                    DateTimeKind.Unspecified);
            table.ColumnVarchar = $"ColumnVarChar:{1}";
            table.ColumnInt = 1;
            table.ColumnDecimal2 = Convert.ToDecimal(1);
            table.ColumnDateTime = EpocDate;
            table.ColumnBlob = $"ColumnBlob:{1}";
            table.ColumnBlobAsArray = $"ColumnBlobAsArray:{1}";
            table.ColumnBinary = $"ColumnBinary:{1}";
            table.ColumnLongBlob = $"ColumnLongBlob:{1}";
            table.ColumnMediumBlob = $"ColumnMediumBlob:{1}";
            table.ColumnTinyBlob = $"ColumnTinyBlob:{1}";
            table.ColumnVarBinary = $"ColumnVarBinary:{1}";
            table.ColumnDate = EpocDate;
            table.ColumnDateTime2 = now;
            table.ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay.ToString();
            table.ColumnTimeStamp = now;
            table.ColumnYear = Convert.ToInt16(now.Year);
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
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                    DateTimeKind.Unspecified);
            for (var i = 0; i < count; i++)
            {
                var item = new ExpandoObject() as IDictionary<string, object>;
                item["Id"] = (long)(i + 1);
                item["ColumnVarchar"] = $"ColumnVarChar:{i}";
                item["ColumnInt"] = i;
                item["ColumnDecimal2"] = Convert.ToDecimal(i);
                item["ColumnDateTime"] = EpocDate;
                item["ColumnBlob"] = $"ColumnBlob:{i}";
                item["ColumnBlobAsArray"] = $"ColumnBlobAsArray:{i}";
                item["ColumnBinary"] = $"ColumnBinary:{i}";
                item["ColumnLongBlob"] = $"ColumnLongBlob:{i}";
                item["ColumnMediumBlob"] = $"ColumnMediumBlob:{i}";
                item["ColumnTinyBlob"] = $"ColumnTinyBlob:{i}";
                item["ColumnVarBinary"] = $"ColumnVarBinary:{i}";
                item["ColumnDate"] = EpocDate;
                item["ColumnDateTime2"] = now;
                item["ColumnTime"] = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay.ToString();
                item["ColumnTimeStamp"] = now;
                item["ColumnYear"] = Convert.ToInt16(now.Year);
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
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                    DateTimeKind.Unspecified);
            var item = table as IDictionary<string, object>;
            item["ColumnVarchar"] = $"ColumnVarChar:{2}";
            item["ColumnInt"] = 2;
            item["ColumnDecimal2"] = Convert.ToDecimal(2);
            item["ColumnDateTime"] = EpocDate;
            item["ColumnBlob"] = $"ColumnBlob:{2}";
            item["ColumnBlobAsArray"] = $"ColumnBlobAsArray:{2}";
            item["ColumnBinary"] = $"ColumnBinary:{2}";
            item["ColumnLongBlob"] = $"ColumnLongBlob:{2}";
            item["ColumnMediumBlob"] = $"ColumnMediumBlob:{2}";
            item["ColumnTinyBlob"] = $"ColumnTinyBlob:{2}";
            item["ColumnVarBinary"] = $"ColumnVarBinary:{2}";
            item["ColumnDate"] = EpocDate;
            item["ColumnDateTime2"] = now;
            item["ColumnTime"] = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay.ToString();
            item["ColumnTimeStamp"] = now;
            item["ColumnYear"] = Convert.ToInt16(now.Year);
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
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
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
                    ColumnBlob = $"ColumnBlob:{i}",
                    ColumnBlobAsArray = $"ColumnBlobAsArray:{i}",
                    ColumnBinary = $"ColumnBinary:{i}",
                    ColumnLongBlob = $"ColumnLongBlob:{i}",
                    ColumnMediumBlob = $"ColumnMediumBlob:{i}",
                    ColumnTinyBlob = $"ColumnTinyBlob:{i}",
                    ColumnVarBinary = $"ColumnVarBinary:{i}",
                    ColumnDate = EpocDate,
                    ColumnDateTime2 = now,
                    ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay.ToString(),
                    ColumnTimeStamp = now,
                    ColumnYear = Convert.ToInt16(now.Year),
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
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                    DateTimeKind.Unspecified);
            table.ColumnVarchar = $"ColumnVarChar:{1}";
            table.ColumnInt = 1;
            table.ColumnDecimal2 = Convert.ToDecimal(1);
            table.ColumnDateTime = EpocDate;
            table.ColumnBlob = $"ColumnBlob:{1}";
            table.ColumnBlobAsArray = $"ColumnBlobAsArray:{1}";
            table.ColumnBinary = $"ColumnBinary:{1}";
            table.ColumnLongBlob = $"ColumnLongBlob:{1}";
            table.ColumnMediumBlob = $"ColumnMediumBlob:{1}";
            table.ColumnTinyBlob = $"ColumnTinyBlob:{1}";
            table.ColumnVarBinary = $"ColumnVarBinary:{1}";
            table.ColumnDate = EpocDate;
            table.ColumnDateTime2 = now;
            table.ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay.ToString();
            table.ColumnTimeStamp = now;
            table.ColumnYear = Convert.ToInt16(now.Year);
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
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
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
                    ColumnBlob = $"ColumnBlob:{i}",
                    ColumnBlobAsArray = $"ColumnBlobAsArray:{i}",
                    ColumnBinary = $"ColumnBinary:{i}",
                    ColumnLongBlob = $"ColumnLongBlob:{i}",
                    ColumnMediumBlob = $"ColumnMediumBlob:{i}",
                    ColumnTinyBlob = $"ColumnTinyBlob:{i}",
                    ColumnVarBinary = $"ColumnVarBinary:{i}",
                    ColumnDate = EpocDate,
                    ColumnDateTime2 = now,
                    ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay.ToString(),
                    ColumnTimeStamp = now,
                    ColumnYear = Convert.ToInt16(now.Year),
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
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                    DateTimeKind.Unspecified);
            table.ColumnVarchar = $"ColumnVarChar:{1}";
            table.ColumnInt = 1;
            table.ColumnDecimal2 = Convert.ToDecimal(1);
            table.ColumnDateTime = EpocDate;
            table.ColumnBlob = $"ColumnBlob:{1}";
            table.ColumnBlobAsArray = $"ColumnBlobAsArray:{1}";
            table.ColumnBinary = $"ColumnBinary:{1}";
            table.ColumnLongBlob = $"ColumnLongBlob:{1}";
            table.ColumnMediumBlob = $"ColumnMediumBlob:{1}";
            table.ColumnTinyBlob = $"ColumnTinyBlob:{1}";
            table.ColumnVarBinary = $"ColumnVarBinary:{1}";
            table.ColumnDate = EpocDate;
            table.ColumnDateTime2 = now;
            table.ColumnTime = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay.ToString();
            table.ColumnTimeStamp = now;
            table.ColumnYear = Convert.ToInt16(now.Year);
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
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                    DateTimeKind.Unspecified);
            for (var i = 0; i < count; i++)
            {
                var item = new ExpandoObject() as IDictionary<string, object>;
                item["Id"] = (long)(i + 1);
                item["ColumnVarchar"] = $"ColumnVarChar:{i}";
                item["ColumnInt"] = i;
                item["ColumnDecimal2"] = Convert.ToDecimal(i);
                item["ColumnDateTime"] = EpocDate;
                item["ColumnBlob"] = $"ColumnBlob:{i}";
                item["ColumnBlobAsArray"] = $"ColumnBlobAsArray:{i}";
                item["ColumnBinary"] = $"ColumnBinary:{i}";
                item["ColumnLongBlob"] = $"ColumnLongBlob:{i}";
                item["ColumnMediumBlob"] = $"ColumnMediumBlob:{i}";
                item["ColumnTinyBlob"] = $"ColumnTinyBlob:{i}";
                item["ColumnVarBinary"] = $"ColumnVarBinary:{i}";
                item["ColumnDate"] = EpocDate;
                item["ColumnDateTime2"] = now;
                item["ColumnTime"] = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay.ToString();
                item["ColumnTimeStamp"] = now;
                item["ColumnYear"] = Convert.ToInt16(now.Year);
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
                DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                    DateTimeKind.Unspecified);
            var item = table as IDictionary<string, object>;
            item["ColumnVarchar"] = $"ColumnVarChar:{2}";
            item["ColumnInt"] = 2;
            item["ColumnDecimal2"] = Convert.ToDecimal(2);
            item["ColumnDateTime"] = EpocDate;
            item["ColumnBlob"] = $"ColumnBlob:{2}";
            item["ColumnBlobAsArray"] = $"ColumnBlobAsArray:{2}";
            item["ColumnBinary"] = $"ColumnBinary:{2}";
            item["ColumnLongBlob"] = $"ColumnLongBlob:{2}";
            item["ColumnMediumBlob"] = $"ColumnMediumBlob:{2}";
            item["ColumnTinyBlob"] = $"ColumnTinyBlob:{2}";
            item["ColumnVarBinary"] = $"ColumnVarBinary:{2}";
            item["ColumnDate"] = EpocDate;
            item["ColumnDateTime2"] = now;
            item["ColumnTime"] = EpocDate.AddHours(5).AddMinutes(7).AddSeconds(12).TimeOfDay.ToString();
            item["ColumnTimeStamp"] = now;
            item["ColumnYear"] = Convert.ToInt16(now.Year);
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
