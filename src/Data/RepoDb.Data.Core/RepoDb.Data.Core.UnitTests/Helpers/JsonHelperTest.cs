#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Helpers;

namespace RepoDb.Data.Core.UnitTests.Helpers
{
    [TestClass]
    public class JsonHelperTest
    {
        #region Helpers

        private readonly List<string> _files = new List<string>();

        private string Write(string text)
        {
            var path = Path.Combine(Path.GetTempPath(), "repodb_json_helper_" + Guid.NewGuid().ToString("N") + ".json");
            File.WriteAllText(path, text, new UTF8Encoding(false));
            _files.Add(path);
            return path;
        }

        private static JsonElement Element(string json) =>
            JsonDocument.Parse(json).RootElement;

        [TestCleanup]
        public void Cleanup()
        {
            foreach (var file in _files.Where(File.Exists))
            {
                File.Delete(file);
            }
            _files.Clear();
        }

        #endregion

        #region ReadColumns

        [TestMethod]
        public void TestReadColumnsReturnsThePropertiesOfTheRowsInTheOrderThatTheyWereFound()
        {
            // Setup
            var path = Write("[{\"Id\":1,\"Name\":\"A\"},{\"Id\":2,\"Birthday\":\"2026-01-01\"}]");

            // Act
            var actual = JsonHelper.ReadColumns(path);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Birthday" }, actual.ToArray());
        }

        [TestMethod]
        public void TestReadColumnsIgnoresTheCaseOfTheNames()
        {
            // Setup
            var path = Write("[{\"Id\":1,\"Name\":\"A\"},{\"id\":2,\"NAME\":\"B\"}]");

            // Act
            var actual = JsonHelper.ReadColumns(path);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, actual.ToArray());
        }

        [TestMethod]
        public void TestReadColumnsLooksAtTheFirstRowsOnly()
        {
            // Setup
            var rows = Enumerable.Range(1, JsonHelper.ProbeRows).Select(i => "{\"Id\":" + i + "}").ToList();
            rows.Add("{\"Id\":0,\"Late\":1}");
            var path = Write("[" + string.Join(",", rows) + "]");

            // Act
            var actual = JsonHelper.ReadColumns(path);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id" }, actual.ToArray());
        }

        [TestMethod]
        public void TestReadColumnsOfARowThatComesAfterTheFirstOneButWithinTheProbe()
        {
            // Setup
            var rows = Enumerable.Range(1, JsonHelper.ProbeRows - 1).Select(i => "{\"Id\":" + i + "}").ToList();
            rows.Add("{\"Id\":0,\"Last\":1}");
            var path = Write("[" + string.Join(",", rows) + "]");

            // Act
            var actual = JsonHelper.ReadColumns(path);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Last" }, actual.ToArray());
        }

        [TestMethod]
        public void TestReadColumnsOfAnEmptyArrayReturnsNoColumn()
        {
            // Setup
            var path = Write("[]");

            // Act
            var actual = JsonHelper.ReadColumns(path);

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestReadColumnsOfASequenceOfObjects()
        {
            // Setup
            var path = Write("{\"Id\":1,\"Name\":\"A\"}\n{\"Id\":2,\"Name\":\"B\"}\n");

            // Act
            var actual = JsonHelper.ReadColumns(path);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, actual.ToArray());
        }

        [TestMethod]
        public void TestReadColumnsOfAnObjectThatHoldsTheRows()
        {
            // Setup
            var path = Write("{\"meta\":{\"x\":1},\"rows\":[{\"Id\":1,\"Name\":\"A\"}]}");

            // Act
            var actual = JsonHelper.ReadColumns(path);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, actual.ToArray());
        }

        [TestMethod]
        public async Task TestReadColumnsAsyncReturnsThePropertiesOfTheRows()
        {
            // Setup
            var path = Write("[{\"Id\":1,\"Name\":\"A\"},{\"id\":2,\"Birthday\":\"2026-01-01\"}]");

            // Act
            var actual = await JsonHelper.ReadColumnsAsync(path);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Birthday" }, actual.ToArray());
        }

        #endregion

        #region CountRows

        [DataTestMethod]
        [DataRow("[]", 0)]
        [DataRow("[{\"Id\":1}]", 1)]
        [DataRow("[{\"Id\":1},{\"Id\":2},{\"Id\":3}]", 3)]
        [DataRow("{\"meta\":1,\"rows\":[{\"Id\":1},{\"Id\":2}]}", 2)]
        [DataRow("{\"Id\":1}\n{\"Id\":2}\n{\"Id\":3}\n{\"Id\":4}", 4)]
        public void TestCountRowsCountsTheRowsOfEachShape(string json,
            int expected)
        {
            // Setup
            var path = Write(json);

            // Act
            var actual = JsonHelper.CountRows(path);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("[]", 0)]
        [DataRow("[{\"Id\":1},{\"Id\":2},{\"Id\":3}]", 3)]
        [DataRow("{\"meta\":1,\"rows\":[{\"Id\":1},{\"Id\":2}]}", 2)]
        [DataRow("{\"Id\":1}\n{\"Id\":2}\n{\"Id\":3}\n{\"Id\":4}", 4)]
        public async Task TestCountRowsAsyncCountsTheRowsOfEachShape(string json,
            int expected)
        {
            // Setup
            var path = Write(json);

            // Act
            var actual = await JsonHelper.CountRowsAsync(path);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCountRowsOfALargeFile()
        {
            // Setup
            var path = Write("[" + string.Join(",", Enumerable.Range(1, 20000).Select(i => "{\"Id\":" + i + ",\"Name\":\"Name " + i + "\"}")) + "]");

            // Act
            var actual = JsonHelper.CountRows(path);

            // Assert
            Assert.AreEqual(20000, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnCountRowsIfTheFileIsNotValidJson()
        {
            // Setup
            var path = Write("[{\"Id\":1},");

            // Act/Assert
            Assert.Throws<FormatException>(() => JsonHelper.CountRows(path));
        }

        #endregion

        #region GetValue

        [TestMethod]
        public void TestGetValueOfAString()
        {
            // Act
            var actual = JsonHelper.GetValue(Element("\"text\""));

            // Assert
            Assert.AreEqual("text", actual);
        }

        [TestMethod]
        public void TestGetValueOfANumberIsADecimal()
        {
            // Act
            var actual = JsonHelper.GetValue(Element("12.5"));

            // Assert
            Assert.IsInstanceOfType<decimal>(actual);
            Assert.AreEqual(12.5m, actual);
        }

        [TestMethod]
        public void TestGetValueOfANumberThatDoesNotFitADecimalIsADouble()
        {
            // Act
            var actual = JsonHelper.GetValue(Element("1.5e300"));

            // Assert
            Assert.IsInstanceOfType<double>(actual);
            Assert.AreEqual(1.5e300, actual);
        }

        [TestMethod]
        public void TestGetValueOfTheBooleansAndTheNull()
        {
            // Act/Assert
            Assert.AreEqual(true, JsonHelper.GetValue(Element("true")));
            Assert.AreEqual(false, JsonHelper.GetValue(Element("false")));
            Assert.IsNull(JsonHelper.GetValue(Element("null")));
        }

        [TestMethod]
        public void TestGetValueOfAValueWithItsTypeIsTheValueThatItHas()
        {
            // Act
            var number = JsonHelper.GetValue(Element("{\"type\":\"Int32\",\"value\":42}"));
            var text = JsonHelper.GetValue(Element("{\"type\":\"String\",\"value\":\"abc\"}"));
            var empty = JsonHelper.GetValue(Element("{\"type\":\"String\",\"value\":null}"));

            // Assert
            Assert.AreEqual(42m, number);
            Assert.AreEqual("abc", text);
            Assert.IsNull(empty);
        }

        [TestMethod]
        public void TestGetValueOfAnObjectOrAnArrayIsItsText()
        {
            // Act
            var objectText = JsonHelper.GetValue(Element("{\"a\":1}"));
            var arrayText = JsonHelper.GetValue(Element("[1,2]"));

            // Assert
            Assert.AreEqual("{\"a\":1}", objectText);
            Assert.AreEqual("[1,2]", arrayText);
        }

        #endregion

        #region GetValues

        [TestMethod]
        public void TestGetValuesReturnsTheValuesByThePositionOfTheColumns()
        {
            // Setup
            var index = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = 0,
                ["Name"] = 1,
                ["Birthday"] = 2
            };
            using (var row = JsonDocument.Parse("{\"name\":\"A\",\"ID\":5,\"Other\":1}"))
            {
                // Act
                var actual = JsonHelper.GetValues(row, index, 3);

                // Assert
                Assert.AreEqual(3, actual.Length);
                Assert.AreEqual(5m, actual[0]);
                Assert.AreEqual("A", actual[1]);
                Assert.IsNull(actual[2]);
            }
        }

        #endregion

        #region ConvertValue

        [TestMethod]
        public void TestConvertValueOfANullIsNull()
        {
            // Act/Assert
            Assert.IsNull(JsonHelper.ConvertValue(null, typeof(int)));
        }

        [TestMethod]
        public void TestConvertValueKeepsTheValueIfTheTypeIsNotKnown()
        {
            // Act
            var actual = JsonHelper.ConvertValue(12.5m, null);

            // Assert
            Assert.AreEqual(12.5m, actual);
        }

        [TestMethod]
        public void TestConvertValueKeepsTheValueIfItIsOfTheTypeAlready()
        {
            // Act/Assert
            Assert.AreEqual("text", JsonHelper.ConvertValue("text", typeof(string)));
            Assert.AreEqual(5m, JsonHelper.ConvertValue(5m, typeof(decimal)));
            Assert.AreEqual(true, JsonHelper.ConvertValue(true, typeof(bool)));
            Assert.AreEqual("text", JsonHelper.ConvertValue("text", typeof(object)));
        }

        [TestMethod]
        public void TestConvertValueToANumber()
        {
            // Act/Assert
            Assert.AreEqual(5, JsonHelper.ConvertValue(5m, typeof(int)));
            Assert.AreEqual(5L, JsonHelper.ConvertValue(5m, typeof(long)));
            Assert.AreEqual((short)5, JsonHelper.ConvertValue(5m, typeof(short)));
            Assert.AreEqual((byte)5, JsonHelper.ConvertValue(5m, typeof(byte)));
            Assert.AreEqual(5.5, JsonHelper.ConvertValue(5.5m, typeof(double)));
            Assert.AreEqual(5.5f, JsonHelper.ConvertValue(5.5m, typeof(float)));
            Assert.AreEqual(12, JsonHelper.ConvertValue("12", typeof(int)));
            Assert.AreEqual(12.75m, JsonHelper.ConvertValue("12.75", typeof(decimal)));
            Assert.AreEqual(7, JsonHelper.ConvertValue(7.0, typeof(int)));
        }

        [TestMethod]
        public void TestConvertValueToANullableType()
        {
            // Act/Assert
            Assert.AreEqual(5, JsonHelper.ConvertValue(5m, typeof(int?)));
            Assert.AreEqual(Guid.Parse("11111111-1111-1111-1111-111111111111"), JsonHelper.ConvertValue("11111111-1111-1111-1111-111111111111", typeof(Guid?)));
        }

        [TestMethod]
        public void TestConvertValueToAString()
        {
            // Act/Assert
            Assert.AreEqual("1.5", JsonHelper.ConvertValue(1.5m, typeof(string)));
            Assert.AreEqual("true", JsonHelper.ConvertValue(true, typeof(string)));
            Assert.AreEqual("false", JsonHelper.ConvertValue(false, typeof(string)));
        }

        [TestMethod]
        public void TestConvertValueToABoolean()
        {
            // Act/Assert
            Assert.AreEqual(true, JsonHelper.ConvertValue(1m, typeof(bool)));
            Assert.AreEqual(false, JsonHelper.ConvertValue(0m, typeof(bool)));
            Assert.AreEqual(true, JsonHelper.ConvertValue("true", typeof(bool)));
            Assert.AreEqual(false, JsonHelper.ConvertValue("False", typeof(bool)));
        }

        [TestMethod]
        public void TestConvertValueToAGuid()
        {
            // Act
            var actual = JsonHelper.ConvertValue("5f2ae1b6-6a1e-4a4a-9f7c-0d3d1e6b9c10", typeof(Guid));

            // Assert
            Assert.AreEqual(Guid.Parse("5f2ae1b6-6a1e-4a4a-9f7c-0d3d1e6b9c10"), actual);
        }

        [TestMethod]
        public void TestConvertValueToADateTime()
        {
            // Act
            var utc = (DateTime)JsonHelper.ConvertValue("2026-01-02T03:04:05Z", typeof(DateTime));
            var local = (DateTime)JsonHelper.ConvertValue("2026-01-02T03:04:05", typeof(DateTime));

            // Assert
            Assert.AreEqual(new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc), utc);
            Assert.AreEqual(DateTimeKind.Utc, utc.Kind);
            Assert.AreEqual(new DateTime(2026, 1, 2, 3, 4, 5), local);
            Assert.AreEqual(DateTimeKind.Unspecified, local.Kind);
        }

        [TestMethod]
        public void TestConvertValueToADateTimeOffset()
        {
            // Act
            var actual = (DateTimeOffset)JsonHelper.ConvertValue("2026-01-02T03:04:05+08:00", typeof(DateTimeOffset));

            // Assert
            Assert.AreEqual(new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.FromHours(8)), actual);
        }

        [TestMethod]
        public void TestConvertValueToATimeSpan()
        {
            // Act
            var actual = JsonHelper.ConvertValue("01:02:03", typeof(TimeSpan));

            // Assert
            Assert.AreEqual(new TimeSpan(1, 2, 3), actual);
        }

        [TestMethod]
        public void TestConvertValueToTheBytes()
        {
            // Act
            var actual = (byte[])JsonHelper.ConvertValue(Convert.ToBase64String(new byte[] { 1, 2, 3, 250 }), typeof(byte[]));

            // Assert
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 250 }, actual);
        }

        [TestMethod]
        public void TestConvertValueToAnEnum()
        {
            // Act/Assert
            Assert.AreEqual(DayOfWeek.Friday, JsonHelper.ConvertValue("friday", typeof(DayOfWeek)));
            Assert.AreEqual(DayOfWeek.Tuesday, JsonHelper.ConvertValue(2m, typeof(DayOfWeek)));
        }

        [TestMethod]
        public void TestConvertValueToAChar()
        {
            // Act
            var actual = JsonHelper.ConvertValue("x", typeof(char));

            // Assert
            Assert.AreEqual('x', actual);
        }

        [TestMethod]
        public void ThrowExceptionOnConvertValueIfTheNumberHasAFractionAndTheTypeIsAnInteger()
        {
            // Act/Assert
            var exception = Assert.Throws<FormatException>(() => JsonHelper.ConvertValue(1.5m, typeof(int)));
            StringAssert.Contains(exception.Message, "fraction", StringComparison.Ordinal);
            Assert.Throws<FormatException>(() => JsonHelper.ConvertValue(1.5, typeof(long)));
        }

        [TestMethod]
        public void ThrowExceptionOnConvertValueIfTheNumberDoesNotFitTheType()
        {
            // Act/Assert
            Assert.Throws<FormatException>(() => JsonHelper.ConvertValue(300m, typeof(byte)));
        }

        [TestMethod]
        public void ThrowExceptionOnConvertValueIfTheTextIsNotOfTheType()
        {
            // Act/Assert
            var exception = Assert.Throws<FormatException>(() => JsonHelper.ConvertValue("not a guid", typeof(Guid)));
            StringAssert.Contains(exception.Message, "Guid", StringComparison.Ordinal);
            Assert.Throws<FormatException>(() => JsonHelper.ConvertValue("abc", typeof(int)));
            Assert.Throws<FormatException>(() => JsonHelper.ConvertValue("not a date", typeof(DateTime)));
            Assert.Throws<FormatException>(() => JsonHelper.ConvertValue("***", typeof(byte[])));
            Assert.Throws<FormatException>(() => JsonHelper.ConvertValue("maybe", typeof(bool)));
        }

        #endregion
    }
}
