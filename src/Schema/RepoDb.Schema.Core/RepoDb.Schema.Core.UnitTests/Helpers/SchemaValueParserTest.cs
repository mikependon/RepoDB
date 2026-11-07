#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    [TestClass]
    public class SchemaValueParserTest
    {
        #region Boolean

        [TestMethod]
        public void TestParseAWrappedZeroAsFalse()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Boolean,
                "((0))",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(false, value.Value);
        }

        [TestMethod]
        public void TestParseAWrappedOneAsTrue()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Boolean,
                "((1))",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(true, value.Value);
        }

        [TestMethod]
        public void TestParseABareWordAsABooleanWithoutCase()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Boolean,
                "TRUE",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(true, value.Value);
        }

        [TestMethod]
        public void TestParseAQuotedLetterAsABoolean()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Boolean,
                "'f'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(false, value.Value);
        }

        [TestMethod]
        public void TestParseABooleanWithACast()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Boolean,
                "'true'::boolean",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(true, value.Value);
        }

        [TestMethod]
        public void TestParseABitLiteralAsABoolean()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Boolean,
                "b'1'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(true, value.Value);
        }

        [TestMethod]
        public void TestParseYesAsTrue()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Boolean,
                "'Yes'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(true, value.Value);
        }

        [TestMethod]
        public void TestParseOffAsFalse()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Boolean,
                "'off'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(false, value.Value);
        }

        [TestMethod]
        public void TestParseAWordThatIsNotABooleanAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Boolean,
                "'maybe'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        #endregion

        #region Number

        [TestMethod]
        public void TestParseAWrappedNumber()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Number,
                "((12.50))",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual("12.50", value.Value);
        }

        [TestMethod]
        public void TestParseANegativeNumber()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Number,
                "(-5)",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual("-5", value.Value);
        }

        [TestMethod]
        public void TestParseAQuotedNumberWithACast()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Number,
                "'0'::numeric",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual("0", value.Value);
        }

        [TestMethod]
        public void TestParseANumberWithAPlusSign()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Number,
                "+1.5",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual("1.5", value.Value);
        }

        [TestMethod]
        public void TestParseANumberWithAnExponent()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Number,
                "1.5e3",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual("1.5e3", value.Value);
        }

        [TestMethod]
        public void TestParseATextThatIsNotANumberAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Number,
                "abc",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        #endregion

        #region Text

        [TestMethod]
        public void TestParseATextWithANationalPrefix()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Text,
                "N'abc'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual("abc", value.Value);
        }

        [TestMethod]
        public void TestParseATextWithADoubledQuote()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Text,
                "'it''s'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual("it's", value.Value);
        }

        [TestMethod]
        public void TestParseATextWithACast()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Text,
                "'x'::character varying",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual("x", value.Value);
        }

        [TestMethod]
        public void TestParseATextWithACharacterSetPrefix()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Text,
                "_utf8mb4'x'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual("x", value.Value);
        }

        [TestMethod]
        public void TestParseAWrappedText()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Text,
                "('abc')",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual("abc", value.Value);
        }

        [TestMethod]
        public void TestParseAnEscapeTextAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Text,
                "E'a\\nb'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TestParseATextWithoutQuotesAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Text,
                "abc",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TestParseATextThatHasTheCastOperatorInsideAsIs()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Text,
                "'a::b'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual("a::b", value.Value);
        }

        #endregion

        #region Date And Time

        [TestMethod]
        public void TestParseAWrappedDate()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Date,
                "('1900-01-01')",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new DateTime(1900, 1, 1), value.Value);
        }

        [TestMethod]
        public void TestParseATypedDate()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Date,
                "DATE '2020-02-29'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new DateTime(2020, 2, 29), value.Value);
        }

        [TestMethod]
        public void TestParseADateThatDoesNotExistAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Date,
                "'2020-13-45'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TestParseADateWithoutQuotesAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Date,
                "2020-01-01",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TestParseATime()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Time,
                "'10:20:30'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new TimeSpan(10, 20, 30), value.Value);
        }

        [TestMethod]
        public void TestParseATimeWithFractionalSeconds()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Time,
                "'10:20:30.1234567'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new TimeSpan(10, 20, 30).Add(TimeSpan.FromTicks(1234567)), value.Value);
        }

        [TestMethod]
        public void TestParseATimeWithoutSeconds()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Time,
                "'10:20'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new TimeSpan(10, 20, 0), value.Value);
        }

        [TestMethod]
        public void TestParseATimeThatDoesNotExistAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Time,
                "'25:00'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TestParseATimeWithoutQuotesAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Time,
                "10:20:30",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TestParseADateTimeWithTheTSeparator()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTime,
                "'2020-01-31T10:20:30'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new DateTime(2020, 1, 31, 10, 20, 30), value.Value);
        }

        [TestMethod]
        public void TestParseADateTimeWithFractionalSeconds()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTime,
                "'2020-01-31 10:20:30.125'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new DateTime(2020, 1, 31, 10, 20, 30, 125), value.Value);
        }

        [TestMethod]
        public void TestParseATypedDateTime()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTime,
                "TIMESTAMP '2020-01-31 10:20:30'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new DateTime(2020, 1, 31, 10, 20, 30), value.Value);
        }

        [TestMethod]
        public void TestParseADateTimeThatIsOnlyADate()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTime,
                "'1900-01-01'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new DateTime(1900, 1, 1), value.Value);
        }

        [TestMethod]
        public void TestParseADateTimeThatIsNotInTheIsoFormatAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTime,
                "'31/01/2020 10:20'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TestParseADateTimeWithoutQuotesAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTime,
                "getdate()",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        #endregion

        #region Date And Time With An Offset

        [TestMethod]
        public void TestParseAnOffsetWithColon()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTimeOffset,
                "'2020-01-31T10:20:30+01:00'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.FromHours(1)), value.Value);
        }

        [TestMethod]
        public void TestParseAnOffsetWithoutMinutes()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTimeOffset,
                "'2020-01-31 10:20:30+01'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.FromHours(1)), value.Value);
        }

        [TestMethod]
        public void TestParseAnOffsetThatIsUtc()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTimeOffset,
                "'2020-01-31T10:20:30Z'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.Zero), value.Value);
        }

        [TestMethod]
        public void TestParseAnOffsetWithASpace()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTimeOffset,
                "TIMESTAMP '2020-01-31 10:20:30 -05:30'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new DateTimeOffset(2020, 1, 31, 10, 20, 30, new TimeSpan(-5, -30, 0)), value.Value);
        }

        [TestMethod]
        public void TestParseAnOffsetWithFractionalSeconds()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTimeOffset,
                "'2020-01-31 10:20:30.5+00:00'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new DateTimeOffset(2020, 1, 31, 10, 20, 30, 500, TimeSpan.Zero), value.Value);
        }

        [TestMethod]
        public void TestParseADateTimeWithoutAnOffsetAsUtc()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTimeOffset,
                "'2020-01-31 10:20:30'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(TimeSpan.Zero, ((DateTimeOffset)value.Value).Offset);
        }

        [TestMethod]
        public void TestParseAnOffsetThatIsNotInTheIsoFormatAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTimeOffset,
                "'Jan 31 2020'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TestParseAnOffsetWithoutQuotesAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.DateTimeOffset,
                "sysdatetimeoffset()",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        #endregion

        #region Guid

        [TestMethod]
        public void TestParseAGuid()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Guid,
                "'6F9619FF-8B86-D011-B42D-00C04FC964FF'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF"), value.Value);
        }

        [TestMethod]
        public void TestParseAGuidWithBraces()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Guid,
                "'{6F9619FF-8B86-D011-B42D-00C04FC964FF}'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF"), value.Value);
        }

        [TestMethod]
        public void TestParseATextThatIsNotAGuidAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Guid,
                "'not-a-guid'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        #endregion

        #region Binary

        [TestMethod]
        public void TestParseABinaryAsAHexadecimalNumber()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Binary,
                "0xDEAD",
                CustomSchemaDialect.Instance);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0xDE, 0xAD }, (byte[])value.Value);
        }

        [TestMethod]
        public void TestParseABinaryAsAHexadecimalLiteral()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Binary,
                "X'BEEF'",
                CustomSchemaDialect.Instance);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0xBE, 0xEF }, (byte[])value.Value);
        }

        [TestMethod]
        public void TestParseABinaryAsAHexadecimalText()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Binary,
                "'\\xCAFE'",
                CustomSchemaDialect.Instance);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0xCA, 0xFE }, (byte[])value.Value);
        }

        [TestMethod]
        public void TestParseAnEmptyBinary()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Binary,
                "0x",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(0, ((byte[])value.Value).Length);
        }

        [TestMethod]
        public void TestParseABinaryWithAnOddNumberOfDigitsAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Binary,
                "0xDEA",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TestParseABinaryWithDigitsThatAreNotHexadecimalAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Binary,
                "0xZZ",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TestParseATextThatIsNotABinaryAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Binary,
                "'abc'",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TestParseAnExpressionThatIsNotABinaryAsNull()
        {
            // Act
            var value = SchemaValueParser.Parse(SchemaValueKind.Binary,
                "newid()",
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(value);
        }

        #endregion
    }
}
