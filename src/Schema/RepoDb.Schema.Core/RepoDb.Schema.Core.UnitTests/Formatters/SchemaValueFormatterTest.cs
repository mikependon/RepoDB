#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Formatters
{
    [TestClass]
    public class SchemaValueFormatterTest
    {
        #region Private Methods

        private static string Format(SchemaValue value,
            string destinationType = null) =>
            new CustomSchemaValueFormatter().Format(value,
                destinationType,
                out _);

        private static string FormatKind(ColumnDefaultValue kind) =>
            new CustomSchemaValueFormatter().FormatKind(kind,
                null,
                out _);

        #endregion

        #region Properties

        [TestMethod]
        public void TestTheFormatterDoesNotHandleTheKindsByDefault()
        {
            // Act
            var handles = new CustomSchemaValueFormatter().HandlesKinds;

            // Assert
            Assert.IsFalse(handles);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestFormatTrue()
        {
            // Act
            var text = Format(SchemaValue.FromBoolean(true));

            // Assert
            Assert.AreEqual("TRUE", text);
        }

        [TestMethod]
        public void TestFormatFalse()
        {
            // Act
            var text = Format(SchemaValue.FromBoolean(false));

            // Assert
            Assert.AreEqual("FALSE", text);
        }

        [TestMethod]
        public void TestFormatANumberAsItsText()
        {
            // Act
            var text = Format(SchemaValue.FromNumber("-12.50"));

            // Assert
            Assert.AreEqual("-12.50", text);
        }

        [TestMethod]
        public void TestFormatATextBetweenQuotes()
        {
            // Act
            var text = Format(SchemaValue.FromText("abc"));

            // Assert
            Assert.AreEqual("'abc'", text);
        }

        [TestMethod]
        public void TestFormatATextWithAQuoteInside()
        {
            // Act
            var text = Format(SchemaValue.FromText("it's"));

            // Assert
            Assert.AreEqual("'it''s'", text);
        }

        [TestMethod]
        public void TestFormatADateAsATypedLiteral()
        {
            // Act
            var text = Format(SchemaValue.FromDate(new DateTime(2020, 1, 31)));

            // Assert
            Assert.AreEqual("DATE '2020-01-31'", text);
        }

        [TestMethod]
        public void TestFormatATimeAsATypedLiteral()
        {
            // Act
            var text = Format(SchemaValue.FromTime(new TimeSpan(10, 20, 30)));

            // Assert
            Assert.AreEqual("TIME '10:20:30'", text);
        }

        [TestMethod]
        public void TestFormatATimeWithFractionalSeconds()
        {
            // Act
            var text = Format(SchemaValue.FromTime(new TimeSpan(10, 20, 30).Add(TimeSpan.FromMilliseconds(500))));

            // Assert
            Assert.AreEqual("TIME '10:20:30.5'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeAsATypedLiteral()
        {
            // Act
            var text = Format(SchemaValue.FromDateTime(new DateTime(2020, 1, 31, 10, 20, 30)));

            // Assert
            Assert.AreEqual("TIMESTAMP '2020-01-31 10:20:30'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeWithTheDigitsOfItsType()
        {
            // Act
            var text = Format(SchemaValue.FromDateTime(new DateTime(2020, 1, 31, 10, 20, 30).AddTicks(1234567)),
                "timestamp(3)");

            // Assert
            Assert.AreEqual("TIMESTAMP '2020-01-31 10:20:30.123'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeWithTheMaximumDigitsIfItsTypeHasNone()
        {
            // Act
            var text = Format(SchemaValue.FromDateTime(new DateTime(2020, 1, 31, 10, 20, 30).AddTicks(1234567)),
                "timestamp");

            // Assert
            Assert.AreEqual("TIMESTAMP '2020-01-31 10:20:30.123456'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeWithoutMoreDigitsThanTheMaximum()
        {
            // Act
            var text = Format(SchemaValue.FromDateTime(new DateTime(2020, 1, 31, 10, 20, 30).AddTicks(1234567)),
                "timestamp(9)");

            // Assert
            Assert.AreEqual("TIMESTAMP '2020-01-31 10:20:30.123456'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeWithoutDigitsIfItsTypeSaysZero()
        {
            // Act
            var text = Format(SchemaValue.FromDateTime(new DateTime(2020, 1, 31, 10, 20, 30).AddTicks(1234567)),
                "timestamp(0)");

            // Assert
            Assert.AreEqual("TIMESTAMP '2020-01-31 10:20:30'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeOffsetAsATypedLiteral()
        {
            // Act
            var text = Format(SchemaValue.FromDateTimeOffset(new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.FromHours(1))));

            // Assert
            Assert.AreEqual("TIMESTAMP WITH TIME ZONE '2020-01-31 10:20:30+01:00'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeOffsetWithANegativeOffset()
        {
            // Act
            var text = Format(SchemaValue.FromDateTimeOffset(new DateTimeOffset(2020, 1, 31, 10, 20, 30, new TimeSpan(-5, -30, 0))));

            // Assert
            Assert.AreEqual("TIMESTAMP WITH TIME ZONE '2020-01-31 10:20:30-05:30'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeOffsetWithoutAWarning()
        {
            // Act
            new CustomSchemaValueFormatter().Format(SchemaValue.FromDateTimeOffset(DateTimeOffset.UtcNow),
                null,
                out var warning);

            // Assert
            Assert.IsNull(warning);
        }

        [TestMethod]
        public void TestFormatAGuidBetweenQuotes()
        {
            // Act
            var text = Format(SchemaValue.FromGuid(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF")));

            // Assert
            Assert.AreEqual("'6f9619ff-8b86-d011-b42d-00c04fc964ff'", text);
        }

        [TestMethod]
        public void TestFormatABinaryAsNullBecauseTheAnsiFormatHasNone()
        {
            // Act
            var text = Format(SchemaValue.FromBinary(new byte[] { 1 }));

            // Assert
            Assert.IsNull(text);
        }

        #endregion

        #region FormatKind

        [TestMethod]
        public void TestFormatKindCurrentTimestamp()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestamp);

            // Assert
            Assert.AreEqual("CURRENT_TIMESTAMP", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentDate()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentDate);

            // Assert
            Assert.AreEqual("CURRENT_DATE", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTime()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTime);

            // Assert
            Assert.AreEqual("CURRENT_TIME", text);
        }

        [TestMethod]
        public void TestFormatKindWithoutAnAnsiEquivalentAsNull()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewGuid);

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatKindWithoutAWarning()
        {
            // Act
            new CustomSchemaValueFormatter().FormatKind(ColumnDefaultValue.CurrentDate,
                null,
                out var warning);

            // Assert
            Assert.IsNull(warning);
        }

        #endregion
    }
}
